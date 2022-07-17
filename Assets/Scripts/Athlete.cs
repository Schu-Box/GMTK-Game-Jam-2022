using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Athlete
{
    public string name;

	public Tile currentTile;

	public Team team;

	public AthleteObject athleteGameObject;

	public Ball heldBall;

	public List<DiceSlot> diceSlots = new List<DiceSlot>();

	public delegate void Action(Tile tile);

	private List<Tile> movementQueue = new List<Tile>();
	//Action action;

	public Athlete(Team assignedTeam)
	{
		team = assignedTeam;

		diceSlots.Add(new DiceSlot(this, MoveToTile, "Move", new List<int> { 1, 2, 3, 4, 5, 6 }));
		diceSlots.Add(new DiceSlot(this, KickToTile, "Kick", new List<int> { 1, 2, 3, 4, 5, 6 }, true));
		//diceSlots.Add(new DiceSlot(this, MoveHorizontal, "Move Forward 1", new List<int> { 1, 2, 3 }, new List<ValueModifier> { ValueModifier.ClampedTo1 }));
		//diceSlots.Add(new DiceSlot(this, MoveVertical, "Move Up 1" + '\n' + "on even," + '\n' + "Move Down 1" + '\n' + "on odd", new List<int> { 3, 4, 5, 6 }, new List<ValueModifier> { ValueModifier.ClampedTo1, ValueModifier.OddBecomesNegative }));
		//diceSlots.Add(new DiceSlot(this));
	}
	public void MoveToTile(Tile newTile)
	{
		if(currentTile != null)
		{
			List<Tile> allTilesBetween = team.runtimeData.GetAllTilesBetween(currentTile, newTile);

			foreach (Tile tile in allTilesBetween)
				AddToMovementQueue(tile);
		}

		AddToMovementQueue(newTile);

		BeginMovement();
	}

	public void KickToTile(Tile newTile)
	{
		Ball kickedBall = heldBall;

		//kickedBall.currentTile_NoPossession = currentTile; //Hacky solution be we jamming
		kickedBall.AssignPossessor(null);

		List<Tile> allTilesBetween = team.runtimeData.GetAllTilesBetween(currentTile, newTile);

		foreach (Tile tile in allTilesBetween)
			kickedBall.AddToMovementQueue(tile);

		kickedBall.AddToMovementQueue(newTile);

		kickedBall.BeginMovement();
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
		while(movementQueue.Count > 0)
		{
			AttemptToEnterTile(movementQueue[0]);
		}
	}

	public void AttemptToEnterTile(Tile tile)
	{
		if(tile.GetOccupier() != null)
		{
			//roll dice
			Athlete defender = tile.GetOccupier();

			Dice intruderRoll = new Dice(team);
			Dice defenderRoll = new Dice(defender.team);

			Debug.Log("Roll " + intruderRoll.value + " vs " + defenderRoll.value);
			athleteGameObject.QueueDisplayRoll(intruderRoll);
			defender.athleteGameObject.QueueDisplayRoll(defenderRoll);

			athleteGameObject.QueueDisplayDestroyDice();
			defender.athleteGameObject.QueueDisplayDestroyDice();

			if (defenderRoll.value > intruderRoll.value)
			{
				AbortMovementQueue();
			}
			else
			{
				team.runtimeData.TackleAthlete(this, defender);
				
				CompleteMovement(tile);
			}
		}
		else
		{
			CompleteMovement(tile);
		}
	}

	public void CompleteMovement(Tile tile)
	{
		if (currentTile != null)
			currentTile.AthleteExited(this);

		currentTile = tile;

		athleteGameObject.QueueDisplayMovement(tile);

		tile.AthleteEntered(this);

		movementQueue.Remove(tile);
	}

	public Tile GetNearestAdjacentTile()
	{
		Vector2Int currentVector2 = team.runtimeData.GetFieldIntForTile(currentTile);

		if (currentVector2.y - 1 >= 0)
		{
			Tile downTile = team.runtimeData.field[currentVector2.x, currentVector2.y - 1];
			if (downTile.GetOccupier() == null)
				return downTile;
		}
		
		if(currentVector2.y + 1 < team.runtimeData.rows)
		{
			Tile upTile = team.runtimeData.field[currentVector2.x, currentVector2.y + 1];
			if (upTile.GetOccupier() == null)
				return upTile;
		}

		if (currentVector2.x - 1 >= 0)
		{
			Tile leftTile = team.runtimeData.field[currentVector2.x - 1, currentVector2.y];
			if (leftTile.GetOccupier() == null)
				return leftTile;
		}

		if (currentVector2.x + 1 < team.runtimeData.columns)
		{
			Tile rightTile = team.runtimeData.field[currentVector2.x + 1, currentVector2.y];
			if (rightTile.GetOccupier() == null)
				return rightTile;
		}

		//TODO: Resolve this situation
		Debug.Log("ERROR: All adjacent spaces are filled. Problem time");
		return null;
	}

	//public void MoveHorizontal(int value)
	//{
	//	int newColumn = team.runtimeData.GetFieldIntForTile(currentTile, true);

	//	if (team == team.runtimeData.playerTeam)
	//	{
	//		newColumn += value;
	//	}
	//	else
	//	{
	//		newColumn -= value;
	//	}

	//	newColumn = Mathf.Clamp(newColumn, 0, team.runtimeData.columns - 1);

	//	Tile newTile = team.runtimeData.field[newColumn, team.runtimeData.GetFieldIntForTile(currentTile, false)];
	//	//AssignToTile(newTile);
	//}

	//public void MoveVertical(int value)
	//{
	//	int newRow = team.runtimeData.GetFieldIntForTile(currentTile, false);

	//	newRow += value;

	//	newRow = Mathf.Clamp(newRow, 0, team.runtimeData.rows - 1);

	//	Tile newTile = team.runtimeData.field[team.runtimeData.GetFieldIntForTile(currentTile, true), newRow];
	//	//AssignToTile(newTile);
	//}

	public void PossessBall(Ball newBall)
	{
		heldBall = newBall;

		newBall.AssignPossessor(this);
	}

	public void UpdateDiceSlots()
	{
		foreach(DiceSlot diceSlot in diceSlots)
		{
			diceSlot.Activate(true);

			if (diceSlot.ballRequired && heldBall == null)
				diceSlot.Activate(false);

			if (diceSlot.noBallRequired && heldBall != null)
				diceSlot.Activate(false);
		}
	}
}
