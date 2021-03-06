using System;
using WarriorsSnuggery.Graphics;

namespace WarriorsSnuggery.Objects.Particles
{
	public class Particle : PhysicsObject
	{
		public readonly bool AffectedByObjects;
		public readonly ParticleType Type;
		readonly Random random;

		int current;
		int dissolve;

		// Z is height
		CPos transform_velocity;
		VAngle rotate_velocity;

		public Particle(CPos pos, int height, ParticleType type, Random random) : base(pos, type.Texture != null ? (BatchRenderable)new BatchSequence(type.Texture.GetTextures(), type.Color + new Color(variety(type.ColorVariety.R), variety(type.ColorVariety.G), variety(type.ColorVariety.B), variety(type.ColorVariety.A)), tick: type.Texture.Tick) : new BatchObject(type.MeshSize * MasterRenderer.PixelMultiplier + variety(type.MeshSizeVariety), type.Color + new Color(variety(type.ColorVariety.R), variety(type.ColorVariety.G), variety(type.ColorVariety.B), variety(type.ColorVariety.A))))
		{
			Height = height;
			Type = type;
			this.random = random;

			AffectedByObjects = type.AffectedByObjects;
			current = type.Duration;
			dissolve = type.DissolveDuration;

			Renderable.SetColor(type.Color);

			transform_velocity = new CPos(random.Next(-type.RandomVelocity.X, type.RandomVelocity.X), random.Next(-type.RandomVelocity.Y, type.RandomVelocity.Y), random.Next(-type.RandomVelocity.Z, type.RandomVelocity.Z));
			rotate_velocity = new VAngle(random.Next(-type.RandomRotation.X, type.RandomRotation.X), random.Next(-type.RandomRotation.Y, type.RandomRotation.Y), random.Next(-type.RandomRotation.Z, type.RandomRotation.Z));
		}

		static float variety(float variation)
		{
			return ((float)Program.SharedRandom.NextDouble() - 0.5f) * variation;
		}

		float xLeft;
		float yLeft;
		float zLeft;

		public void AffectVelocity(ParticleForce force, float ratio, CPos origin)
		{
			var useZ = force.UseHeight;

			var angle = (Position - origin).FlatAngle;
			var xFloat = 0f;
			var yFloat = 0f;
			var zFloat = 0f;

			switch (force.Type)
			{
				case ParticleForceType.FORCE:
					xFloat = (float)(force.Strength * Math.Cos(angle)) * ratio;
					yFloat = (float)(force.Strength * Math.Sin(angle)) * ratio;
					if (useZ)
						zFloat = force.Strength * ratio;
					break;
				case ParticleForceType.TURBULENCE:
					angle = (float)(random.NextDouble() * 2 * Math.PI);
					xFloat = (float)(force.Strength * Math.Cos(angle)) * ratio;
					yFloat = (float)(force.Strength * Math.Sin(angle)) * ratio;
					if (useZ)
						zFloat = ((float)random.NextDouble() - 0.5f) * force.Strength * ratio;
					break;
				case ParticleForceType.DRAG:
					xFloat = -force.Strength * ratio * transform_velocity.X / 256;
					if (Math.Abs(xFloat) > Math.Abs(transform_velocity.X))
						xFloat = -transform_velocity.X * ratio;

					yFloat = -force.Strength * ratio * transform_velocity.Y / 256;
					if (Math.Abs(yFloat) > Math.Abs(transform_velocity.Y))
						yFloat = -transform_velocity.Y * ratio;

					if (useZ)
					{
						zFloat = -force.Strength * ratio * transform_velocity.Z / 256;
						if (Math.Abs(zFloat) > Math.Abs(transform_velocity.Z))
							zFloat = -transform_velocity.Z * ratio;
					}
					break;
				case ParticleForceType.VORTEX:
					angle -= (float)Math.PI / 2;
					xFloat = (float)(force.Strength * Math.Cos(angle)) * ratio;
					yFloat = (float)(force.Strength * Math.Sin(angle)) * ratio;
					zFloat = 0; // Vortex is only 2D
					break;
			}
			var x = (int)Math.Round(xFloat + xLeft);
			var y = (int)Math.Round(yFloat + yLeft);
			var z = (int)Math.Round(zFloat + zLeft);
			xLeft = (xFloat + xLeft) - x;
			yLeft = (yFloat + yLeft) - y;
			zLeft = (zFloat + zLeft) - z;
			transform_velocity += new CPos(x, y, z);
		}

		public void AffectRotation(ParticleForce force, float ratio, CPos origin)
		{
			var angle = (origin - Position).FlatAngle - 2 * (float)Math.PI + Rotation.CastToAngleRange().Z;

			if (angle < -Math.PI)
				angle += 2 * (float)Math.PI;

			if (angle > Math.PI)
				angle -= 2 * (float)Math.PI;

			var zFloat = 0f;

			switch (force.Type)
			{
				case ParticleForceType.FORCE:
					zFloat = Math.Sign(-angle) * force.Strength * ratio * 0.1f;
					zFloat = Math.Min(0.628f, zFloat);
					break;
				case ParticleForceType.TURBULENCE:
					angle = (float)(random.NextDouble() * 2 * Math.PI);
					zFloat = Math.Sign(-angle) * force.Strength * ratio * 0.1f;
					zFloat = Math.Min(0.628f, zFloat);
					break;
				case ParticleForceType.DRAG:
					zFloat = -force.Strength * ratio * rotate_velocity.Z * 0.1f;
					if (Math.Abs(zFloat) > Math.Abs(rotate_velocity.Z))
						zFloat = -rotate_velocity.Z * ratio;
					break;
				case ParticleForceType.VORTEX:
					zFloat = Math.Sign(-angle - (float)Math.PI / 2) * force.Strength * 0.1f;
					zFloat = Math.Min(0.628f, zFloat);
					break;
			}

			rotate_velocity = new VAngle(0, 0, zFloat);
		}

		public override void Tick()
		{
			base.Tick();

			transform_velocity += new CPos(0, 0, -Type.Gravity);
			Rotation += rotate_velocity;

			Position += new CPos(transform_velocity.X, transform_velocity.Y, 0);
			
			if (Height + transform_velocity.Z < 0)
				Height = 0;
			else
				Height += transform_velocity.Z;

			if (current-- <= 0)
			{
				if (dissolve-- <= 0)
				{
					Dispose();
					return;
				}
				else
				{
					Renderable.SetColor(new Color(Type.Color.R, Type.Color.G, Type.Color.B, Type.Color.A * dissolve / Type.DissolveDuration));
				}
			}
		}

		public override void Render()
		{
			if (Type.ShowShadow)
				RenderShadow();

			base.Render();
		}
	}
}
