﻿using System;
using System.Collections.Generic;
using System.Linq;
using WarriorsSnuggery.Objects;

namespace WarriorsSnuggery
{
	public sealed class SmudgeLayer : ITickRenderDisposable
	{
		readonly List<Smudge> smudgeList = new List<Smudge>();

		public SmudgeLayer()
		{

		}

		public void Add(Smudge smudge)
		{
			smudgeList.Add(smudge);

			if (smudgeList.Count > 128)
			{
				for (int i = 0; i < smudgeList.Count; i++)
				{
					if (!smudgeList[i].IsDissolving)
					{
						smudgeList[i].BeginDissolve();
						break;
					}
				}
			}
		}

		public void Render()
		{
			foreach (var smudge in smudgeList)
				smudge.Render();
		}

		public void Tick()
		{
			var toRemove = new List<int>();
			for (int i = 0; i < smudgeList.Count; i++)
			{
				var smudge = smudgeList[i];
				smudge.Tick();

				if (smudge.Disposed)
					toRemove.Add(i);
			}

			foreach (var i in toRemove)
				smudgeList.RemoveAt(i);
		}

		public void Dispose()
		{
			foreach (var smudge in smudgeList)
				smudge.Dispose();
			smudgeList.Clear();
		}
	}
}
