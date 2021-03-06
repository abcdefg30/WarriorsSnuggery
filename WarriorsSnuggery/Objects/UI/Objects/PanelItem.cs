﻿using System;
using WarriorsSnuggery.Graphics;

namespace WarriorsSnuggery.UI
{
	public class PanelItem : ITickRenderable, IDisableTooltip
	{
		public virtual bool Visible
		{
			get { return renderable.Visible; }
			set { renderable.Visible = value; }
		}

		public virtual CPos Position
		{
			get { return position; }
			set
			{
				renderable.SetPosition(value);
				position = value;
			}
		}
		CPos position;

		public virtual VAngle Rotation
		{
			get { return rotation; }
			set
			{
				rotation = value;
				renderable.SetRotation(rotation);
			}
		}
		VAngle rotation;

		public virtual float Scale
		{
			get { return scale; }
			set
			{
				scale = value;
				renderable.SetScale(scale);
			}
		}
		float scale = 1f;

		readonly BatchRenderable renderable;
		readonly Action action;
		protected readonly MPos size;

		protected bool mouseOnItem;

		protected readonly Tooltip tooltip;

		public PanelItem(CPos pos, BatchRenderable renderable, MPos size, string title, string[] text, Action action)
		{
			tooltip = new Tooltip(pos, title, text);
			this.renderable = renderable;
			this.action = action;
			this.size = size;
			position = pos;
		}

		public virtual void SetColor(Color color)
		{
			renderable.SetColor(color);
		}

		public virtual void Render()
		{
			renderable.PushToBatchRenderer();
		}

		public virtual void Tick()
		{
			if (!Visible)
				return;

			checkMouse();
		}

		public virtual void DisableTooltip()
		{
			UIRenderer.DisableTooltip(tooltip);
		}

		void checkMouse()
		{
			var mousePosition = MouseInput.WindowPosition;

			mouseOnItem = mousePosition.X > position.X - size.X && mousePosition.X < position.X + size.X && mousePosition.Y > position.Y - size.Y && mousePosition.Y < position.Y + size.Y;

			if (mouseOnItem)
			{
				UIRenderer.SetTooltip(tooltip);

				if (MouseInput.IsLeftClicked)
					takeAction();
			}
			else
			{
				UIRenderer.DisableTooltip(tooltip);
			}
		}

		protected virtual void takeAction()
		{
			action?.Invoke();
		}
	}
}
