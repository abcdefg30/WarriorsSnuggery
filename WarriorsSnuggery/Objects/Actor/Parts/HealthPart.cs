﻿namespace WarriorsSnuggery.Objects.Parts
{
	[Desc("Attach this to an actor to make it vulnerable and to have health.")]
	public class HealthPartInfo : PartInfo
	{
		[Desc("Maximal Health to archive.")]
		public readonly int MaxHealth;
		[Desc("Health when the actor is spawned.")]
		public readonly int StartHealth;

		public override ActorPart Create(Actor self)
		{
			return new HealthPart(self, this);
		}

		public HealthPartInfo(string internalName, MiniTextNode[] nodes) : base(internalName, nodes)
		{
			if (StartHealth <= 0 || StartHealth > MaxHealth)
				StartHealth = MaxHealth;
		}
	}

	public class HealthPart : ActorPart
	{
		readonly HealthPartInfo info;

		public int MaxHP
		{
			get { return info.MaxHealth; }
		}
		public int StartHealth
		{
			get { return info.StartHealth; }
		}

		public float RelativeHP
		{
			get
			{
				return health / (float)MaxHP;
			}
			set
			{
				health = (int)(value * MaxHP);
			}
		}

		public int HP
		{
			get
			{
				return health;
			}
			set
			{
				health = value;
				if (health > MaxHP)
					health = MaxHP;
				if (health <= 0)
					health = 0;
			}
		}
		int health;

		public HealthPart(Actor self, HealthPartInfo info) : base(self)
		{
			this.info = info;

			HP = StartHealth;
		}

		public override void Tick()
		{
			var terrain = self.World.TerrainAt(self.TerrainPosition);
			if (terrain != null && terrain.Type.Damage != 0 && self.World.Game.LocalTick % 2 == 0)
				HP -= terrain.Type.Damage;
		}
	}
}