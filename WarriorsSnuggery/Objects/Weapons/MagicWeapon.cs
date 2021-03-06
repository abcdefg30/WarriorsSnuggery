﻿using System;

namespace WarriorsSnuggery.Objects.Weapons
{
	class MagicWeapon : Weapon
	{
		readonly MagicProjectileType projectileType;

		Vector speed;
		float angle;

		public MagicWeapon(World world, WeaponType type, Target target, Actor origin) : base(world, type, target, origin)
		{
			projectileType = (MagicProjectileType)type.Projectile;

			TargetPosition += getInaccuracy();

			angle = (Position - TargetPosition).FlatAngle;
			calculateSpeed(angle);

			if ((Position - TargetPosition).Dist > type.MaxRange * RangeModifier)
				TargetPosition = Position + new CPos((int)(Math.Cos(angle) * type.MaxRange * RangeModifier), (int)(Math.Sin(angle) * type.MaxRange * RangeModifier), 0) + getInaccuracy();

		}

		public override void Tick()
		{
			base.Tick();

			if (projectileType.OrientateToTarget)
				Rotation = new VAngle(0, 0, -(TargetPosition - GraphicPosition).FlatAngle);

			if (projectileType.FollowTarget)
			{
				TargetPosition = Target.Position;
				TargetHeight = Target.Height;
				calculateAngle();
				calculateSpeed(angle);
			}

			Move();

			if (projectileType.TrailParticles != null)
				World.Add(projectileType.TrailParticles.Create(World, Position, Height));
		}

		void calculateAngle()
		{
			if (projectileType.Turbulence != 0)
				calculateTurbulence();

			var diff = (Position - TargetPosition).FlatAngle - angle;

			if (Math.Abs(diff) > projectileType.FloatTurnSpeed)
				diff = Math.Sign(diff) * projectileType.FloatTurnSpeed;

			angle += diff;
		}

		void calculateTurbulence()
		{
			var dist = (Position - TargetPosition).FlatDist;

			angle += (float)(Program.SharedRandom.NextDouble() - 0.5f) * projectileType.Turbulence * dist / (Type.MaxRange * 1024f);
		}

		void calculateSpeed(float angle)
		{
			var x = (float)Math.Cos(angle) * projectileType.Speed;
			var y = (float)Math.Sin(angle) * projectileType.Speed;

			var zDiff = TargetHeight - Height;
			var dDiff = (int)(Position - TargetPosition).FlatDist;

			var angle2 = new CPos(-dDiff, -zDiff, 0).FlatAngle;
			var z = (float)Math.Sin(angle2) * projectileType.Speed;

			speed = new Vector(x, y, z);
		}

		public void Move()
		{
			var curSpeed = new CPos((int)speed.X, (int)speed.Y, (int)speed.Z);
			Position = new CPos(Position.X + curSpeed.X, Position.Y + curSpeed.Y, Position.Z);
			Physics.Position = Position;
			Height += curSpeed.Z;
			Physics.Height = Height;

			if (Height < 0 || !World.IsInWorld(Position))
				Detonate(new Target(Position, 0));

			World.PhysicsLayer.UpdateSectors(this, updateSectors: false);

			if (World.CheckCollision(this, false, new[] { Origin }))
				Detonate(new Target(Position, Height));
		}

		CPos getInaccuracy()
		{
			if (projectileType.Inaccuracy > 0)
			{
				var ranX = (Program.SharedRandom.Next(projectileType.Inaccuracy) - projectileType.Inaccuracy / 2) * InaccuracyModifier;
				var ranY = (Program.SharedRandom.Next(projectileType.Inaccuracy) - projectileType.Inaccuracy / 2) * InaccuracyModifier;

				return new CPos((int)ranX, (int)ranY, 0);
			}

			return CPos.Zero;
		}
	}
}
