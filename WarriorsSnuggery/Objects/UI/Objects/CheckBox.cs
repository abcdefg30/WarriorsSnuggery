using System;

namespace WarriorsSnuggery.UI
{
	public class CheckBox : IPositionable, ITickRenderable
	{
		public virtual CPos Position
		{
			get { return position; }
			set
			{
				position = value;
				type.Click.SetPosition(position);
				type.Default.SetPosition(position);
				type.Checked.SetPosition(position);
			}
		}
		CPos position;

		public virtual VAngle Rotation
		{
			get { return rotation; }
			set
			{
				rotation = value;
				type.Click.SetRotation(rotation);
				type.Default.SetRotation(rotation);
				type.Checked.SetRotation(rotation);
			}
		}
		VAngle rotation;

		public virtual float Scale
		{
			get { return scale; }
			set
			{
				scale = value;
				type.Click.SetScale(scale);
				type.Default.SetScale(scale);
				type.Checked.SetScale(scale);
			}
		}
		float scale = 1f;

		public bool Checked;

		readonly CheckBoxType type;
		readonly Action<bool> action;

		bool mouseOnBox;

		public CheckBox(CPos pos, bool @checked, CheckBoxType type, Action<bool> onTicked)
		{
			Checked = @checked;
			this.type = type;
			action = onTicked;
			Position = pos;
		}

		public void Render()
		{
			if (mouseOnBox && MouseInput.IsLeftDown)
			{
				type.Click.SetPosition(Position);
				type.Click.PushToBatchRenderer();
			}
			else
			{
				if (!Checked)
				{
					type.Default.SetPosition(Position);
					type.Default.PushToBatchRenderer();
				}
				else
				{
					type.Checked.SetPosition(Position);
					type.Checked.PushToBatchRenderer();
				}
			}
		}

		public void Tick()
		{
			checkMouse();
		}

		void checkMouse()
		{
			var mousePosition = MouseInput.WindowPosition;

			mouseOnBox = mousePosition.X > Position.X - type.Width && mousePosition.X < Position.X + type.Width && mousePosition.Y > Position.Y - type.Height && mousePosition.Y < Position.Y + type.Height;

			if (MouseInput.IsLeftClicked && mouseOnBox)
			{
				Checked = !Checked;
				action?.Invoke(Checked);
			}
		}
	}
}
