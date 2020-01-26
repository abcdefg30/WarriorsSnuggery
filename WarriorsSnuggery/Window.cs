using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System;
using System.Drawing;
using WarriorsSnuggery.Graphics;

namespace WarriorsSnuggery
{
	public static class WindowInfo
	{
		public static int Height;

		public static int Width;

		public static float Ratio { get { return Width / (float)Height; } }

		public const float UnitHeight = Camera.DefaultZoom;

		public static float UnitWidth { get { return Camera.DefaultZoom * Ratio; } private set { } }

		public static bool Focused = true;
	}

	public class Window : GameWindow
	{
		static Window current;
		const string title = "Warrior's Snuggery";

		public static char CharInput;

		public static uint GlobalTick;
		public static uint GlobalRender;

		public static bool Ready;
		public static bool Exiting;

		public Window() : base(Settings.Width, Settings.Height, GraphicsMode.Default, title)
		{
			current = this;
			SetScreen();
			CursorVisible = false;
		}

		public static void CloseWindow()
		{
			current.Exit();
		}

		public static void UpdateScreen()
		{
			current.SetScreen();
		}

		public void SetScreen()
		{
			if (Settings.Fullscreen && !Program.isDebug)
			{
				WindowBorder = WindowBorder.Hidden;
				WindowState = WindowState.Fullscreen;
				Width = ClientRectangle.Width;
				Height = ClientRectangle.Height;
			}
			else
			{
				WindowBorder = WindowBorder.Fixed;
				WindowState = WindowState.Normal;
				var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
				X = bounds.Width / 2 - Settings.Width / 2;
				Y = bounds.Height / 2 - Settings.Height / 2;
				Width = Settings.Width;
				Height = Settings.Height;
			}
			WarriorsSnuggery.WindowInfo.Height = Height;
			WarriorsSnuggery.WindowInfo.Width = Width;

			MasterRenderer.UpdateView();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (Height == 0 || Width == 0)
				return;

			WarriorsSnuggery.WindowInfo.Height = Height;
			WarriorsSnuggery.WindowInfo.Width = Width;

			ColorManager.WindowRescaled();

			MasterRenderer.UpdateView();
		}

		protected override void OnLoad(EventArgs e)
		{
			Console.Write("Loading...");

			MasterRenderer.Initialize();

			base.OnLoad(e);

			var font = Timer.Start();
			Icon = new Icon(FileExplorer.Misc + "/warsnu.ico");
			IFont.LoadFonts();
			IFont.InitializeFonts();
			CharManager.Initialize();

			font.StopAndWrite("Loading Fonts");

			var watch2 = Timer.Start();
			AudioController.Load();

			watch2.StopAndWrite("Loading Sound");

			var watch = Timer.Start();
			GameController.Load();

			watch.StopAndWrite("Loading Rules");


			Ready = true;
			Console.WriteLine(" Done!");
			Console.WriteLine("Textures: " + TextureManager.TextureCount);

			// For multithreads
			//IGraphicsContext context2 = new GraphicsContext(GraphicsMode.Default, this.WindowInfo);
			//context2.MakeCurrent(WindowInfo);
		}

		public static double TPS;
		public static double TMS;
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if (!Ready)
				return;

			Timer watch = null;
			if (GlobalTick % 20 == 0)
				watch = Timer.Start();

			GameController.Tick();
			AudioController.Tick();

			CharInput = '';

			if (KeyInput.IsKeyDown(Key.F4) && (KeyInput.IsKeyDown(Key.AltLeft) || KeyInput.IsKeyDown(Key.AltRight)))
				Program.Exit();

			if (KeyInput.IsKeyDown(Key.N, 10))
				AudioController.Music.Next();

			if (GlobalTick % 20 == 0)
			{
				TPS = 1 / e.Time;
				TMS = watch.Stop();
			}

			GlobalTick++;
		}

		public static double FPS;
		public static double FMS;
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			if (!Ready || Exiting)
				return;

			Timer watch = null;
			if (GlobalRender % 20 == 0)
				watch = Timer.Start();

			MasterRenderer.Render();

			lock (MasterRenderer.GLLock)
			{
				SwapBuffers();
			}

			if (GlobalRender % 20 == 0)
			{
				watch.StopAndWrite("render" + GlobalRender);
				FPS = 1 / e.Time;
				FMS = watch.Stop();
			}

			Title = title + " | " + MasterRenderer.RenderCalls + " Calls | " + MasterRenderer.Batches + " Batches | " + MasterRenderer.BatchCalls + " BatchCalls";
			GlobalRender++;
		}

		protected override void OnFocusedChanged(EventArgs e)
		{
			WarriorsSnuggery.WindowInfo.Focused = Focused;
			if (!Focused && !Program.isDebug && Ready)
				GameController.Pause();
		}

		public override void Exit()
		{
			Exiting = true;

			TextureManager.DeleteTextures();
			ColorManager.Dispose();
			CharManager.Dispose();

			WorldRenderer.TerrainRenderer.Dispose();
			WorldRenderer.SmudgeRenderer.Dispose();
			WorldRenderer.ObjectRenderer.Dispose();
			WorldRenderer.ShroudRenderer.Dispose();
			UIRenderer.UIBatchRenderer.Dispose();
			MasterRenderer.Dispose();

			TerrainSpriteManager.DeleteTexture();
			SpriteManager.DeleteTextures();

			IImage.DisposeImages();
			IFont.DisposeFonts();

			base.Exit();
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			MouseInput.UpdateMousePosition(new MPos(e.Position.X, e.Position.Y));
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e)
		{
			KeyInput.KeyPressed(e);
			var str = e.Key.ToString();
			if (str.Length == 1)
			{
				CharInput = str[0];
				if (!e.Shift)
					CharInput = char.ToLower(CharInput);
			}
		}
	}
}
