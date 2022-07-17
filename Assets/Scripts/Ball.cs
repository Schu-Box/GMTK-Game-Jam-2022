using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball
{
	public Tile currentTile_NoPossession;
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

		AttemptToEnterTile(runtimeData.field[spawn.x, spawn.y]);
	}

	public void AddToMovementQueue(Tile tile)
	{
		movementQueue.Add(tile);
	}

	public void AbortMovementQueue()
	{
		movementQueue = new List<Tile>();
	}

	public void BeginMovement()
	{
		while (movementQueue.Count > 0)
		{
			AttemptToEnterTile(movementQueue[0]);
		}
	}

	public void AttemptToEnterTile(Tile tile)
	{
		if (tile.GetOccupier() == null) 
		{
			if(tile.GetLooseBall() == null) //Tile is completely empty, ball may enter freely
			{
				CompleteMovement(tile);
			}
			else //Already a loose ball in tile, deflection occurs
			{
				Debug.Log("Thinks there's a loose ball");

				Deflection(tile);
			}
		}
		else
		{
			if(tile.GetOccupier().heldBall == null)
			{
				CompleteMovement(tile);
				AbortMovementQueue();
			}
			else //Athlete already holds ball so it deflects
			{
				Debug.Log("Thinks athlete holds?");

				Deflection(tile);
			}
		}
	}

	public void CompleteMovement(Tile tile)
	{
		if (currentTile_NoPossession != null)
		{
			currentTile_NoPossession.LooseBallExited(this);
			currentTile_NoPossession = null;
		}

		ballGameObject.QueueDisplayMovement(tile);

		if (tile.GetOccupier() != null)
		{
			tile.GetOccupier().PossessBall(this);
		}
		else
		{
			currentTile_NoPossession = tile;
			tile.LooseBallEntered(this);
		}

		movementQueue.Remove(tile);
	}

	public void Deflection(Tile tileDeflectedFrom)
	{
		Debug.Log("TODO: Deflection");

		AbortMovementQueue();

		//AddToMovementQueue(runtimeData.GetAdjacentTiles(tileDeflectedFrom), 1, false);
		
	}

	public void AssignPossessor(Athlete newPossessor)
	{
		if(newPossessor == null)
		{
			Debug.Log("assigning null possessor");
		}
		else
		{
			Debug.Log("assigning possessor " + newPossessor.name);
		}

		if (currentPossessor != null)
			currentPossessor.heldBall = null;

		if(newPossessor != null)
		{
			currentTile_NoPossession = null;

			currentPossessor = newPossessor;
			newPossessor.heldBall = this;
		}

		ballGameObject.QueueDisplayPossession(newPossessor);
	}

	public void ResetBall()
	{
		if(currentTile_NoPossession != null)
		{
			currentTile_NoPossession.LooseBallExited(null);
			currentTile_NoPossession = null;
		}

		if(currentPossessor != null)
		{
			currentPossessor.heldBall = null;
			currentPossessor = null;
		}

		Tile spawnTile = runtimeData.field[spawn.x, spawn.y];

		ballGameObject.QueueDisplayReset(spawnTile.tileGameObject.transform.position);

		AttemptToEnterTile(spawnTile);
	}
}
