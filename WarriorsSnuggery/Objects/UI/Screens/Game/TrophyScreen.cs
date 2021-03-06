﻿using WarriorsSnuggery.Graphics;
using WarriorsSnuggery.Objects;
using WarriorsSnuggery.Trophies;

namespace WarriorsSnuggery.UI
{
	public class TrophyScreen : Screen
	{
		readonly Game game;

		readonly PanelList trophies;
		readonly TextBlock information;

		public TrophyScreen(Game game) : base("Trophy Collection")
		{
			this.game = game;
			Title.Position = new CPos(0, -4096, 0);

			trophies = new PanelList(new CPos(0, -1024, 0), new MPos(8120, 1024), new MPos(512, 1024), PanelManager.Get("wooden"));
			foreach (var key in TrophyManager.Trophies.Keys)
			{
				var value = TrophyManager.Trophies[key];

				var sprite = value.Image.GetTextures()[0];
				var scale = (sprite.Width > sprite.Height ? 24f / sprite.Width : 24f / sprite.Height) - 0.1f;
				var item = new PanelItem(CPos.Zero, new BatchObject(sprite, Color.White), new MPos(512, 512), value.Name, new string[0], () => selectTrophy(key, value))
				{
					Scale = scale * 2f
				};
				if (!game.Statistics.UnlockedTrophies.Contains(key))
					item.SetColor(Color.Black);

				trophies.Add(item);
			}

			information = new TextBlock(new CPos(-7900, 512, 0), FontManager.Pixel16, TextLine.OffsetType.LEFT, "Select a trophy for further information.", "", "");

			Content.Add(new Panel(new CPos(0, 1024, 0), new Vector(8, 1, 0), PanelManager.Get("stone")));
			Content.Add(new Button(new CPos(0, 6144, 0), "Resume", "wooden", () => { game.Pause(false); game.ScreenControl.ShowScreen(ScreenType.DEFAULT); }));
		}

		void selectTrophy(string name, Trophy trophy)
		{
			if (!game.Statistics.UnlockedTrophies.Contains(name))
			{
				information.Lines[0].WriteText(Color.Red + "Locked Trophy");
				information.Lines[1].WriteText(Color.Grey + " ");
				information.Lines[1].WriteText(Color.Grey + " ");
				return;
			}

			information.Lines[0].WriteText(Color.White + trophy.Name);
			information.Lines[1].WriteText(Color.Grey + trophy.Description);
			information.Lines[2].WriteText(Color.Grey + (trophy.MaxManaIncrease != 0 ? "Gives " + Color.Blue + trophy.MaxManaIncrease + Color.Grey + " additional mana storage!" : " "));
		}

		public override void Render()
		{
			base.Render();

			trophies.Render();
			information.Render();
		}

		public override void Tick()
		{
			base.Tick();

			trophies.Tick();
			information.Tick();

			if (KeyInput.IsKeyDown("escape", 10))
			{
				game.Pause(false);
				game.ChangeScreen(ScreenType.DEFAULT);
			}
		}

		public override void Show()
		{
			var i = 0;
			foreach (var key in TrophyManager.Trophies.Keys)
			{
				if (!game.Statistics.UnlockedTrophies.Contains(key))
					trophies.Container[i].SetColor(Color.Black);
				else
					trophies.Container[i].SetColor(Color.White);

				i++;
			}
		}

		public override void Hide()
		{
			trophies.DisableTooltip();
		}
	}
}
