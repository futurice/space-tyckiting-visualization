using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public static class Settings
	{
		public const int gridSize = 28;
		public const float cellSize = 200f / (float)gridSize;
		public static readonly Vector2 gridOffset = new Vector2(-gridSize / 2 + 0.5f, gridSize / 2 - 0.5f);
		public const float minRoundLength = 3;
		public const int maxMove = 2;
		public const int radarArea = 7;

		public static Vector3 GetWorldCoordinate(int x, int y)
		{
			return new Vector3((x + gridOffset.x) * cellSize, 0, (-y + gridOffset.y) * cellSize);
		}
	}
}