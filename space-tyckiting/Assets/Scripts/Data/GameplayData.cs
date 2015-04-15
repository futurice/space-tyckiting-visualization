using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace SpaceTyckiting
{
	public struct GameplayData
	{
		public readonly string teamNameOne;
		public readonly string teamNameTwo;
		public readonly BotData[] bots;
		public readonly AsteroidData[] asteroids;
		public readonly GameTurndata[] turns;

		public GameplayData(string teamNameOne, string teamNameTwo, BotData[] bots, GameTurndata[] turns, AsteroidData[] asteroids)
		{
			this.teamNameOne = teamNameOne;
			this.teamNameTwo = teamNameTwo;
			this.bots = bots;
			this.turns = turns;
			this.asteroids = asteroids;
		}

		public static GameplayData FromJson(string json)
		{
			var data = JSON.Parse(json);

			string teamNameOne = "";
			string teamNameTwo = "";
			var bots = new List<BotData>();
			var turns = new List<GameTurndata>();
			var asteroids = new List<AsteroidData> ();

			foreach (var message in data.Children) {
				switch (message["type"]) {
				case "connected":
					// TODO: Read config
					break;
				case "start":
					var teamsData = message["teams"];
					// Read team 1
					var teamDataOne = teamsData[0];
					teamNameOne = teamDataOne["name"];
					int teamIdOne = 0;  // Need to be zero indexed
					foreach (var botData in teamDataOne["bots"].Children) {
						bots.Add(new BotData(botData["botId"].AsInt, botData["name"], teamIdOne, botData["pos"]["x"].AsInt, botData["pos"]["y"].AsInt));
					}

					// Read team 2
					var teamDataTwo = teamsData[1];
					teamNameTwo = teamDataTwo["name"];
					int teamIdTwo = 1;  // Need to be zero indexed
					foreach (var botData in teamDataTwo["bots"].Children) {
						bots.Add(new BotData(botData["botId"].AsInt, botData["name"], teamIdTwo, botData["pos"]["x"].AsInt, botData["pos"]["y"].AsInt));
					}

					break;
				case "round":
					// Read asteroids when first available
					if (asteroids.Count <= 0) {
						foreach (var asteroidData in message["asteroids"].Children) {
							asteroids.Add(new AsteroidData(asteroidData["x"].AsInt, asteroidData["y"].AsInt));
						}
					}

					// Read actions and events
					var radars = new List<PlayerAction>();
					var moves = new List<PlayerAction>();
					var cannons = new List<PlayerAction>();
					var sees = new List<PlayerAction>();
					var radarEchos = new List<PlayerAction>();
					var damages = new List<PlayerAction>();
					var deaths = new List<PlayerAction>();
					foreach (var actionData in message["actions"].Children) {
						switch (actionData["type"]) {
						case "radar":
							radars.Add(new PlayerAction(actionData["botId"].AsInt, actionData["pos"]["x"].AsInt, actionData["pos"]["y"].AsInt));
							break;
						case "move":
							break;
						case "cannon":
							cannons.Add(new PlayerAction(actionData["botId"].AsInt, actionData["pos"]["x"].AsInt, actionData["pos"]["y"].AsInt));
							break;
						default:
							Debug.Log ("Unkown action type " + actionData["type"]);
							break;
						}
					}
					foreach (var eventData in message["events"].Children) {
						switch (eventData["event"]) {
						case "see":
							sees.Add(new PlayerAction(eventData["botId"].AsInt, eventData["pos"]["x"].AsInt, eventData["pos"]["y"].AsInt));
							break;
						case "move":
							moves.Add(new PlayerAction(eventData["botId"].AsInt, eventData["pos"]["x"].AsInt, eventData["pos"]["y"].AsInt));
							break;
						case "damaged":
							damages.Add(new PlayerAction(eventData["botId"].AsInt, eventData["damage"].AsInt));
							break;
						case "die":
							deaths.Add(new PlayerAction(eventData["botId"].AsInt));
							break;
						case "radarEcho":
							radarEchos.Add(new PlayerAction(eventData["botId"].AsInt, eventData["pos"]["x"].AsInt, eventData["pos"]["y"].AsInt));
							break;
						case "hit":
						case "noaction":
						case "detected":
						case "seeAsteroid":
							break;
						default:
							Debug.Log ("Unkown event type " + eventData["event"]);
							break;
						}
					}

					turns.Add(new GameTurndata(radars.ToArray(), moves.ToArray(), cannons.ToArray(),
					                           sees.ToArray(), radarEchos.ToArray(), damages.ToArray(), deaths.ToArray()));
					break;
				case "endSummary":
					// TODO: Add end event
					break;
				default:
					Debug.Log ("Unkown message type " + message["type"]);
					break;
				}
			}

			return new GameplayData(teamNameOne, teamNameTwo, bots.ToArray(), turns.ToArray(), asteroids.ToArray());
		}
	}
}