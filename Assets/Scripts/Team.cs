using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
	public Color teamColor;

	public List<Vector2Int> startingPositions;
	public List<AthleteConfigData> athleteConfigs;

	[HideInInspector] public RuntimeData runtimeData;
	[HideInInspector] public TeamUIController teamController;

	public List<Athlete> athletesInPlay = new List<Athlete>();
	public List<Dice> diceRolled = new List<Dice>();

	public bool userControlled = true;

	private int numDicePerTurn = 5;

	private int score = 0;

	public void Setup(RuntimeData data, TeamUIController teamUIController)
	{
		runtimeData = data;
		teamController = teamUIController;

		for (int i = 0; i < athleteConfigs.Count && i < startingPositions.Count; i++)
		{
			AddAthleteToRoster(new Athlete(athleteConfigs[i], this));
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
			athletesInPlay[i].MoveToTile(runtimeData.field[startingPosition.x, startingPosition.y]);
		}
	}

	public void StartTurn()
	{
		teamController.QueueDisplayTurnStart();

		RollDice();

		foreach(Athlete athlete in athletesInPlay)
		{
			foreach(DiceSlot diceSlot in athlete.diceSlots)
			{
				diceSlot.ClearDice();
			}
		}

		//foreach (Athlete athlete in athletesInPlay)
		//	athlete.PerformAction();
	}

	public void RollDice()
	{
		ClearDice();

		for(int i= 0; i < numDicePerTurn; i++)
		{
			Dice newDice = new Dice(this);
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
		//Debug.Log("GOAL by " + scorer.name);
		score++;

		teamController.QueueDisplayScore(score);

		ball.ballGameObject.QueueDisplayScored();

		ball.ResetBall();
	}

	public ComputerAction DetermineNextMove()
	{
		Debug.Log("COMPUTING...");

		List<ComputerAction> possibleComputerActions = new List<ComputerAction>();

		for (int d = 0; d < diceRolled.Count; d++)
		{
			Dice dice = diceRolled[d];

			for (int a = 0; a < athletesInPlay.Count; a++)
			{
				Athlete athlete = athletesInPlay[a];
				for (int s = 0; s < athlete.diceSlots.Count; s++)
				{
					DiceSlot diceSlot = athlete.diceSlots[s];
					if (!diceSlot.locked && diceSlot.heldDice == null && diceSlot.allowedValues.Contains(dice.value))
					{
						List<Tile> optionalTiles = new List<Tile>();

						if (diceSlot.actionType == ActionType.Juke)
							optionalTiles = runtimeData.GetValidDiagonals(athlete.currentTile);
						else
							optionalTiles = runtimeData.GetValidTiles(athlete.currentTile, dice.value);

						for (int t = 0; t < optionalTiles.Count; t++)
						{
							Tile optionalTile = optionalTiles[t];

							bool canScore = false;
							if (athlete.heldBall != null && (optionalTile.tileType == TileType.Goal && optionalTile.owner != athlete.team))
							{
								//TODO: Technically if kicking the athlete wouldn't know if there's a defender
								canScore = true;
							}
							
							bool canPossess = false; 
							if (optionalTile.ContainsBall())
								canPossess = true;

							List<Tile> tilesBetween = runtimeData.GetAllTilesBetween(athlete.currentTile, optionalTile);
							for (int i = 0; i < tilesBetween.Count; i++)
							{
								if (tilesBetween[i].ContainsBall())
									canPossess = true;
							}

							possibleComputerActions.Add(new ComputerAction(dice, diceSlot, optionalTile, canScore, canPossess));
						}
					}
				}
			}
		}

		List<ComputerAction> scoringActions = new List<ComputerAction>();
		for(int i = 0; i < possibleComputerActions.Count; i++)
		{
			if (possibleComputerActions[i].canScore)
				scoringActions.Add(possibleComputerActions[i]);
		}

		if (scoringActions.Count > 0)
			return scoringActions[Random.Range(0, scoringActions.Count)];

		List<ComputerAction> possessingActions = new List<ComputerAction>();
		for (int i = 0; i < possibleComputerActions.Count; i++)
		{
			if (possibleComputerActions[i].canPossess)
				possessingActions.Add(possibleComputerActions[i]);
		}

		if (possessingActions.Count > 0)
			return possessingActions[Random.Range(0, possessingActions.Count)];

		Debug.Log("No possession or scores possible, might as well do something random");
		
		if(possibleComputerActions.Count > 0)
			return possibleComputerActions[Random.Range(0, possibleComputerActions.Count)];

		Debug.Log("ERROR: Computer had no possible actions");
		return null;
	}
}

public class ComputerAction 
{
	public Dice dice;
	public DiceSlot diceSlot;
	public Tile selectedTile;
	public bool canScore;
	public bool canPossess;

	public ComputerAction(Dice d, DiceSlot s, Tile t, bool scorable, bool possessable)
	{
		dice = d;
		diceSlot = s;
		selectedTile = t;
		canScore = scorable;
		canPossess = possessable;
	}
}
