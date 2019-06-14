﻿using System;

namespace WarriorsSnuggery.Objects
{
	public enum Shape
	{
		CIRCLE,
		RECTANGLE,
		LINE_HORIZONTAL,
		LINE_VERTICAL,
		NONE
	}

	public sealed class Physics : IDisposable
	{
		public readonly Shape Shape;
		public readonly int RadiusX;
		public readonly int RadiusY;// only for box
		public readonly int HeightRadius;

		readonly PhysicsObject renderable;

		public CPos Position;
		public int Height;

		public Physics(CPos position, int height, Shape shape, int radiusX, int radiusY, int heightradius)
		{
			Position = position;
			Height = height;
			Shape = shape;
			RadiusX = radiusX;
			RadiusY = radiusY;
			HeightRadius = heightradius;

			if (!Settings.DeveloperMode)
				return;

			switch (Shape)
			{
				case Shape.CIRCLE:
					renderable = new ColoredCircle(Position, Color.Cyan, RadiusX * 2 / 1024f, 16, isFilled: false);
					break;
				case Shape.RECTANGLE:
					renderable = new ColoredRect(Position, Color.Cyan, RadiusX * 2 / 1024f, RadiusY * 2 / 1024f, isFilled: false);
					break;
				case Shape.LINE_HORIZONTAL:
					var debugLine = new ColoredLine(Position - new CPos(0, RadiusX, -10240), Color.Cyan, RadiusX * 2 / 1024f)
					{
						Rotation = new VAngle(0, 0, 90)
					};
					renderable = debugLine;
					break;
				case Shape.LINE_VERTICAL:
					renderable = new ColoredLine(Position - new CPos(RadiusX, 0, 0), Color.Cyan, RadiusX * 2 / 1024f);
					break;
			}
			if (renderable != null)
				WorldRenderer.RenderAfter(renderable);
		}

		public bool Intersects(Physics other, bool ignoreHeight)
		{
			if (other == null)
				return false;

			if (Shape == Shape.NONE || other.Shape == Shape.NONE)
				return false;

			if (!ignoreHeight && Math.Abs(other.Height - Height) >= other.HeightRadius + HeightRadius)
				return false;

			if (other.Shape != Shape.LINE_HORIZONTAL && Shape != Shape.LINE_HORIZONTAL && other.Shape != Shape.LINE_VERTICAL && Shape != Shape.LINE_VERTICAL)
			{
				if (Math.Abs(other.Position.X - Position.X) >= other.RadiusX + RadiusX)
					return false;

				if (Math.Abs(other.Position.Y - Position.Y) >= other.RadiusX + RadiusX)
					return false;
			}

			if (Shape == Shape.CIRCLE && other.Shape == Shape.CIRCLE)
			{
				return Position.DistToXY(other.Position) <= RadiusX + other.RadiusX;
			}
			if (Shape == Shape.RECTANGLE && other.Shape == Shape.RECTANGLE)
			{

				var diff = other.Position - Position;
				var scaleX = RadiusX + other.RadiusX;
				var scaleY = RadiusY + other.RadiusY;
				return Math.Abs(diff.X) < scaleX && Math.Abs(diff.Y) < scaleY;
			}

			// CIRCLE <-> BOX
			if (Shape == Shape.CIRCLE && other.Shape == Shape.RECTANGLE || Shape == Shape.RECTANGLE && other.Shape == Shape.CIRCLE)
			{
				var circle = Shape == Shape.CIRCLE ? this : other;
				var box = circle == this ? other : this;
				var pos = circle.Position - box.Position;
				pos = new CPos(Math.Abs(pos.X),Math.Abs(pos.Y),Math.Abs(pos.Z));

				if (pos.X > (box.RadiusX + circle.RadiusX)) return false;
				if (pos.Y > (box.RadiusY + circle.RadiusY)) return false;

				if (pos.X <= box.RadiusX) return true;
				if (pos.Y <= box.RadiusY) return true;

				// Pythagorean theorem: We calculate X and Y in order to get the circle distance; Added error margin "box.Radius/8".
				var corner = (pos.X - box.RadiusX) ^ 2 + (pos.Y - box.RadiusY) ^ 2 - box.RadiusX/8;

				return corner <= (circle.RadiusX^2);
			}

			// CIRCLE <-> LINE
			if (Shape == Shape.CIRCLE && other.Shape == Shape.LINE_VERTICAL || Shape == Shape.LINE_VERTICAL && other.Shape == Shape.CIRCLE)
			{
				var circle = Shape == Shape.CIRCLE ? this : other;
				var line = circle == this ? other : this;

				if (Math.Abs(circle.Position.Y - line.Position.Y + 512) > circle.RadiusX + line.RadiusX) return false;

				return Math.Abs(circle.Position.X - line.Position.X + 512) <= circle.RadiusX;
			}

			if (Shape == Shape.CIRCLE && other.Shape == Shape.LINE_HORIZONTAL || Shape == Shape.LINE_HORIZONTAL && other.Shape == Shape.CIRCLE)
			{
				var circle = Shape == Shape.CIRCLE ? this : other;
				var line = circle == this ? other : this;

				if (Math.Abs(circle.Position.X - line.Position.X + 512) > circle.RadiusX + line.RadiusX) return false;

				return Math.Abs(circle.Position.Y - line.Position.Y + 512) <= circle.RadiusX;
			}

			// BOX <-> LINE
			if (Shape == Shape.RECTANGLE && other.Shape == Shape.LINE_HORIZONTAL || Shape == Shape.LINE_HORIZONTAL && other.Shape == Shape.RECTANGLE)
			{
				var box = Shape == Shape.RECTANGLE ? this : other;
				var line = box == this ? other : this;

				if (Math.Abs(box.Position.Y - line.Position.Y + 512) > line.RadiusX) return false;
				if (Math.Abs(box.Position.X - line.Position.X + 512) > (box.RadiusY + line.RadiusX)) return false;

				return true;
			}
			if (Shape == Shape.RECTANGLE && other.Shape == Shape.LINE_VERTICAL || Shape == Shape.LINE_VERTICAL && other.Shape == Shape.RECTANGLE)
			{
				var box = Shape == Shape.RECTANGLE ? this : other;
				var line = box == this ? other : this;

				if (Math.Abs(box.Position.X - line.Position.X + 512) > line.RadiusX) return false;
				if (Math.Abs(box.Position.Y - line.Position.Y + 512) > (box.RadiusY + line.RadiusX)) return false;

				return true;
			}

			return false;
		}

		public void RenderShape()
		{
			if (renderable == null)
				return;

			switch (Shape)
			{
				default:
					renderable.Position = Position;
					break;
				case Shape.LINE_HORIZONTAL:
					renderable.Position = Position - new CPos(0,RadiusX,0);
					break;
				case Shape.LINE_VERTICAL:
					renderable.Position = Position - new CPos(RadiusX,0,0);
					break;
			}
		}

		public void Dispose()
		{
			if (renderable != null)
			{
				WorldRenderer.RemoveRenderAfter(renderable);
				renderable.Dispose();
			}
		}
	}
}
