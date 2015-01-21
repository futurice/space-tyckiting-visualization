using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public struct PlayerAction
	{
		public readonly int actorId;
		public readonly int targetX;
		public readonly int targetY;
		public readonly bool wasCancelled;

		public PlayerAction(int actorId, int targetX, int targetY, bool wasCancelled = false)
		{
			this.actorId = actorId;
			this.targetX = targetX;
			this.targetY = targetY;
			this.wasCancelled = wasCancelled;
		}
	}
}