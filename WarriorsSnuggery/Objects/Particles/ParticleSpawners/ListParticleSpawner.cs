﻿using System.Collections.Generic;

namespace WarriorsSnuggery.Objects.Particles
{
	[Desc("Contains a list of ParticleSpawners.")]
	public class ListParticleSpawner : ParticleSpawner
	{
		[Desc("List of ParticleSpawners.")]
		public readonly ParticleSpawner[] Spawners;

		public ListParticleSpawner(MiniTextNode[] nodes) : base()
		{
			Loader.PartLoader.SetValues(this, nodes);
		}

		public override Particle[] Create(World world, CPos position, int height)
		{
			var particleList = new List<Particle>();

			foreach(var spawner in Spawners)
				particleList.AddRange(spawner.Create(world, position, height));

			return particleList.ToArray();
		}
	}
}
