﻿using System.Collections.Generic;
using WarriorsSnuggery.Objects;

namespace WarriorsSnuggery.Maps
{
	public sealed class TerrainGenerationType
	{
		public readonly int ID;

		public readonly GenerationType GenerationType;
		public readonly int Strength;
		public readonly float Scale;

		public readonly float Intensity;
		public readonly float Contrast;

		public readonly float EdgeNoise;

		public readonly int[] Terrain;
		public readonly bool SpawnPieces;
		public readonly int[] BorderTerrain;
		public readonly int Border;
		public readonly Dictionary<ActorType, int> SpawnActors;

		TerrainGenerationType(int id, GenerationType generationType, int strength, float scale, float intensity, float contrast, float edgeNoise, int[] terrain, bool spawnPieces, int[] borderTerrain, int border, Dictionary<ActorType, int> spawnActors)
		{
			ID = id;
			GenerationType = generationType;
			Strength = strength;
			Scale = scale;
			Intensity = intensity;
			Contrast = contrast;
			EdgeNoise = edgeNoise;
			Terrain = terrain;
			SpawnPieces = spawnPieces;
			BorderTerrain = borderTerrain;
			Border = border;
			SpawnActors = spawnActors;
		}

		public static TerrainGenerationType Empty()
		{
			return new TerrainGenerationType(0, GenerationType.NONE, 1, 1f, 1f, 1f, 0f, new[] { 0 }, true, new int[] { }, 0, new Dictionary<ActorType, int>());
		}

		public static TerrainGenerationType GetType(int id, MiniTextNode[] nodes)
		{
			var noise = GenerationType.NONE;
			var strength = 8;
			var scale = 2f;
			var intensity = 0f;
			var contrast = 1f;
			var edgeNoise = 0f;
			var terrainTypes = new int[0];
			var spawnPieces = true;
			var borderTerrain = new int[0];
			var border = 0;
			var spawnActorBlob = new Dictionary<ActorType, int>();

			foreach (var generation in nodes)
			{
				switch (generation.Key)
				{
					case "Type":
						noise = generation.Convert<GenerationType>();

						foreach (var noiseChild in generation.Children)
						{
							switch (noiseChild.Key)
							{
								case "Strength":
									strength = noiseChild.Convert<int>();
									break;
								case "Scale":
									scale = noiseChild.Convert<float>();
									break;
								case "Contrast":
									contrast = noiseChild.Convert<float>();
									break;
								case "Intensity":
									intensity = noiseChild.Convert<float>();
									break;
							}
						}
						break;
					case "Terrain":
						terrainTypes = generation.Convert<int[]>();

						break;
					case "Border":
						border = generation.Convert<int>();

						borderTerrain = generation.Children.Find(n => n.Key == "Terrain").Convert<int[]>();

						break;
					case "EdgeNoise":
						edgeNoise = generation.Convert<float>();

						break;
					case "SpawnPieces":
						spawnPieces = generation.Convert<bool>();

						break;
					case "SpawnActor":
						var type = ActorCreator.GetType(generation.Value);
						var probability = 50;

						probability = generation.Children.Find(n => n.Key == "Probability").Convert<int>();

						spawnActorBlob.Add(type, probability);
						break;
				}
			}
			return new TerrainGenerationType(id, noise, strength, scale, intensity, contrast, edgeNoise, terrainTypes, spawnPieces, borderTerrain, border, spawnActorBlob);
		}
	}
}