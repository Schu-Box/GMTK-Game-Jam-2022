using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball
{
	public Tile looseInTile;
	public Athlete currentPossessor;

	public BallObject ballGameObject;

	private Vector2Int spawn;

	private List<Tile> movementQueue = new List<Tile>();

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

	public void AddToMovementQueue(Tile tile)
	{
		movementQueue.Add(tile);
	}

	public void AbortMovementQueue()
	{
		movementQueue = new List<Tile>();
	}

	public void AttemptMovement()
	{
		while (movementQueue.Count > 0)
		{
			AssignToTile(movementQueue[0]);
		}
	}

	public void AssignToTile(Tile tile)
	{
		if (looseInTile != null)
			looseInTile.ballLooseInTile = null;

		if (tile.occupier != null)
		{
			tile.occupier.PossessBall(this);
		}
		else
		{
			looseInTile = tile;
			tile.AssignLooseBall(this);
		}

		ballGameObject.QueueDisplayMovement(tile);
	}

	public void AssignToAthlete(Athlete athlete) //Must always go through Athlete.PossessBall
	{
		Debug.Log("Assigned to athlete");

		looseInTile = null;

		currentPossessor = athlete;

		ballGameObject.QueueDisplayPossession(athlete);
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

		ballGameObject.QueueDisplayReset(spawnTile.tileGameObject.transform.position);

		AssignToTile(spawnTile);
	}
}
