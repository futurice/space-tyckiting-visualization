using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public static class Settings
	{
		public const int gridSize = 14;
		public const float cellSize = 10f;	// Width of one hex edge
		public static readonly float cellHeight = 2f*cellSize;
		public static readonly float cellWidth = Mathf.Sqrt(3f)/2f * cellHeight;
		public const float minRoundLength = 3;
		public const int maxMove = 2;
		public const int radarArea = 3;

		public static Vector3 GetWorldCoordinate(int x, int y)
		{
			var pixelX = cellWidth * (x + (y / 2f));
			var pixelY = -1 * cellHeight * (3f/4f) * y;
			return new Vector3 (pixelX, 0, pixelY);
		}
	}
}