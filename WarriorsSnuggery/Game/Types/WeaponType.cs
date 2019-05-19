﻿using WarriorsSnuggery.Graphics;
using WarriorsSnuggery.Objects.Parts;

namespace WarriorsSnuggery.Objects
{
	public enum FalloffType
	{
		QUADRATIC,
		CUBIC,
		EXPONENTIAL,
		LINEAR,
		ROOT
	}

	public enum WeaponFireType
	{
		BULLET,
		ROCKET,
		BEAM,
	}

	public class WeaponType
	{
		[Desc("Texture of the Weapon.")]
		public readonly TextureInfo Textures;
		[Desc("Texture of the Smudge that will be left behind from impact.")]
		public readonly TextureInfo Smudge;

		[Desc("Highest damage value possible.")]
		public readonly int Damage;

		[Desc("Speed of the warhead.")]
		public readonly int Speed;
		[Desc("Acceleration of the warhead.")]
		public readonly int Acceleration;

		[Desc("Time until the weapon has been reloaded.")]
		public readonly int Reload;

		[Desc("Particles when the weapon impacts.")]
		public readonly ParticleType ParticlesWhenExplode; //TODO replace through particleSpawner
		[Desc("Count of the particles.")]
		public readonly int ParticleCountWhenExplode;

		[Desc("Inaccuracy of the weapon.")]
		public readonly int Inaccuracy;

		[Desc("Maximal Range the weapon can travel.")]
		public readonly int MaxRange;
		[Desc("Minimal Range the player can target.")]
		public readonly int MinRange;

		[Desc("Type of weapon.", "Possible: BULLET, ROCKET, BEAM")]
		public readonly WeaponFireType WeaponFireType;
		[Desc("Falloff of the impact.", "Possible: QUADRATIC, CUBIC, EXPONENTIAL, LINEAR, ROOT")]
		public readonly FalloffType DamageFalloff;

		[Desc("Weapon always points to the target.")]
		public readonly bool OrientateToTarget;

		[Desc("Collision shape of the weapon.", "Possible: CIRCLE, RECTANGLE, LINE_HORIZONTAL, LINE_VERTICAL, NONE")]
		public readonly Shape PhysicalShape;
		[Desc("Size of the collision boundary.")]
		public readonly int PhysicalSize;

		public WeaponType(TextureInfo textures, TextureInfo smudge, int damage, int speed, int acceleration, int reload, ParticleType particlesWhenExplode, int particleCountWhenExplode, int inaccuracy, int maxRange, int minRange, FalloffType damageFalloff, WeaponFireType weaponFireType, bool orientateToTarget, Shape physicalShape, int physicalSize)
		{
			Textures = textures;
			Smudge = smudge;
			Damage = damage;
			Speed = speed;
			Acceleration = acceleration;
			Reload = reload;
			ParticlesWhenExplode = particlesWhenExplode;
			ParticleCountWhenExplode = particleCountWhenExplode;
			Inaccuracy = inaccuracy;
			MaxRange = maxRange;
			MinRange = minRange;
			WeaponFireType = weaponFireType;
			DamageFalloff = damageFalloff;
			OrientateToTarget = orientateToTarget;
			PhysicalShape = physicalShape;
			PhysicalSize = physicalSize;
		}
	}
}
