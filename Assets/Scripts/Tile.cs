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

	private Athlete occupier = null;
	private Ball ballLooseInTile = null;

	public void SetAsGoal()
	{
		tileType = TileType.Goal;
	}

	public void SetForTeam(Team team)
	{
		owner = team;
	}

	public void AthleteEntered(Athlete athlete)
	{
		occupier = athlete;

		if(ballLooseInTile != null)
		{
			if(occupier.heldBall == null)
			{
				occupier.PossessBall(ballLooseInTile);
				ballLooseInTile = null;
			}
			else //Athlete is already holding a ball, therefore it should be deflected
			{
				Debug.Log("ERROR: Athlete entered tile holding ball while ball is loose in Tile");
				ballLooseInTile.Deflection(this);
			}
		}

		if(tileType == TileType.Goal)
		{
			Debug.Log("Athlete entered goal");

			if (athlete.heldBall != null)
			{
				athlete.team.runtimeData.GetOppositeTeam(owner).ScoreGoal(athlete, athlete.heldBall);
			}
		}
	}

	public void AthleteExited(Athlete athlete)
	{
		occupier = null;
	}

	public void LooseBallEntered(Ball ball)
	{
		ballLooseInTile = ball;

		if (tileType == TileType.Goal)
		{
			owner.runtimeData.GetOppositeTeam(owner).ScoreGoal(owner.runtimeData.activeAthlete, ball);
		}
	}

	public void LooseBallExited(Ball ball)
	{
		ballLooseInTile = null;
	}

	public Athlete GetOccupier()
	{
		return occupier;
	}

	public Ball GetLooseBall()
	{
		return ballLooseInTile;
	}
}
