﻿using System;
using System.Linq;

namespace WarriorsSnuggery.Objects.Parts
{
	[Desc("Attach this to an actor to activate mobility features.")]
	public class MobilityPartInfo : PartInfo
	{
		[Desc("Speed of the Actor.")]
		public readonly int Speed;
		[Desc("Acceleration of the Actor. If 0, Speed is used.")]
		public readonly int Acceleration;
		[Desc("Deceleration of the Actor. If 0, Speed is used.")]
		public readonly int Deceleration;
		[Desc("Acceleration to use for the vertical axis.")]
		public readonly int HeightAcceleration;
		[Desc("Actor may also control velocity while in air.")]
		public readonly bool CanFly;
		[Desc("Gravity to apply while flying.", "Gravity will not be applied when the actor can fly.")]
		public readonly CPos Gravity = new CPos(0, 0, -9);
		[Desc("Sound to be played while moving.")]
		public readonly SoundType Sound;

		public override ActorPart Create(Actor self)
		{
			return new MobilityPart(self, this);
		}

		public MobilityPartInfo(string internalName, MiniTextNode[] nodes) : base(internalName, nodes)
		{
			if (Acceleration == 0)
				Acceleration = Speed;
			if (Deceleration == 0)
				Deceleration = Speed;
		}
	}

	public class MobilityPart : ActorPart
	{
		readonly MobilityPartInfo info;

		public CPos Force;
		public CPos Velocity;
		public CPos oldVelocity;
		public Sound sound;

		public bool CanFly
		{
			get { return info.CanFly; }
		}

		public MobilityPart(Actor self, MobilityPartInfo info) : base(self)
		{
			this.info = info;
			if (info.Sound != null)
				sound = new Sound(info.Sound);
		}

		public override void Tick()
		{
			if (Velocity != CPos.Zero)
			{
				if (self.World.Game.Type == GameType.EDITOR)
					return;

				if (self.Height == 0 || CanFly)
				{
					var signX = Math.Sign(Velocity.X);
					var signY = Math.Sign(Velocity.Y);
					var signZ = Math.Sign(Velocity.Z);
					Velocity -= new CPos(info.Deceleration * signX, info.Deceleration * signY, info.Deceleration * signZ);

					if (Math.Sign(Velocity.X) != signX)
						Velocity = new CPos(0, Velocity.Y, Velocity.Z);
					if (Math.Sign(Velocity.Y) != signY)
						Velocity = new CPos(Velocity.X, 0, Velocity.Z);
					if (Math.Sign(Velocity.Z) != signZ)
						Velocity = new CPos(Velocity.X, Velocity.Y, 0);
				}

				if (oldVelocity == CPos.Zero)
					sound?.Play(self.Position, true);
			}
			else if (oldVelocity == CPos.Zero)
				sound?.Stop();
			oldVelocity = Velocity;

			if (self.Height > 0 && !CanFly)
				Force += info.Gravity;

			Velocity += Force;
			Force = CPos.Zero;

			var speedFactor = 1f;
			foreach (var effect in self.Effects.Where(e => e.Active && e.Spell.Type == Spells.EffectType.SPEED))
				speedFactor *= effect.Spell.Value;

			var maxSpeed = speedFactor * info.Speed;
			if (Math.Abs(Velocity.X) > maxSpeed)
				Velocity = new CPos((int)maxSpeed * Math.Sign(Velocity.X), Velocity.Y, Velocity.Z);
			if (Math.Abs(Velocity.Y) > maxSpeed)
				Velocity = new CPos(Velocity.X, (int)maxSpeed * Math.Sign(Velocity.Y), Velocity.Z);
			if (Math.Abs(Velocity.Z) > maxSpeed)
				Velocity = new CPos(Velocity.X, Velocity.Y, (int)maxSpeed * Math.Sign(Velocity.Z));

			sound?.SetPosition(self.Position);
		}

		public new void OnAccelerate(CPos acceleration)
		{
			Force += acceleration;
		}

		public new int OnAccelerate(float angle, int customAcceleration)
		{
			var acceleration = customAcceleration == 0 ? info.Acceleration * 2 : customAcceleration;
			var x = (int)Math.Ceiling(Math.Cos(angle) * acceleration);
			var y = (int)Math.Ceiling(Math.Sin(angle) * acceleration);

			Force += new CPos(x, y, 0);

			return acceleration;
		}

		public int OnAccelerateHeight(bool up, int customAcceleration)
		{
			var acceleration = (up ? 1 : -1) * (customAcceleration == 0 ? info.Acceleration * 2 : customAcceleration);

			Force += new CPos(0, 0, acceleration);

			return acceleration;
		}
	}
}
