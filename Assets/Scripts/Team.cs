using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
	public Color teamColor;

	public List<Vector2Int> startingPositions;

	[HideInInspector] public RuntimeData runtimeData;
	[HideInInspector] public TeamUIController teamController;

	public List<Athlete> athletesInPlay = new List<Athlete>();
	public List<Dice> diceRolled = new List<Dice>();

	private int numDicePerTurn = 3;

	public void Setup(RuntimeData data, TeamUIController teamUIController)
	{
		runtimeData = data;
		teamController = teamUIController;

		for (int i = 0; i < startingPositions.Count; i++)
		{
			AddAthleteToRoster(new Athlete(this));
		}
	}

	public void AddAthleteToRoster(Athlete newAthlete)
	{
		athletesInPlay.Add(newAthlete);
		teamController.GenerateAthleteGameObject(newAthlete);
	}

	public void AssignAthletesToStartingPositions()
	{
		for(int i = 0; i < athletesInPlay.Count; i++)
		{
			Vector2Int startingPosition = startingPositions[i];
			athletesInPlay[i].AssignToTile(runtimeData.field[startingPosition.x, startingPosition.y]);
		}
	}

	public void PerformTurn()
	{
		RollDice();

		//foreach (Athlete athlete in athletesInPlay)
		//	athlete.PerformAction();
	}

	public void RollDice()
	{
		ClearDice();

		for(int i= 0; i < numDicePerTurn; i++)
		{
			Dice newDice = new Dice();
			diceRolled.Add(newDice);

			teamController.GenerateDiceGameObject(newDice);
		}
	}

	public void ClearDice()
	{
		diceRolled = new List<Dice>();
		runtimeData.gameController.DeleteAllChidlren(teamController.diceGameObjectParent);
	}

	public void ScoreGoal(Athlete scorer, Ball ball)
	{
		Debug.Log("GOAL by " + scorer.name);

		ball.ResetBall();
	}
}
