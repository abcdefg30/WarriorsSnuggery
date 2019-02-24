﻿using System;
using WarriorsSnuggery.Objects;

namespace WarriorsSnuggery.UI
{
	public class DeathScreen : Screen
	{
		readonly Button restart;
		readonly Button menu;
		readonly Text score;
		readonly Text deaths;
		readonly Game game;
		bool firsttick = true;

		public DeathScreen(Game game) : base("Your Death was Inevitable.")
		{
			this.game = game;
			Title.SetColor(Color.Red);
			Speed = 64;

			score = new Text(new CPos(0,1024,0), IFont.Pixel16, Text.OffsetType.MIDDLE);
			deaths = new Text(new CPos(0, 2048, 0), IFont.Pixel16, Text.OffsetType.MIDDLE);

			restart = ButtonCreator.Create("wooden", new CPos(-2048, 5120,0), "Restart Map", () => Window.Current.NewGame(game.Stats, sameSeed: true));
			menu = ButtonCreator.Create("wooden", new CPos(2048, 5120, 0), "Main Menu", () => Window.Current.NewGame(game.Stats, GameType.MENU));
		}

		public override void Render()
		{
			base.Render();

			restart.Render();
			menu.Render();
			score.Render();
			deaths.Render();
		}

		public override void Tick()
		{
			base.Tick();

			restart.Tick();
			menu.Tick();

			if (firsttick)
			{
				firsttick = false;
				score.WriteText("Score: " + Color.Blue + (game.Stats.Level * game.Stats.LevelToReach + game.Stats.Mana * 3 - game.Stats.Deaths * 7 + game.Stats.Kills * 4));
				deaths.WriteText(Color.Red + "Deaths: " + game.Stats.Deaths);
			}
			score.Tick();
			deaths.Tick();
		}

		public override void Dispose()
		{
			base.Dispose();

			restart.Dispose();
			menu.Dispose();
			score.Dispose();
			deaths.Dispose();
		}
	}
}