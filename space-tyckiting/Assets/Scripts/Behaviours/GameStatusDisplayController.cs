using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SpaceTyckiting
{
	public class GameStatusDisplayController : MonoBehaviour
	{
		[SerializeField]
		private UnityEngine.UI.Text playerOneNameText;
		[SerializeField]
		private UnityEngine.UI.Text playerOneScoreText;
		[SerializeField]
		private UnityEngine.UI.Text playerTwoNameText;
		[SerializeField]
		private UnityEngine.UI.Text playerTwoScoreText;
		[SerializeField]
		private UnityEngine.UI.Text turnNumberDisplay;

		private GameplayData data;

		private int maxHPTeamOne;
		private int maxHPTeamTwo;

		void OnEnable()
		{
			GameManager.GameStarted += GameManager_GameStarted;
			GameManager.GameEnded += GameManager_GameEnded;
			GameManager.GameLoaded += GameManager_GameLoaded;
		}

		void OnDisable()
		{
			GameManager.GameStarted -= GameManager_GameStarted;
			GameManager.GameEnded -= GameManager_GameEnded;
			GameManager.GameLoaded -= GameManager_GameLoaded;
		}

		void GameManager_GameLoaded()
		{
			data = GameManager.Instance.GameData;

			playerOneNameText.text = data.teamNameOne;
			playerTwoNameText.text = data.teamNameTwo;
			turnNumberDisplay.text = "Game loaded";

			playerOneScoreText.text = "-";
			playerTwoScoreText.text = "-";
		}	

		void GameManager_GameStarted()
		{
			Init();
		}

		void GameManager_GameEnded()
		{
			var hp1 = GetHitPointsForBots(0);
			var hp2 = GetHitPointsForBots(1);

			if (hp1 > hp2)
			{
				turnNumberDisplay.text = data.teamNameOne + " wins";
			}
			else if (hp2 > hp1)
			{
				turnNumberDisplay.text = data.teamNameTwo + " wins";
			}
			else
			{
				turnNumberDisplay.text = "DRAW";
			}
		}

		void Init()
		{
			data = GameManager.Instance.GameData;

			playerOneNameText.text = data.teamNameOne;
			playerTwoNameText.text = data.teamNameTwo;

			maxHPTeamOne = GetHitPointsForBots(0);
			maxHPTeamTwo = GetHitPointsForBots(1);

			UpdateScoreDisplays();
		}

		void UpdateScoreDisplays()
		{
			int teamOne = GetHitPointsForBots(0);
			int teamTwo = GetHitPointsForBots(1);

			playerOneScoreText.text = teamOne + "/" + maxHPTeamOne;
			playerTwoScoreText.text = teamTwo + "/" + maxHPTeamTwo;
		}

		int GetHitPointsForBots(int faction)
		{
			int total = 0;

			if (GameManager.Instance.Units != null)
			{
				for (int i = 0; i < GameManager.Instance.Units.Count; i++)
				{
					if (GameManager.Instance.Units[i] == null) continue;

					if (GameManager.Instance.Units[i].FactionId == faction)
					{
						total += GameManager.Instance.Units[i].HitPoints;
					}
				}
			}

			return total;
		}

		void Update()
		{
			if (GameManager.Instance.CurrentPhase != GameManager.GamePhase.None)
			{
				turnNumberDisplay.text = "Turn " + (GameManager.Instance.CurrentTurn + 1);
				UpdateScoreDisplays();
			}
		}
	}
}