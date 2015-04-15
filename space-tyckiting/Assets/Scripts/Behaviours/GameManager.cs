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
		private GameObject asteroidPrefab;
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
			SpawnAsteroids();
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

		void SpawnAsteroids()
		{
			if (GameData.asteroids == null) return;

			foreach (var asteroidData in GameData.asteroids)
			{
				var position = Settings.GetWorldCoordinate(asteroidData.x, asteroidData.y);
				var go = Instantiate(asteroidPrefab, position, Random.rotation) as GameObject;
				go.transform.parent = gameParent;
			}
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
					HandleMoves(turns[CurrentTurn].moves);

					timeUsed += 0.3f;
					yield return new WaitForSeconds(0.3f);
				}

				var spottedByRange = new List<UnitController>();
				HandleSpots(turns[CurrentTurn].sees, spottedByRange);
				spotted.AddRange(spottedByRange);
				RevealSpotted(spottedByRange, spottedLastTurn);

				CurrentPhase = GamePhase.Radars;
				var spottedByRadar = new List<UnitController>();
				if (turns[CurrentTurn].radars.Length > 0)
				{
					HandleRadars(turns[CurrentTurn].radars);
					HandleSpots(turns[CurrentTurn].radarEchos, spottedByRadar);

					timeUsed += 1f;
					yield return new WaitForSeconds(0.5f);

					RevealSpotted(spottedByRadar, spottedLastTurn);
					spotted.AddRange(spotted);

					yield return new WaitForSeconds(0.5f);
				}

				CurrentPhase = GamePhase.Cannons;
				if (turns[CurrentTurn].cannons.Length > 0)
				{

					HandleCannons(turns[CurrentTurn].cannons);

					timeUsed += 1.85f;
					yield return new WaitForSeconds(1.5f);

					HandleDamages(turns[CurrentTurn].damages);
					HandleDeaths(turns[CurrentTurn].deaths);

					timeUsed += 0.35f;
					yield return new WaitForSeconds(0.35f);
				}
				else
				{
					// Just in case someone dies without cannonings
					HandleDamages(turns[CurrentTurn].damages);
					HandleDeaths(turns[CurrentTurn].deaths);
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

		private void HandleMoves(PlayerAction[] moves)
		{
			foreach (var action in moves)
			{
				var unit = GetUnit(action.actorId);
				if (unit != null) unit.MoveTo(action.targetX, action.targetY);
			}			
		}

		private void HandleSpots(PlayerAction[] sees, List<UnitController> spotted)
		{
			foreach (var action in sees)
			{
				var unit = GetUnit(action.actorId);
				if (unit != null) spotted.Add(unit);
			}			
		}

		private void HandleRadars(PlayerAction[] radars)
		{
			foreach (var action in radars)
			{
				var unit = GetUnit(action.actorId);
				if (unit != null) unit.Radar(action.targetX, action.targetY);
			}			
		}

		private void HandleCannons(PlayerAction[] cannons)
		{
			foreach (var action in cannons)
			{
				var unit = GetUnit(action.actorId);
				if (unit != null) unit.Shoot(action.targetX, action.targetY, 0.2f);
			}			
		}

		private void HandleDamages(PlayerAction[] damages)
		{
			foreach (var action in damages)
			{
				var unit = GetUnit(action.actorId);
				if (unit != null) unit.TakeDamage(action.amount);
			}
		}

		private void HandleDeaths(PlayerAction[] deaths)
		{
			foreach (var action in deaths)
			{
				var unit = GetUnit(action.actorId);
				if (unit != null) unit.Die();
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
