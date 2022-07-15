using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
	Field,
	Goal
}

public class Tile
{
	public Team owner;
	public TileType tileType;

	public TileObject tileGameObject;

	public Athlete occupier = null;
	public Ball ballLooseInTile = null;

	public void SetAsGoal()
	{
		tileType = TileType.Goal;
	}

	public void SetForTeam(Team team)
	{
		owner = team;
	}

	public void AssignLooseBall(Ball looseBall)
	{
		ballLooseInTile = looseBall;
	}

	public void AthleteEntered(Athlete athlete)
	{
		occupier = athlete;

		if(ballLooseInTile != null && occupier.heldBall == null)
		{
			occupier.PossessBall(ballLooseInTile);
			ballLooseInTile = null;
		}

		if(tileType == TileType.Goal)
		{
			//Debug.Log("Athlete entered goal, should not be allowed maybe");

			if(athlete.heldBall != null)
			{
				athlete.team.ScoreGoal(athlete, athlete.heldBall);
			}
		}
	}

	public void AthleteExited(Athlete athlete)
	{
		occupier = null;
	}
}
