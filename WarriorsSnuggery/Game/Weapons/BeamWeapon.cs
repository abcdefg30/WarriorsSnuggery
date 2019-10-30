﻿/*
 * User: Andreas
 * Date: 25.11.2017
 * 
 */
using System;
using WarriorsSnuggery.Graphics;


namespace WarriorsSnuggery.Objects
{
	class BeamWeapon : Weapon
	{
		readonly ImageRenderable[] renderables;
		readonly int renderabledistance;
		readonly int tick;
		CPos originPos;
		int curTick;
		int frame;

		public BeamWeapon(World world, WeaponType type, CPos origin, CPos target) : base(world, type, origin, target)
		{
			Target += getInaccuracy();
			originPos = origin;
			tick = type.Textures.Tick;

			renderabledistance = (1024 * type.Textures.Height) / 24;
			var sprite = TextureManager.Sprite(type.Textures);
			renderables = new ImageRenderable[sprite.Length];
			init(sprite);
		}

		public BeamWeapon(World world, WeaponType type, Actor origin, CPos target) : base(world, type, origin, target)
		{
			Target += getInaccuracy();
			originPos = origin.ActiveWeapon.WeaponOffsetPosition;
			tick = type.Textures.Tick;

			renderabledistance = (1024 * type.Textures.Height) / 24;
			var sprite = TextureManager.Sprite(type.Textures);
			renderables = new ImageRenderable[sprite.Length];
			init(sprite);
		}

		void init(ITexture[] sprite)
		{
			for (int i = 0; i < sprite.Length; i++)
			{
				renderables[i] = new ImageRenderable(sprite[i]);
			}
			curTick = tick;
		}

		public override void Render()
		{
			var distance = originPos.DistToXY(Target);
			var angle = Target.AngleToXY(originPos);
			var fit = distance / renderabledistance;

			var curFrame = frame;
			for (int i = 0; i < fit; i++)
			{
				var renderable = renderables[curFrame];

				var posX = (int)(Math.Cos(angle) * i * renderabledistance);
				var posY = (int)(Math.Sin(angle) * i * renderabledistance);

				renderable.SetRotation(new VAngle(0, 0, -angle) + new VAngle(0, 0, 270));
				renderable.SetPosition(originPos + new CPos(posX, posY, 0));
				renderable.Render();

				curFrame--;
				if (curFrame < 0)
					curFrame = renderables.Length - 1;
			}
		}

		public override void Tick()
		{
			base.Tick();
			curTick--;
			if (curTick < 0)
			{
				frame++;
				if (frame >= renderables.Length)
					frame = 0;

				curTick = tick;
			}

			if (Origin != null)
			{
				originPos = Origin.ActiveWeapon.WeaponOffsetPosition;
			}
		}

		public override void Dispose()
		{
			//foreach (var renderable in renderables)
			//renderable.Dispose();

			base.Dispose();
		}
	}
}
