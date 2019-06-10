﻿using System;

namespace WarriorsSnuggery.Objects.Parts
{
	[Desc("Adds a weapon to the object.", "IMPORTANT NOTE: Currently, shroud is only supported for teams 0-9. If you use higher teams, the game will crash!")]
	public class RevealsShroudPartInfo : PartInfo
	{
		[Desc("Range of shroudreveal.", "Given in half terrain dimension. (2 = 1 terrain size)")]
		public readonly int Range;

		[Desc("Offset of the shoot point relative to the object's center.")]
		public readonly int Interval;

		public override ActorPart Create(Actor self)
		{
			return new RevealsShroudPart(self, this);
		}

		public RevealsShroudPartInfo(MiniTextNode[] nodes) : base(nodes)
		{
			foreach (var node in nodes)
			{
				switch (node.Key)
				{
					case "Range":
						Range = node.ToInt();
						break;
					case "Interval":
						Interval = node.ToInt();
						break;
					default:
						throw new YamlUnknownNodeException(node.Key, "WeaponPart");
				}
			}
		}
	}

	public class RevealsShroudPart : ActorPart
	{
		readonly RevealsShroudPartInfo info;
		int tick;
		bool firstActive;

		public RevealsShroudPart(Actor self, RevealsShroudPartInfo info) : base(self)
		{
			this.info = info;
			firstActive = true;
		}

		public override void OnMove(CPos old, CPos speed)
		{
			if (tick < 0)
			{
				// Use Rectangular as Circular is sill unperformant
				self.World.ShroudLayer.RevealShroudRectangular(self.Team, self.Position.ToMPos() * new MPos(2, 2), info.Range);
				tick = info.Interval;
			}
		}

		public override void Tick()
		{
			tick--;
			if (firstActive)
			{
				self.World.ShroudLayer.RevealShroudRectangular(self.Team, self.Position.ToMPos() * new MPos(2, 2), info.Range);
				firstActive = false;
			}
		}
	}
}
