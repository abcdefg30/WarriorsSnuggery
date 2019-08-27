﻿using WarriorsSnuggery.Graphics;

namespace WarriorsSnuggery.Objects.Particles
{
	public class ParticleType
	{
		[Desc("Texture to use for the particle.", "The game will crash when both Texture and MeshSize are not defined or zero.")]
		public readonly TextureInfo Texture;
		[Desc("Color to use for Texture/Monocolored particle.")]
		public readonly Color Color = Color.White;
		[Desc("Size of the particle when using monocolored particles in pixels.", "The game will crash when both Texture and MeshSize are not defined or zero.", "Does not work if Texture is defined.")]
		public readonly int MeshSize;

		[Desc("Gravitational force.")]
		public readonly int Gravity = 2;

		[Desc("Random velocity given at start.")]
		public readonly CPos RandomVelocity = CPos.Zero;
		[Desc("Random rotational velocity given at start.")]
		public readonly CPos RandomRotation = CPos.Zero;
		[Desc("Random scale added to one, given at start.")]
		public readonly float RandomScale = 0f;

		[Desc("Time in which the particle is alive.")]
		public readonly int Duration;
		[Desc("Time in which the particle is dissolving.", "This time will be added on the time given in Duration.")]
		public readonly int DissolveDuration;

		[Desc("Show shadow of the particle.")]
		public readonly bool ShowShadow = false;
		[Desc("Regulates whether force can be applied on the particle from other objects.")]
		public readonly bool AffectedByObjects = false;

		public ParticleType(MiniTextNode[] nodes)
		{
			Loader.PartLoader.SetValues(this, nodes);
		}
	}
}
