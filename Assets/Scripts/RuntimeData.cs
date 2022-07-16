using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeData
{
	public Team playerTeam;
	public Team opponentTeam;

	public List<Vector2Int> ballStartingPositions;
	public List<Ball> balls = new List<Ball>();

	public Tile[,] field;
	public int rows = 6;
	public int columns = 8;
	//public List<Athlete> athletesInPlay = new List<Athlete>();

	[HideInInspector] public GameController gameController;

	public void Setup(GameController gc)
	{
		gameController = gc;

		playerTeam.Setup(this, gameController.playerTeamController);
		opponentTeam.Setup(this, gameController.opponentTeamController);

		SetField();
		gameController.SetFieldObjects();

		SpawnBalls();

		playerTeam.AssignAthletesToStartingPositions();
		opponentTeam.AssignAthletesToStartingPositions();
	}

	public void SetField()
	{
		field = new Tile[columns, rows];

		for(int r = 0; r < rows; r++)
		{
			for(int c = 0; c < columns; c++)
			{
				field[c, r] = new Tile();

				if (c > columns / 2)
					field[c, r].SetForTeam(opponentTeam);
				else if (c == columns / 2)
					field[c, r].SetForTeam(null);
				else
					field[c, r].SetForTeam(playerTeam);
			}
		}

		for (int i = 0; i < rows; i++)
		{
			field[0, i].SetAsGoal();
		}

		for (int i = 0; i < rows; i++)
		{
			field[columns - 1, i].SetAsGoal();
		}
	}

	public Vector2Int GetFieldIntForTile(Tile tile)
	{
		for(int r = 0; r < rows; r++)
		{
			for(int c = 0; c < columns; c++)
			{
				if (field[c, r] == tile)
				{
					return new Vector2Int(c, r);
				}
			}
		}

		Debug.Log("ERROR: Tile doesn't exist");
		return new Vector2Int(0, 0);
	}

	public List<Tile> GetAdjacentTiles(Tile tile, int range, bool blockedByAthletes)
	{
		List<Tile> tiles = new List<Tile>();

		Vector2Int tilePosition = GetFieldIntForTile(tile);

		for (int i = range; i == range; i++)
		{
			int right = tilePosition.x + i;
			if (right < columns)
			{
				Tile potentialTile = field[right, tilePosition.y];
				if (!blockedByAthletes || !CheckForAthleteInTile(potentialTile))
					tiles.Add(potentialTile);
			}

			int left = tilePosition.x - i;
			if (left >= 0)
			{
				Tile potentialTile = field[left, tilePosition.y];
				if (!blockedByAthletes || !CheckForAthleteInTile(potentialTile))
					tiles.Add(potentialTile);
			}

			int up = tilePosition.y + i;
			if (up < rows)
			{
				Tile potentialTile = field[tilePosition.x, up];
				if (!blockedByAthletes || !CheckForAthleteInTile(potentialTile))
					tiles.Add(potentialTile);
			}

			int down = tilePosition.y - i;
			if (down >= 0)
			{
				Tile potentialTile = field[tilePosition.x, down];
				if (!blockedByAthletes || !CheckForAthleteInTile(potentialTile))
					tiles.Add(potentialTile);
			}
		}

		Debug.Log("NUm tiles " + tiles.Count);
		return tiles;
	}

	public List<Tile> GetAllTilesBetween(Tile startTile, Tile endTile)
	{
		List<Tile> tilesBetween = new List<Tile>();

		Vector2Int startTilePosition = GetFieldIntForTile(startTile);
		Vector2Int endTilePosition = GetFieldIntForTile(endTile);

		if (startTilePosition.x == endTilePosition.x)
		{
			int difference = endTilePosition.y - startTilePosition.y;

			if(difference > 0)
			{
				for(int i = startTilePosition.y + 1; i < endTilePosition.y; i++)
				{
					tilesBetween.Add(field[startTilePosition.x, i]);
				}
			}
			else if(difference < 0)
			{
				for (int i = startTilePosition.y - 1; i > endTilePosition.y; i--)
				{
					tilesBetween.Add(field[startTilePosition.x, i]);
				}
			}
		}
		else if(startTilePosition.y == endTilePosition.y)
		{
			int difference = endTilePosition.x - startTilePosition.x;

			if (difference > 0)
			{
				for (int i = startTilePosition.x + 1; i < endTilePosition.x; i++)
				{
					tilesBetween.Add(field[i, startTilePosition.y]);
				}
			}
			else if (difference < 0)
			{
				for (int i = startTilePosition.x - 1; i > endTilePosition.x; i--)
				{
					tilesBetween.Add(field[i, startTilePosition.y]);
				}
			}
		}
		else
		{
			Debug.Log("Error - start and end tiles have no matching row/column");
		}

		Debug.Log("Tiles between count " + tilesBetween.Count);
		return tilesBetween;
	}

	public bool CheckForAthleteInTile(Tile tile)
	{
		if (tile.occupier != null)
			return true;
		else
			return false;
	}

	public void SpawnBalls()
	{
		for(int i = 0; i < ballStartingPositions.Count; i++)
		{
			Ball newBall = new Ball(this);
			gameController.GenerateBallGameObject(newBall);
			newBall.SetBallSpawn(ballStartingPositions[i]);
		}
	}

	private bool playerTurn = true;
	private int turn = 0;
    public void NextTurn()
	{
		//foreach (Athlete athlete in athletesInPlay)
		//	athlete.PerformAction();

		if(playerTurn)
			playerTeam.PerformTurn();
		else
			opponentTeam.PerformTurn();

		turn++;
		playerTurn = !playerTurn;
	}

	private Athlete activeAthlete = null;
	private Athlete.Action activeAction = null;
	public void ActivateAthlete(Athlete athlete, Athlete.Action action)
	{
		activeAthlete = athlete;
		activeAction = action;
	}

	public void ResolveActiveAthleteAction(Tile tile)
	{
		activeAction.Invoke(tile);
	}

	public void TackleAthlete(Athlete tackler, Athlete tacklee)
	{
		if(tacklee.heldBall != null && tackler.heldBall == null)
		{
			tackler.PossessBall(tacklee.heldBall);
		}

		tacklee.MoveToTile(tacklee.GetNearestAdjacentTile());
	}
}
