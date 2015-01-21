using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public struct GameTurndata
	{
		public readonly PlayerAction[] radars;
		public readonly PlayerAction[] moves;
		public readonly PlayerAction[] cannons;

		public GameTurndata(PlayerAction[] radars, PlayerAction[] moves, PlayerAction[] cannons)
		{
			this.radars = radars;
			this.moves = moves;
			this.cannons = cannons;
		}
	}
}