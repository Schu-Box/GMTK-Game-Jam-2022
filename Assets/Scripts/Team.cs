using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
	public Color teamColor;

	public List<Vector2Int> startingPositions;

	[HideInInspector] public RuntimeData runtimeData;

	public List<Athlete> athletesInPlay = new List<Athlete>();

	private int numDicePerTurn = 3;

	public void Setup(RuntimeData data)
	{
		runtimeData = data;

		for (int i = 0; i < startingPositions.Count; i++)
		{
			AddAthleteToRoster(new Athlete(this));
		}
	}

	public void AddAthleteToRoster(Athlete newAthlete)
	{
		athletesInPlay.Add(newAthlete);
		runtimeData.gameController.GenerateAthleteGameObject(newAthlete);
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


		foreach (Athlete athlete in athletesInPlay)
			athlete.PerformAction();
	}

	public void ScoreGoal(Athlete scorer, Ball ball)
	{
		Debug.Log("GOAL by " + scorer.name);

		ball.ResetBall();
	}
}
