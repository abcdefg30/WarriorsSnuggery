﻿using System;

namespace WarriorsSnuggery.Objects.Weapons
{
	class InstantHitWeapon : Weapon
	{
		readonly InstantHitProjectileType projectileType;

		public InstantHitWeapon(World world, WeaponType type, CPos origin, Target target, Actor originActor) : base(world, type, origin, target, originActor)
		{
			projectileType = (InstantHitProjectileType)type.Projectile;

			var diff = Position - TargetPosition;
			if (diff.FlatDist > type.MaxRange)
			{
				var angle = (Position - TargetPosition).FlatAngle;
				TargetPosition = Position + new CPos((int)(Math.Cos(angle) * type.MaxRange), (int)(Math.Sin(angle) * type.MaxRange), 0);
			}
		}

		public override void Tick()
		{
			Position = TargetPosition;
			if (Program.SharedRandom.NextDouble() < projectileType.HitChance)
				Detonate(Target);
		}
	}
}