﻿using System;
using WarriorsSnuggery.Graphics;

namespace WarriorsSnuggery.Objects
{
	public class TextBlock : ITickRenderable, IPositionable, IDisposable
	{
		public CPos Position
		{
			get { return position; }
			set
			{
				position = value;

				for (int i = 0; i < Lines.Length; i++)
				{
					Lines[i].Position = position + new CPos(0, (font.Gap + font.Height) * i, 0);
				}
			}
		}
		CPos position;

		public VAngle Rotation
		{
			get { return rotation; }
			set
			{
				rotation = value;

				for (int i = 0; i < Lines.Length; i++)
				{
					Lines[i].Rotation = rotation;
				}
			}
		}
		VAngle rotation;

		public float Scale
		{
			get { return scale; }
			set
			{
				scale = value;

				for (int i = 0; i < Lines.Length; i++)
				{
					Lines[i].Scale = scale;
				}
			}
		}
		float scale = 1f;

		readonly IFont font;
		public readonly TextLine[] Lines = new TextLine[0];

		public TextBlock(CPos position, IFont font, TextLine.OffsetType type, params string[] text)
		{
			Position = position;
			this.font = font;
			Lines = new TextLine[text.Length];

			for (int i = 0; i < text.Length; i++)
			{
				Lines[i] = new TextLine(position + new CPos(0, (font.Gap + font.Height) * i, 0), font, type);
				Lines[i].WriteText(text[i]);
			}
		}

		public void Render()
		{
			foreach (var line in Lines)
				line.Render();
		}

		public void Tick()
		{
			foreach (var line in Lines)
				line.Tick();
		}

		public void Dispose()
		{
			foreach (var line in Lines)
				line.Dispose();
		}
	}
}