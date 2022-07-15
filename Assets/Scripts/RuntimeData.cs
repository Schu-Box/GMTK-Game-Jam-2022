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

	public GameController gameController;

	public void Setup(GameController gc)
	{
		gameController = gc;

		playerTeam.Setup(this);
		opponentTeam.Setup(this);

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

				if(c >= columns/2)
					field[c, r].SetForTeam(opponentTeam);
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

	public int GetFieldIntForTile(Tile tile, bool horizontal)
	{
		for(int r = 0; r < rows; r++)
		{
			for(int c = 0; c < columns; c++)
			{
				if (field[c, r] == tile)
				{
					if (horizontal)
						return c;
					else
						return r;
				}
			}
		}

		Debug.Log("ERROR: Tile doesn't exist");
		return 0;
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
}
