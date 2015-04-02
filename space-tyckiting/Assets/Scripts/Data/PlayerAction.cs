using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public struct PlayerAction
	{
		public readonly int actorId;
		public readonly int targetX;
		public readonly int targetY;
		public readonly int amount;

		public PlayerAction(int actorId, int targetX, int targetY)
		{
			this.actorId = actorId;
			this.targetX = targetX;
			this.targetY = targetY;
			this.amount = 0;
		}

		public PlayerAction(int actorId)
		{
			this.actorId = actorId;
			this.targetX = 0;
			this.targetY = 0;
			this.amount = 0;
		}

		public PlayerAction(int actorId, int amount)
		{
			this.actorId = actorId;
			this.targetX = 0;
			this.targetY = 0;
			this.amount = amount;
		}

	}
}