using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpaceTyckiting
{
	public class GameManager : MonoBehaviour
	{
		public enum GamePhase
		{
			None = 0,
			Radars = 1,
			Moves = 2,
			Cannons = 3
		}

		public static GameManager Instance { get; private set; }

		public static event System.Action GameStarted;
		public static event System.Action GameEnded;
		public static event System.Action GameLoaded;
		public static event System.Action<GamePhase, GamePhase> GamePhaseChanged;

		[SerializeField]
		private UnitController unitPrefabFaction1;
		[SerializeField]
		private UnitController unitPrefabFaction2;
		[SerializeField]
		private NameLabelController unitLabelPrefab1;
		[SerializeField]
		private NameLabelController unitLabelPrefab2;
		[SerializeField]
		private Canvas gameWorldUICanvasTop;
		[SerializeField]
		private Transform gameParent;
		public Transform GameParent { get { return gameParent; } }

		public List<UnitController> Units {get; private set; }

		public int CurrentTurn { get; private set; }

		private GamePhase _currentPhase;
		public GamePhase CurrentPhase
		{
			get { return _currentPhase;  }
			set {
				if (value != _currentPhase) {
					if (GamePhaseChanged != null) {
						GamePhaseChanged(_currentPhase, value);
					}
					_currentPhase = value;
				} 
			}
		}

		private GameplayData _gameData;
		public GameplayData GameData {
			get { return _gameData;}
			set { Clean(); _gameData = value; }
		}

		void Awake()
		{
			Instance = this;
		
			LeanTween.init(200);
		}

		void OnEnable()
		{
			GameDataLoader.GameplayDataLoaded += GameDataLoader_GameplayDataLoaded;
		}

		void GameDataLoader_GameplayDataLoaded(GameplayData data)
		{
			GameData = data;

			if (GameLoaded != null) GameLoaded();
		}

		public void Play()
		{
			Clean();
			SpawnUnits();
			StartCoroutine(Play_Coroutine(GameData));
		}

		void Clean()
		{
			StopAllCoroutines();
			for (int i = 0; i < gameParent.childCount; i++)
			{
				Destroy(gameParent.GetChild(i).gameObject);
			}
			CurrentPhase = GamePhase.None;
		}

		void SpawnUnits()
		{
			if (GameData.bots == null) return;

			Units = new List<UnitController>();
			
			for (int i = 0; i < GameData.bots.Length; i++)
			{
				CreateUnit(GameData.bots[i]);
			}
		}

		void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape)) Quit();
		}

		void CreateUnit(BotData data)
		{
			GameObject prefab;
			GameObject labelPrefab;
			if (data.faction <= 0)
			{
				prefab = unitPrefabFaction1.gameObject;
				labelPrefab = unitLabelPrefab1.gameObject;
			}
			else
			{
				prefab = unitPrefabFaction2.gameObject;
				labelPrefab = unitLabelPrefab2.gameObject;
			}

			var go = Instantiate(prefab) as GameObject;
			go.GetComponent<Transform>().parent = gameParent;
			var unit = go.GetComponent<UnitController>();
			unit.SetPos(data.startX, data.startY);
			unit.ActorId = data.actorId;
			unit.FactionId = data.faction;
			unit.Data = data;
			Units.Add(unit);

			var labelGo = Instantiate(labelPrefab) as GameObject;
			var labelRt = labelGo.GetComponent<RectTransform>();
			labelRt.SetParent(gameWorldUICanvasTop.GetComponent<RectTransform>());
			labelRt.localEulerAngles = Vector3.zero;
			var label = labelGo.GetComponent<NameLabelController>();
			label.Init(unit);
		}

		public void HandleExplosion(int x, int y)
		{
			for (int i = 0; i < Units.Count; i++)
			{
				var distance = SquareDistance(x, y, Units[i].PositionX, Units[i].PositionY);
				if (distance == 0)
				{
					// Direct hit
					Units[i].TakeDamage(2);
				}
				else if (distance == 1)
				{
					// Side hit
					Units[i].TakeDamage(1);
				}
			}
		}

		private int SquareDistance(int x1, int y1, int x2, int y2)
		{
			return Mathf.Max(Mathf.Abs(x1 - x2), Mathf.Abs(y1 - y2));
		}

		public int UnitsInCell(int x, int y)
		{
			int count = 0;

			for (int i = 0; i < Units.Count; i++)
			{
				if (Units[i] != null && Units[i].PositionX == x && Units[i].PositionY == y)
					count++;
			}

			return count;
		}

		IEnumerator Play_Coroutine(GameplayData game)
		{
			CurrentTurn = 0;
			CurrentPhase = GamePhase.None;

			if (GameStarted != null) GameStarted();

			var turns = game.turns;

			if (turns == null) yield break;

			var spotted = new List<UnitController>();
			var spottedLastTurn = new List<UnitController>();

			yield return new WaitForSeconds(3);

			var time = Time.time;

			while (CurrentTurn < turns.Length)
			{
				float timeUsed = 0;

				spottedLastTurn.Clear();
				spottedLastTurn.AddRange(spotted);
				spotted.Clear();

				CurrentPhase = GamePhase.Moves;
				if (turns[CurrentTurn].moves.Length > 0)
				{
					for (int i = 0; i < turns[CurrentTurn].moves.Length; i++)
					{
						var action = turns[CurrentTurn].moves[i];
						if (action.wasCancelled) continue;

						var unit = GetUnit(action.actorId);

						if (unit != null)
						{
							var finalTargetX = Mathf.Clamp(action.targetX, 0, Settings.gridSize - 1);
							var finalTargetY = Mathf.Clamp(action.targetY, 0, Settings.gridSize - 1);

							if (finalTargetX > unit.PositionX + Settings.maxMove || finalTargetX < unit.PositionX - Settings.maxMove
								|| finalTargetY > unit.PositionY + Settings.maxMove || finalTargetY < unit.PositionY - Settings.maxMove
								|| (finalTargetX == unit.PositionX && finalTargetY == unit.PositionY))
							{
								Debug.Log("Illegal move for bot " + unit.ActorId + " from " + unit.PositionX + ", " + unit.PositionY + " to " + finalTargetX + ", " + finalTargetY);
							}
							else
							{
								unit.MoveTo(finalTargetX, finalTargetY);
							}

							//finalTargetX = Mathf.Clamp(finalTargetX, unit.PositionX - Settings.maxMove, unit.PositionX + Settings.maxMove);
							//finalTargetY = Mathf.Clamp(finalTargetY, unit.PositionY - Settings.maxMove, unit.PositionY + Settings.maxMove);
						}
					}

					timeUsed += 0.3f;
					yield return new WaitForSeconds(0.3f);
				}

				var spottedByRange = new List<UnitController>();
				for (int i = 0; i < Units.Count; i++)
				{
					for (int j = 0; j < Units.Count; j++)
					{
						if (Units[i] != null && Units[j] != null && Units[i].FactionId != Units[j].FactionId)
						{
							bool inRange = SquareDistance(Units[i].PositionX, Units[i].PositionY, Units[j].PositionX, Units[j].PositionY) <= 2;

							if (inRange && !spotted.Contains(Units[i]) && !spottedByRange.Contains(Units[i]))
							{
								spottedByRange.Add(Units[i]);
							}
							if (inRange && !spotted.Contains(Units[j]) && !spottedByRange.Contains(Units[j]))
							{
								spottedByRange.Add(Units[j]);
							}
						}
					}
				}

				spotted.AddRange(spottedByRange);
				RevealSpotted(spottedByRange, spottedLastTurn);

				CurrentPhase = GamePhase.Radars;
				var spottedByRadar = new List<UnitController>();
				if (turns[CurrentTurn].radars.Length > 0)
				{
					for (int i = 0; i < turns[CurrentTurn].radars.Length; i++)
					{
						var action = turns[CurrentTurn].radars[i];
						if (action.wasCancelled) continue;

						var unit = GetUnit(action.actorId);
						if (unit != null)
						{
							unit.Radar(action.targetX, action.targetY);
							AddSpottedToList(spottedByRadar, action.targetX, action.targetY, unit.FactionId);
						}
					}

					timeUsed += 1f;
					yield return new WaitForSeconds(0.5f);

					RevealSpotted(spottedByRadar, spottedLastTurn);
					spotted.AddRange(spotted);

					yield return new WaitForSeconds(0.5f);
				}

				CurrentPhase = GamePhase.Cannons;
				if (turns[CurrentTurn].cannons.Length > 0)
				{
					for (int i = 0; i < turns[CurrentTurn].cannons.Length; i++)
					{
						var action = turns[CurrentTurn].cannons[i];
						if (action.wasCancelled) continue;

						var unit = GetUnit(action.actorId);
						if (unit != null) unit.Shoot(action.targetX, action.targetY, 0.2f);
					}

					timeUsed += 1.85f;
					yield return new WaitForSeconds(1.5f);

					List<int[]> explosions = new List<int[]>();
					for (int i = 0; i < turns[CurrentTurn].cannons.Length; i++)
					{
						var action = turns[CurrentTurn].cannons[i];
						if (action.wasCancelled) continue;

						var unit = GetUnit(action.actorId);
						if (unit != null) explosions.Add(new int[] { action.targetX, action.targetY });
					}
					for (int i = 0; i < explosions.Count; i++)
					{
						HandleExplosion(explosions[i][0], explosions[i][1]);
					}

					timeUsed += 0.35f;
					yield return new WaitForSeconds(0.35f);
				}

				float timeToWait = Mathf.Max(Settings.minRoundLength - timeUsed, 0.35f);
				
				yield return new WaitForSeconds(timeToWait);

				if (CountLiveUnitsForFaction(0) > 0 && CountLiveUnitsForFaction(1) > 0)
				{
					CurrentTurn++;
				}
				else
				{
					break;
				}
			}

			Debug.Log("Time taken: " + (Time.time - time));

			CurrentPhase = GamePhase.None;

			if (GameEnded != null) GameEnded();

			yield return new WaitForSeconds(0.1f);

			Debug.Log("Game over. " + turns.Length + " turns in " + (Time.time - time) + " seconds. (avg. " + ((Time.time - time) / (float)turns.Length) + ")");
		}

		private void RevealSpotted(List<UnitController> spotted, List<UnitController> spottedLastTurn)
		{
			for (int i = 0; i < spotted.Count; i++)
			{
				spotted[i].Reveal(!spottedLastTurn.Contains(spotted[i]));
			}
		}

		private void AddSpottedToList(List<UnitController> spottedList, int centerX, int centerY, int spotterFaction)
		{
			for (int i = 0; i < Units.Count; i++)
			{
				if (Units[i] != null && !spottedList.Contains(Units[i]) && Units[i].FactionId != spotterFaction)
				{
					if (SquareDistance(centerX, centerY, Units[i].PositionX, Units[i].PositionY) <= (Settings.radarArea - 1) / 2)
					{
						spottedList.Add(Units[i]);
					}
				}
			}
		}

		UnitController GetUnit(int actorId)
		{
			for (int i = 0; i < Units.Count; i++)
			{
				if (Units[i] != null && Units[i].ActorId == actorId) return Units[i];
			}

			return null;
		}

		int CountLiveUnitsForFaction(int faction)
		{
			int count = 0;
			for (int i = 0; i < Units.Count; i++)
			{
				if (Units[i] != null && Units[i].FactionId == faction) count++;
			}
			return count;
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}
