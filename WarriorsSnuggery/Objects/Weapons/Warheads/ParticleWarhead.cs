﻿using WarriorsSnuggery.Objects.Particles;

namespace WarriorsSnuggery.Objects.Weapons
{
	public class ParticleWarhead : IWarhead
	{
		[Desc("Particlespawner that will be used to spawn the particles.")]
		public readonly ParticleSpawner Spawner;

		public ParticleWarhead(MiniTextNode[] nodes)
		{
			Loader.PartLoader.SetValues(this, nodes);
		}

		public void Impact(World world, Weapon weapon, Target target)
		{
			world.Add(Spawner.Create(world, target.Position, target.Height));
		}
	}
}
