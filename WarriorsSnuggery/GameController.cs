﻿using WarriorsSnuggery.Maps;

namespace WarriorsSnuggery
{
	public static class GameController
	{
		static Game game;

		public static void Tick()
		{
			KeyInput.Tick();
			MouseInput.Tick();

			game.Tick();
		}

		public static void Load()
		{
			RuleLoader.LoadRules();

			PieceManager.RefreshPieces();

			MapCreator.LoadTypes(FileExplorer.Maps, "maps.yaml");

			GameSaveManager.Load();
			GameSaveManager.DefaultStatistic = GameStatistics.LoadGameStatistic("DEFAULT");

			createFirst();
		}

		static void createFirst()
		{
			var map = Program.MapType != null ? MapCreator.GetType(Program.MapType) : MapCreator.FindMainMenuMap(0);

			game = new Game(new GameStatistics(GameSaveManager.DefaultStatistic), map);
			game.Load();

			if (Program.StartEditor)
				game.SwitchEditor();
		}

		public static void CreateReturn(GameType type)
		{
			var stats = game.OldStatistics;

			game.Finish();
			game.Dispose();

			game = type switch
			{
				GameType.TUTORIAL => new Game(stats, MapCreator.FindTutorial()),
				GameType.MENU => new Game(stats, MapCreator.FindMainMap(stats.Level)),
				GameType.MAINMENU => new Game(stats, MapCreator.FindMainMenuMap(stats.Level)),
				_ => new Game(stats, MapCreator.FindMap(stats.Level)),
			};
			game.Load();
		}

		public static void CreateRestart()
		{
			var stats = game.OldStatistics;

			game.Finish();
			game.Dispose();

			game = new Game(stats, game.MapType, game.Seed);
			game.Load();
		}

		public static void CreateNext(GameType type)
		{
			var stats = game.Statistics;

			game.Finish();
			game.Dispose();

			game = type switch
			{
				GameType.TUTORIAL => new Game(stats, MapCreator.FindTutorial()),
				GameType.MENU => new Game(stats, MapCreator.FindMainMap(stats.Level)),
				GameType.MAINMENU => new Game(stats, MapCreator.FindMainMenuMap(stats.Level)),
				_ => new Game(stats, MapCreator.FindMap(stats.Level)),
			};
			game.Load();
		}

		public static void CreateNew(GameStatistics stats, GameType type = GameType.NORMAL, MapInfo custom = null, bool loadStatsMap = false)
		{
			game.Finish();
			game.Dispose();

			if (loadStatsMap)
			{
				try
				{
					custom = MapInfo.MapTypeFromSave(stats);
				}
				catch (System.IO.FileNotFoundException)
				{
					Log.WriteDebug(string.Format("Unable to load saved map of save '{0}'. Using a random map.", stats.SaveName));
				}
			}

			game = type switch
			{
				GameType.TUTORIAL => new Game(stats, custom ?? MapCreator.FindTutorial()),
				GameType.MENU => new Game(stats, custom ?? MapCreator.FindMainMap(stats.Level)),
				GameType.MAINMENU => new Game(stats, custom ?? MapCreator.FindMainMenuMap(stats.Level)),
				_ => new Game(stats, custom ?? MapCreator.FindMap(stats.Level)),
			};
			game.Load();
		}

		public static void AddInfoMessage(int duration, string text)
		{
			game.AddInfoMessage(duration, text);
		}

		public static void Pause()
		{
			if (!game.Paused)
				game.ChangeScreen(UI.ScreenType.PAUSED);

			game.Pause(true);
		}

		public static void Exit()
		{
			if (game != null)
			{
				game.Finish();
				game.Dispose();
			}
		}
	}
}
