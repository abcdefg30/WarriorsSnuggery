﻿using System;
using WarriorsSnuggery.Graphics;
using WarriorsSnuggery.Objects.Conditions;
using WarriorsSnuggery.Objects.Weapons;

namespace WarriorsSnuggery.Objects.Parts
{
	[Desc("This will add a sprite to an actor which will be rendered upon call.")]
	public class AnimatedSpritePartInfo : PartInfo
	{
		public readonly Texture[] Textures;

		[Desc("Name of the texture file.")]
		public readonly string Name;

		[Desc("Size of a single frame.")]
		public readonly MPos Dimensions;

		[Desc("Count of facings that the animation has.")]
		public readonly int Facings = 1;

		[Desc("Frame rate (Speed of animation).")]
		public readonly int Tick = 20;

		[Desc("If true, a random start frame of the animation will be picked.")]
		public readonly bool StartRandom = false;

		[Desc("Activate only by the following Condition.")]
		public readonly Condition Condition = new Condition("True");

		[Desc("Offset of the sprite.")]
		public readonly CPos Offset;

		[Desc("Use Sprite as preview in e.g. the editor.")]
		public readonly bool UseAsPreview;

		public override ActorPart Create(Actor self)
		{
			return new AnimatedSpritePart(self, this);
		}

		public AnimatedSpritePartInfo(string internalName, MiniTextNode[] nodes) : base(internalName, nodes)
		{
			if (Name != null)
				Textures = SpriteManager.AddTexture(new TextureInfo(Name, TextureType.ANIMATION, Tick, Dimensions.X, Dimensions.Y));
		}
	}

	public class AnimatedSpritePart : RenderablePart
	{
		readonly AnimatedSpritePartInfo info;

		readonly BatchRenderable[] renderables;
		BatchRenderable renderable;
		int currentFacing;
		Color color = Color.White;

		public AnimatedSpritePart(Actor self, AnimatedSpritePartInfo info) : base(self)
		{
			this.info = info;
			renderables = new BatchRenderable[info.Facings];
			var frameCountPerIdleAnim = info.Textures.Length / info.Facings;

			if (frameCountPerIdleAnim * info.Facings != info.Textures.Length)
				throw new YamlInvalidNodeException(string.Format(@"Idle Frame '{0}' count cannot be matched with the given Facings '{1}'.", info.Textures.Length, info.Facings));

			for (int i = 0; i < renderables.Length; i++)
			{
				var anim = new Texture[frameCountPerIdleAnim];
				for (int x = 0; x < frameCountPerIdleAnim; x++)
					anim[x] = info.Textures[i * frameCountPerIdleAnim + x];

				renderables[i] = new BatchSequence(anim, Color.White, info.Tick, startRandom: info.StartRandom);
			}
		}

		public override int FacingFromAngle(float angle)
		{
			float part = (float)(2f * Math.PI) / info.Facings;

			int facing = (int)Math.Round(angle / part);
			if (facing >= info.Facings)
				facing = 0;

			return facing;
		}

		public override BatchRenderable GetRenderable(ActorAction action, int facing)
		{
			if (info.Condition != null && !info.Condition.True(self))
				return null;

			return renderables[facing];
		}

		public override void OnMove(CPos old, CPos speed)
		{
			currentFacing = FacingFromAngle(self.Angle);
		}

		public override void OnAttack(CPos target, Weapon weapon)
		{
			currentFacing = FacingFromAngle(self.Angle);
		}

		public override void Tick()
		{
			renderable = GetRenderable(self.CurrentAction, currentFacing);
			renderable?.Tick();
		}

		public override void Render()
		{
			if (renderable == null)
				return;

			if (self.Height > 0)
			{
				renderable.SetPosition(self.GraphicPositionWithoutHeight);
				renderable.SetColor(new Color(0, 0, 0, 64));
				renderable.PushToBatchRenderer();
			}

			self.Offset = info.Offset;
			renderable.SetPosition(self.GraphicPosition);
			renderable.SetColor(color);
			renderable.PushToBatchRenderer();
		}

		public override void SetColor(Color color)
		{
			this.color = color;
		}
	}
}
