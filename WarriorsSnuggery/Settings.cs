using OpenToolkit.Windowing.Common.Input;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace WarriorsSnuggery
{
	public static class Settings
	{
		public static readonly IFormatProvider FloatFormat = CultureInfo.InvariantCulture;

		public const int MaxTeams = 8;

		public const string Version = "(Release) 2.2";

		public const int UpdatesPerSecond = 60;

		public static int BatchSize = 4096;

		public static int MaxSheets = 4;

		public static int SheetSize = 1024;
		public static float SheetHalfPixel = 0.1f / SheetSize;

		public static int FrameLimiter = 0;

		public static float ScrollSpeed = 6;

		public static int EdgeScrolling = 4;

		public static bool DeveloperMode = false;

		public static bool EnableDebug { get { return DeveloperMode; } private set { } }

		public static bool EnableInfoScreen;

		public static bool Fullscreen = true;

		public static int Width = 1920;

		public static int Height = 1080;

		public static bool PartyMode = false;

		public static bool AntiAliasing = false;

		public static bool EnablePixeling = false;

		public static bool EnableTextShadowing = true;

		public static bool FirstStarted = true;

		public static float MasterVolume = 1f;

		public static float EffectsVolume = 1f;

		public static float MusicVolume = 0.5f;

		public static Dictionary<string, Key> KeyDictionary = new Dictionary<string, Key>();

		public static Key GetKey(string value)
		{
			if (!KeyDictionary.ContainsKey(value))
				throw new YamlInvalidNodeException(string.Format("Unable to find keyboard key with name {0}.", value));

			return KeyDictionary[value];
		}

		public static void Initialize(bool newSettings)
		{
			if (!newSettings && FileExplorer.Exists(FileExplorer.MainDirectory, "Settings.yaml"))
				load();
			else
				defaultKeys();

			// Set FirstStarted to 0.
			if (FirstStarted)
				Save();
		}

		static void load()
		{
			foreach (var node in RuleReader.Read(FileExplorer.MainDirectory, "Settings.yaml"))
			{
				switch (node.Key)
				{
					case "BatchSize":
						BatchSize = node.Convert<int>();

						break;
					case "MaxSheets":
						MaxSheets = node.Convert<int>();

						break;
					case "SheetSize":
						SheetSize = node.Convert<int>();
						SheetHalfPixel = 0.1f / SheetSize;

						break;
					case "FrameLimiter":
						FrameLimiter = node.Convert<int>();

						break;
					case "ScrollSpeed":
						ScrollSpeed = node.Convert<float>();
						break;
					case "EdgeScrolling":
						EdgeScrolling = node.Convert<int>();
						break;
					case "DeveloperMode":
						DeveloperMode = node.Convert<bool>();
						break;
					case "Fullscreen":
						Fullscreen = node.Convert<bool>();
						break;
					case "Width":
						Width = node.Convert<int>();
						break;
					case "Height":
						Height = node.Convert<int>();
						break;
					case "AntiAliasing":
						AntiAliasing = node.Convert<bool>();
						break;
					case "EnablePixeling":
						EnablePixeling = node.Convert<bool>();
						break;
					case "EnableTextShadowing":
						EnableTextShadowing = node.Convert<bool>();
						break;
					case "FirstStarted":
						FirstStarted = node.Convert<bool>();
						break;
					case "MasterVolume":
						MasterVolume = node.Convert<float>();
						break;
					case "EffectsVolume":
						EffectsVolume = node.Convert<float>();
						break;
					case "MusicVolume":
						MusicVolume = node.Convert<float>();
						break;
					case "Keys":
						foreach (var key in node.Children)
							KeyDictionary.Add(key.Key, KeyInput.ToKey(key.Value));
						break;
				}
			}
		}

		static void defaultKeys()
		{
			KeyDictionary.Clear();
			KeyDictionary.Add("Pause", Key.P);
			KeyDictionary.Add("CameraLock", Key.L);
			KeyDictionary.Add("MoveUp", Key.W);
			KeyDictionary.Add("MoveDown", Key.S);
			KeyDictionary.Add("MoveLeft", Key.A);
			KeyDictionary.Add("MoveRight", Key.D);
			KeyDictionary.Add("MoveAbove", Key.E);
			KeyDictionary.Add("MoveBelow", Key.R);
			KeyDictionary.Add("CameraUp", Key.Up);
			KeyDictionary.Add("CameraDown", Key.Down);
			KeyDictionary.Add("CameraLeft", Key.Left);
			KeyDictionary.Add("CameraRight", Key.Right);
		}

		public static void Save()
		{
			using var writer = new System.IO.StreamWriter(FileExplorer.MainDirectory + "Settings.yaml");

			writer.WriteLine("BatchSize=" + BatchSize);
			writer.WriteLine("MaxSheets=" + MaxSheets);
			writer.WriteLine("SheetSize=" + SheetSize);
			writer.WriteLine("FrameLimiter=" + FrameLimiter);
			writer.WriteLine("ScrollSpeed=" + ScrollSpeed.ToString(FloatFormat));
			writer.WriteLine("EdgeScrolling=" + EdgeScrolling);
			writer.WriteLine("DeveloperMode=" + DeveloperMode.GetHashCode());
			writer.WriteLine("Fullscreen=" + Fullscreen.GetHashCode());
			writer.WriteLine("Width=" + Width);
			writer.WriteLine("Height=" + Height);
			writer.WriteLine("AntiAliasing=" + AntiAliasing.GetHashCode());
			writer.WriteLine("EnablePixeling=" + EnablePixeling.GetHashCode());
			writer.WriteLine("EnableTextShadowing=" + EnableTextShadowing.GetHashCode());
			writer.WriteLine("FirstStarted=" + 0);
			writer.WriteLine("MasterVolume=" + MasterVolume.ToString(FloatFormat));
			writer.WriteLine("EffectsVolume=" + EffectsVolume.ToString(FloatFormat));
			writer.WriteLine("MusicVolume=" + MusicVolume.ToString(FloatFormat));

			writer.WriteLine("Keys=");
			foreach (var key in KeyDictionary)
				writer.WriteLine("\t" + key.Key + "=" + key.Value);

			writer.Flush();
			writer.Close();
		}
	}
}
