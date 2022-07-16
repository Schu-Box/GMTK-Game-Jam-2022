using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Athlete
{
    public string name;

	private Tile currentTile;

	public Team team;

	public AthleteObject athleteGameObject;

	public Ball heldBall;

	public List<DiceSlot> diceSlots = new List<DiceSlot>();

	public Athlete(Team assignedTeam)
	{
		team = assignedTeam;

		diceSlots.Add(new DiceSlot(this, MoveHorizontal, "Move Forward 1", new List<int> { 1, 2, 3 }, new List<ValueModifier> { ValueModifier.ClampedTo1 }));
		diceSlots.Add(new DiceSlot(this, MoveVertical, "Move Up 1" + '\n' + "on even," + '\n' + "Move Down 1" + '\n' + "on odd", new List<int> { 3, 4, 5, 6 }, new List<ValueModifier> { ValueModifier.ClampedTo1, ValueModifier.OddBecomesNegative }));
		//diceSlots.Add(new DiceSlot(this));
	}

	public void AssignToTile(Tile tile)
	{
		if (currentTile != null)
			currentTile.AthleteExited(this);

		currentTile = tile;

		tile.AthleteEntered(this);

		athleteGameObject.MoveToTileObject(currentTile.tileGameObject);
	}

	public void MoveHorizontal(int value)
	{
		int newColumn = team.runtimeData.GetFieldIntForTile(currentTile, true);

		if (team == team.runtimeData.playerTeam)
		{
			newColumn += value;
		}
		else
		{
			newColumn -= value;
		}

		newColumn = Mathf.Clamp(newColumn, 0, team.runtimeData.columns - 1);

		Tile newTile = team.runtimeData.field[newColumn, team.runtimeData.GetFieldIntForTile(currentTile, false)];
		AssignToTile(newTile);
	}

	public void MoveVertical(int value)
	{
		int newRow = team.runtimeData.GetFieldIntForTile(currentTile, false);

		newRow += value;

		newRow = Mathf.Clamp(newRow, 0, team.runtimeData.rows - 1);

		Tile newTile = team.runtimeData.field[team.runtimeData.GetFieldIntForTile(currentTile, true), newRow];
		AssignToTile(newTile);
	}

	public void PossessBall(Ball newBall)
	{
		heldBall = newBall;

		newBall.AssignToAthlete(this);

		//Assign image or something
	}
}
