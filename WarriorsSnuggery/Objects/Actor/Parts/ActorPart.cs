﻿using WarriorsSnuggery.Objects.Weapons;

namespace WarriorsSnuggery.Objects.Parts
{
	public abstract class PartInfo
	{
		public readonly string InternalName;

		public abstract ActorPart Create(Actor self);

		protected PartInfo(string internalName, MiniTextNode[] nodes)
		{
			InternalName = internalName;
			Loader.PartLoader.SetValues(this, nodes);
		}
	}

	public abstract class ActorPart
	{
		protected readonly Actor self;

		protected ActorPart(Actor self)
		{
			this.self = self;
		}

		public virtual void Tick()
		{

		}

		public virtual void Render()
		{

		}

		public virtual void OnAttack(CPos target, Weapon weapon)
		{

		}

		public virtual void OnKill(Actor killed)
		{

		}

		public virtual void OnDamage(Actor damager, int damage)
		{

		}

		public virtual void OnKilled(Actor killer)
		{

		}

		public virtual void OnMove(CPos old, CPos speed)
		{

		}

		public virtual void OnStop()
		{

		}

		public virtual void OnAccelerate(CPos acceleration)
		{

		}

		public virtual void OnAccelerate(float angle, int acceleration)
		{

		}

		public virtual void OnDispose()
		{

		}
	}
}
