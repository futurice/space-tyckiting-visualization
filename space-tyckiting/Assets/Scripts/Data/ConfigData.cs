using UnityEngine;

namespace SpaceTyckiting
{
	public struct ConfigData
	{
		public readonly int bots;
		public readonly int fieldRadius;
		public readonly int move;
		public readonly int startHp;
		public readonly int cannon;
		public readonly int radar;
		public readonly int see;
		public readonly int maxCount;
		public readonly int loopTime;

		public ConfigData (int bots, int fieldRadius, int move, int startHp, int cannon, int radar, int see, int maxCount, int loopTime)
		{
			this.bots = bots;
			this.fieldRadius = fieldRadius;
			this.move = move;
			this.startHp = startHp;
			this.cannon = cannon;
			this.radar = radar;
			this.see = see;
			this.maxCount = maxCount;
			this.loopTime = loopTime;
		}
	}
}

