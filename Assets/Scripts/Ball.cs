using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball
{
	public Tile looseInTile;
	public Athlete currentPossessor;

	public BallObject ballGameObject;

	private Vector2Int spawn;

	private RuntimeData runtimeData;
	public Ball(RuntimeData data)
	{
		runtimeData = data;
	}

	public void SetBallSpawn(Vector2Int initialSpawn)
	{
		spawn = initialSpawn;

		AssignToTile(runtimeData.field[spawn.x, spawn.y]);
	}

	public void AssignToTile(Tile tile)
	{
		if (tile.occupier != null)
		{
			tile.occupier.PossessBall(this);
		}
		else
		{
			looseInTile = tile;
			tile.AssignLooseBall(this);
		}

		ballGameObject.MoveToTileObject(looseInTile.tileGameObject);
	}

	public void AssignToAthlete(Athlete athlete) //Must always go through Athlete.PossessBall
	{
		Debug.Log("Assigned to athlete");

		looseInTile = null;

		currentPossessor = athlete;

		ballGameObject.DisplayPosession(athlete);
	}

	public void ResetBall()
	{
		if(looseInTile != null)
		{
			looseInTile.ballLooseInTile = null;
			looseInTile = null;
		}

		if(currentPossessor != null)
		{
			currentPossessor.heldBall = null;
			currentPossessor = null;
		}

		Tile spawnTile = runtimeData.field[spawn.x, spawn.y];

		ballGameObject.DisplayReset(spawnTile.tileGameObject.transform.position);

		AssignToTile(spawnTile);
	}
}
