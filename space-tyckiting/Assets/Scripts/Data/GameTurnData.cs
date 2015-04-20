using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public struct GameTurndata
	{
		public readonly PlayerAction[] radars;
		public readonly PlayerAction[] moves;
		public readonly PlayerAction[] cannons;
		public readonly PlayerAction[] sees;
		public readonly PlayerAction[] radarEchos;
		public readonly PlayerAction[] damages;
		public readonly PlayerAction[] deaths;

		public GameTurndata(PlayerAction[] radars, PlayerAction[] moves, PlayerAction[] cannons,
		                    PlayerAction[] sees, PlayerAction[] radarEchos, PlayerAction[] damages, PlayerAction[] deaths)
		{
			this.radars = radars;
			this.moves = moves;
			this.cannons = cannons;
			this.sees = sees;
			this.radarEchos = radarEchos;
			this.damages = damages;
			this.deaths = deaths;
		}
	}
}