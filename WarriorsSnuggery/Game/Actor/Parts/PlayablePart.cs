﻿namespace WarriorsSnuggery.Objects.Parts
{
	[Desc("Attach this to an actor to make it playable by the player.")]
	public class PlayablePartInfo : PartInfo
	{
		[Desc("When true, this actor is playable.")]
		public readonly bool Playable;
		[Desc("When true, this actor is unlocked from the beginning of the Game. Unused.")]
		public readonly bool Unlocked;

		[Desc("Cost to unlock this actor. Unused.")]
		public readonly int UnlockCost;
		[Desc("Cost to change to this actor.")]
		public readonly int Cost;

		public override ActorPart Create(Actor self)
		{
			return new PlayablePart(self, this);
		}

		public PlayablePartInfo(string internalName, MiniTextNode[] nodes) : base(internalName, nodes)
		{

		}
	}

	public class PlayablePart : ActorPart
	{
		readonly PlayablePartInfo info;

		public bool Playable
		{
			get { return info.Playable; }
			set { }
		}

		public bool Unlocked
		{
			get { return info.Unlocked; }
			set { }
		}

		public int UnlockCost
		{
			get { return info.UnlockCost; }
			set { }
		}

		public int ChangeCost
		{
			get { return info.Cost; }
			set { }
		}

		public PlayablePart(Actor self, PlayablePartInfo info) : base(self)
		{
			this.info = info;
		}
	}
}
