﻿using System;
using System.Collections.Generic;
using System.Linq;
using WarriorsSnuggery.Graphics;
using WarriorsSnuggery.Objects;
using WarriorsSnuggery.Objects.Particles;
using WarriorsSnuggery.Objects.Parts;

namespace WarriorsSnuggery
{
	public static class ActorCreator
	{
		public static void Load(string directory, string file)
		{
			var actors = RuleReader.Read(directory, file);

			foreach (var actor in actors)
			{
				var name = actor.Key;

				var partinfos = new List<PartInfo>();

				foreach (var child in actor.Children)
				{
					if (Loader.PartLoader.IsPart(child.Key))
					{
						var part = Loader.PartLoader.GetPart(child.Key, child.Children.ToArray());

						partinfos.Add(part);
						continue;
					}
					else
					{
						throw new YamlUnknownNodeException(child.Key, name);
					}
				}

				var physics = (PhysicsPartInfo)partinfos.Find(p => p is PhysicsPartInfo);
				var playable = (PlayablePartInfo)partinfos.Find(p => p is PlayablePartInfo);

				AddType(new ActorType(physics, playable, partinfos.ToArray()), name);
			}
		}

		static readonly Dictionary<string, ActorType> types = new Dictionary<string, ActorType>();

		public static void AddType(ActorType info, string name)
		{
			types.Add(name, info);
		}

		public static string GetName(ActorType type)
		{
			return types.FirstOrDefault(t => t.Value == type).Key;
		}

		public static string[] GetNames()
		{
			return types.Keys.ToArray();
		}

		public static ActorType GetType(string name)
		{
			if (!types.ContainsKey(name))
				throw new MissingInfoException(name);

			return types[name];
		}

		public static Actor Create(World world, string name, CPos position, byte team = 0, bool isBot = false, bool isPlayer = false, float health = 1f)
		{
			var type = GetType(name);
			return Create(world, type, position, team, isBot, isPlayer, health);
		}

		public static Actor Create(World world, ActorType type, CPos position, byte team = 0, bool isBot = false, bool isPlayer = false, float health = 1f)
		{
			var actor = new Actor(world, type, position, team, isBot, isPlayer);
			if (actor.Health != null)
			{
				actor.Health.HP = (int)(actor.Health.HP * health);
			}
			return actor;
		}
	}

	public static class WeaponCreator
	{
		public static void Load(string directory, string file)
		{
			var weapons = RuleReader.Read(directory, file);

			foreach (var weapon in weapons)
			{
				var name = weapon.Key;

				AddTypes(new WeaponType(name, weapon.Children.ToArray()), name);
			}
		}

		static readonly Dictionary<string, WeaponType> types = new Dictionary<string, WeaponType>();

		public static string[] GetNames()
		{
			return types.Keys.ToArray();
		}

		public static string GetName(WeaponType type)
		{
			return types.FirstOrDefault(t => t.Value == type).Key;
		}

		public static void AddTypes(WeaponType info, string name)
		{
			types.Add(name, info);
		}

		public static WeaponType GetType(string name)
		{
			if (!types.ContainsKey(name))
				throw new MissingInfoException(name);

			return types[name];
		}

		public static Weapon Create(World world, string name, CPos position, CPos target)
		{
			var type = GetType(name);
			return Create(world, type, position, target);
		}

		public static Weapon Create(World world, WeaponType type, CPos origin, CPos target)
		{
			switch (type.WeaponFireType)
			{
				case WeaponFireType.ROCKET:
					return new RocketWeapon(world, type, origin, target);

				case WeaponFireType.DIRECTEDBEAM:
				case WeaponFireType.BEAM:
					return new BeamWeapon(world, type, origin, target);
				default:
					return new BulletWeapon(world, type, origin, target);
			}
		}

		public static Weapon Create(World world, WeaponType type, Actor origin, CPos target)
		{
			switch (type.WeaponFireType)
			{
				case WeaponFireType.ROCKET:
					return new RocketWeapon(world, type, origin, target);

				case WeaponFireType.DIRECTEDBEAM:
				case WeaponFireType.BEAM:
					return new BeamWeapon(world, type, origin, target);
				default:
					return new BulletWeapon(world, type, origin, target);
			}
		}
	}

	public static class ParticleCreator
	{
		public static void Load(string directory, string file)
		{
			var nodes = RuleReader.Read(directory, file);

			foreach (var node in nodes)
			{
				AddType(new ParticleType(node.Children.ToArray()), node.Key);
			}
		}

		static readonly Dictionary<string, ParticleType> types = new Dictionary<string, ParticleType>();

		public static string[] GetNames()
		{
			return types.Keys.ToArray();
		}

		public static string GetName(ParticleType type)
		{
			return types.FirstOrDefault(t => t.Value == type).Key;
		}

		public static void AddType(ParticleType info, string name)
		{
			types.Add(name, info);
		}

		public static ParticleType GetType(string name)
		{
			if (!types.ContainsKey(name))
				throw new MissingInfoException(name);

			return types[name];
		}

		public static Particle Create(string name, CPos position, int height, Random random)
		{
			return Create(GetType(name), position, height, random);
		}

		public static Particle Create(ParticleType type, CPos position, int height, Random random)
		{
			return new Particle(position, height, type, random);
		}
	}

	public static class TerrainCreator
	{
		public static void LoadTypes(string directory, string file)
		{
			var terrains = RuleReader.Read(directory, file);

			foreach (var terrain in terrains)
			{
				var id = ushort.Parse(terrain.Key);

				var image = string.Empty;
				var speedModifier = 1f;
				var overlapHeight = -1;
				var spawnSmudge = true;
				var edge_Image = "";
				var edge_Image2 = "";
				var corner_Image = "";

				foreach (var child in terrain.Children)
				{
					switch (child.Key)
					{
						case "Sprite":
							image = child.Convert<string>();
							break;
						case "Speed":
							speedModifier = child.Convert<float>();
							break;
						case "OverlapHeight":
							overlapHeight = child.Convert<int>();
							break;
						case "EdgeSprite":
							edge_Image = child.Convert<string>();
							break;
						case "VerticalEdgeSprite":
							edge_Image2 = child.Convert<string>();
							break;
						case "CornerSprite":
							corner_Image = child.Convert<string>();
							break;
						case "SpawnSmudge":
							spawnSmudge = child.Convert<bool>();
							break;
						default:
							throw new YamlUnknownNodeException(child.Key, "Terrain " + id);
					}
				}

				if (image == string.Empty)
					throw new YamlMissingNodeException(terrain.Key, "Image");

				AddType(new TerrainType(id, image, speedModifier, edge_Image != "", spawnSmudge, overlapHeight, edge_Image, corner_Image, edge_Image2));
			}
		}

		static readonly Dictionary<int, TerrainType> types = new Dictionary<int, TerrainType>();

		public static int[] GetIDs()
		{
			return types.Keys.ToArray();
		}

		public static int GetID(TerrainType type)
		{
			return types.FirstOrDefault(t => t.Value == type).Key;
		}

		public static void AddType(TerrainType type)
		{
			types.Add(type.ID, type);
		}

		public static TerrainType GetType(int ID)
		{
			if (!types.ContainsKey(ID))
				throw new MissingInfoException(ID.ToString());

			return types[ID];
		}

		public static Terrain Create(World world, WPos position, int ID)
		{
			return new Terrain(world, position, GetType(ID));
		}
	}

	public static class WallCreator
	{
		public static void Load(string directory, string file)
		{
			var walls = RuleReader.Read(directory, file);

			foreach (var wall in walls)
			{
				string texture = string.Empty;
				bool blocks = true;
				bool destroyable = false;
				int height = 512;

				foreach (var child in wall.Children)
				{
					switch (child.Key)
					{
						case "Image":
							texture = child.Convert<string>();

							break;
						case "Blocks":
							blocks = child.Convert<bool>();

							break;
						case "Destroyable":
							destroyable = child.Convert<bool>();

							break;
						case "Height":
							height = child.Convert<int>();

							break;
					}
				}

				if (texture == string.Empty)
					throw new YamlMissingNodeException(wall.Key, "Image");

				var id = int.Parse(wall.Key);

				AddType(new WallType(id, texture, blocks, destroyable, height));
			}
		}

		static readonly Dictionary<int, WallType> types = new Dictionary<int, WallType>();

		public static int[] GetIDs()
		{
			return types.Keys.ToArray();
		}

		public static int GetID(WallType type)
		{
			return types.FirstOrDefault(t => t.Value == type).Key;
		}

		public static void AddType(WallType type)
		{
			types.Add(type.ID, type);
		}

		public static WallType GetType(int ID)
		{
			if (!types.ContainsKey(ID))
				throw new MissingInfoException(ID.ToString());

			return types[ID];
		}

		public static Wall Create(WPos position, int ID)
		{
			return new Wall(position, GetType(ID));
		}
	}
}
