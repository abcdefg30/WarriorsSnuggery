﻿using System;

namespace WarriorsSnuggery.Objects
{
	public class GameObject : IDisposable, ITickRenderable, ICheckVisible
	{
		protected readonly GraphicsObject Renderable;
		//TODO: move to actor. This class is now the main class for all ingame objects. UI things should not use this class.
		public PhysicsSector[] PhysicsSectors = new PhysicsSector[0];
		public readonly Physics Physics;
		public bool Disposed;

		public int Height
		{
			get { return height; }
			set
			{
				height = value;

				if (Renderable != null)
					Renderable.setPosition(GraphicPosition);

				if (Physics != null)
					Physics.Height = height;
			}
		}
		int height;

		public virtual CPos Position {
			get { return position; }
			set
			{
				position = value; 

				if (Renderable != null)
					Renderable.setPosition(GraphicPosition);

				if (Physics != null)
					Physics.Position = position;
			}
		}
		CPos position;

		public virtual CPos Offset {
			get { return offset; }
			set
			{
				offset = value; 

				if (Renderable != null)
					Renderable.setPosition(GraphicPosition);
			}
		}
		CPos offset;

		public virtual CPos GraphicPosition
		{
			get { return new CPos(position.X + offset.X, position.Y + offset.Y - height, position.Z + offset.Z + height); }
			private set { }
		}

		public virtual CPos GraphicPositionWithoutHeight
		{
			get { return new CPos(position.X + offset.X, position.Y + offset.Y, position.Z + offset.Z); }
			private set { }
		}

		public virtual CPos Rotation {
			get { return rotation; }
			set
			{
				rotation = value;

				if (Renderable != null) // int is degree
					Renderable.setRotation(rotation.ToAngle());
			}
		}
		CPos rotation;

		public virtual float Scale {
			get { return scale; }
			set
			{
				scale = value;

				if (Renderable != null)
					Renderable.setScale(scale);
			}
		}
		float scale = 1f;
		
		public GameObject(CPos pos)
		{
			Position = pos;
		}

		public GameObject(CPos pos, GraphicsObject renderable, Physics physics = null) // TODO remove physics
		{
			Physics = physics;
			Renderable = renderable;
			Position = pos;
		}

		public virtual void Tick()
		{
		}

		public virtual void Render()
		{
			if (Renderable != null)
				Renderable.Render();
			RenderPhysics();
		}

		protected virtual void RenderPhysics()
		{
			if (Settings.DeveloperMode && Physics != null)
			{
				Physics.RenderShape();
			}
		}

		public virtual void CheckVisibility()
		{
			if (Renderable != null)
				Renderable.CheckVisibility();
		}

		public override string ToString()
		{
			return string.Format("GameObject(Disposed={0} | Position={1} | Rotation={2} | Scale={3})", Disposed, position, rotation, scale);
		}

		public virtual void Dispose()
		{
			if (Physics != null)
				Physics.Dispose();
			foreach(var sector in PhysicsSectors)
				sector.Leave(this);

			Disposed = true;
		}
	}
}