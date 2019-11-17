﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarriorsSnuggery.Objects.Conditions
{
	public class Condition
	{
		public readonly bool Negate;
		public readonly string Type;
		readonly Condition[] children;
		readonly Operation operation;

		enum Operation
		{
			NONE,
			AND,
			OR
		}

		public Condition(string input)
		{
			input = input.Trim();
			if (input.Contains("||"))
			{
				operation = Operation.OR;
				var split = input.LastIndexOf("||");
				children = new Condition[2];
				children[0] = new Condition(input.Substring(0, split));
				children[1] = new Condition(input.Substring(split + 2));
			}
			else if (input.Contains("&&"))
			{
				operation = Operation.AND;
				var split = input.LastIndexOf("&&");
				children = new Condition[2];
				children[0] = new Condition(input.Substring(0, split));
				children[1] = new Condition(input.Substring(split + 2));
			}
			else
			{
				if (input.StartsWith("!"))
				{
					Negate = true;
					input = input.Remove(0, 1);
				}

				Type = input;
			}
		}

		public bool True(Actor actor)
		{
			switch (operation)
			{
				case Operation.AND:
					return children[0].True(actor) && children[1].True(actor);
				case Operation.OR:
					return children[0].True(actor) || children[1].True(actor);
				default:
					return actor.World.Game.ConditionManager.CheckCondition(this, actor);
			}
		}
	}
}