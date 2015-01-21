using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public struct BotData
	{
		public readonly string name;
		public readonly int faction;
		public readonly int actorId;
		public readonly int startX;
		public readonly int startY;

		public BotData(int actorId, string name, int faction, int startX, int startY)
		{
			this.actorId = actorId;
			
			this.name = name;
			this.faction = faction;
			this.startX = startX;
			this.startY = startY;
		}
	}
}