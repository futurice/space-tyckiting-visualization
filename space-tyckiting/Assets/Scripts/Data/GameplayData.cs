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
		public readonly GameTurndata[] turns;

		public GameplayData(string teamNameOne, string teamNameTwo, BotData[] bots, GameTurndata[] turns)
		{
			this.teamNameOne = teamNameOne;
			this.teamNameTwo = teamNameTwo;
			this.bots = bots;
			this.turns = turns;
		}

		public static GameplayData FromJson(string json)
		{
			var data = JSON.Parse(json);

			var teamsData = data["teams"];

			var teamData1 = teamsData[0];
			string teamNameOne = teamData1["team"];
			int teamIdOne = teamData1["id"].AsInt;
			var botsData1 = teamData1["bots"];
			var bots = new List<BotData>();
			foreach (var botData in botsData1.Children)
			{
				bots.Add(new BotData(botData["id"].AsInt, botData["name"],  teamIdOne, botData["x"].AsInt, botData["y"].AsInt));
			}

			var teamData2 = teamsData[1];
			string teamNameTwo = teamData2["team"];
			int teamIdTwo = teamData2["id"].AsInt;
			var botsData2 = teamData2["bots"];
			
			foreach (var botData in botsData2.Children)
			{
				bots.Add(new BotData(botData["id"].AsInt, botData["name"], teamIdTwo, botData["x"].AsInt, botData["y"].AsInt));
			}
			
			var turnsData = data["turns"];
			var turns = new List<GameTurndata>();

			foreach (var turnData in turnsData.Children)
			{
				var radars = new List<PlayerAction>();
				foreach (var radarsData in turnData["radars"].Children)
				{
					radars.Add(new PlayerAction(radarsData["botId"].AsInt, radarsData["x"].AsInt, radarsData["y"].AsInt));
				}
				var moves = new List<PlayerAction>();
				foreach (var movesData in turnData["moves"].Children)
				{
					moves.Add(new PlayerAction(movesData["botId"].AsInt, movesData["x"].AsInt, movesData["y"].AsInt));
				}
				var cannons = new List<PlayerAction>();
				foreach (var cannonsData in turnData["cannons"].Children)
				{
					cannons.Add(new PlayerAction(cannonsData["botId"].AsInt, cannonsData["x"].AsInt, cannonsData["y"].AsInt));
				}
				turns.Add(new GameTurndata(radars.ToArray(), moves.ToArray(), cannons.ToArray()));
			}
			return new GameplayData(teamNameOne, teamNameTwo, bots.ToArray(), turns.ToArray());
		}
	}
}