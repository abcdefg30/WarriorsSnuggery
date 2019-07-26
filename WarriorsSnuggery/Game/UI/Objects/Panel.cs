﻿using System;
using WarriorsSnuggery.Graphics;
using WarriorsSnuggery.Objects;

namespace WarriorsSnuggery.UI
{
	public class Panel : IPositionable, ITickRenderable, IDisposable
	{
		public virtual CPos Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
				inner.SetPosition(position);
				outer.SetPosition(position);
				if (Highlight != null)
					Highlight.SetPosition(position);
			}
		}
		CPos position;

		public virtual VAngle Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				rotation = value;
				inner.SetRotation(rotation);
				outer.SetRotation(rotation);
				if (Highlight != null)
					Highlight.SetRotation(rotation);
			}
		}
		VAngle rotation;

		public virtual float Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
				inner.SetScale(scale);
				outer.SetScale(scale);
				if (Highlight != null)
					Highlight.SetScale(scale);
			}
		}
		float scale = 1f;

		public virtual Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				inner.SetColor(color);
				outer.SetColor(color);
				if (Highlight != null)
					Highlight.SetColor(color);
			}
		}
		Color color = Color.White;

		readonly GraphicsObject inner;
		readonly GraphicsObject outer;
		public readonly GraphicsObject Highlight;

		public bool HighlightVisible;

		public Panel(CPos position, MPos size, PanelType type) : this(position, size, type.Border, type.DefaultString, type.BorderString, type.ActiveString)
		{
		}
		public Panel(CPos position, MPos size, int border, string inner, string outer, string highlight) : this(position, size, border, inner, outer, highlight != "" ? new ImageRenderable(TextureManager.Texture(highlight), size) : null)
		{
		}

		public Panel(CPos position, MPos size, int border, string inner, string outer, ImageRenderable highlight)
		{
			this.inner = new ImageRenderable(TextureManager.Texture(inner), size);
			this.outer = new ImageRenderable(TextureManager.Texture(outer), size + new MPos(border, border));
			if (highlight != null) Highlight = highlight;

			Position = position;
		}

		public virtual void Render()
		{
			outer.Render();
			inner.Render();

			if (HighlightVisible && Highlight != null)
				Highlight.Render();
		}

		public virtual void Tick()
		{

		}

		public virtual void Dispose()
		{
			// Does not need any dispose
		}
	}
}