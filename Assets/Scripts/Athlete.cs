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

	public Athlete(Team assignedTeam)
	{
		team = assignedTeam;
	}

	public void AssignToTile(Tile tile)
	{
		if (currentTile != null)
			currentTile.AthleteExited(this);

		currentTile = tile;

		tile.AthleteEntered(this);

		athleteGameObject.MoveToTileObject(currentTile.tileGameObject);
	}

    public void PerformAction()
	{
		int rando = Random.Range(0, 6);
		switch (rando)
		{
			case 0:
			case 4:
			case 5:
				MoveHorizontal(true);
				break;
			case 1:
				MoveHorizontal(false);
				break;
			case 2:
				MoveVertical(true);
				break;
			case 3:
				MoveVertical(false);
				break;
		}
	}

	public void MoveHorizontal(bool towardsGoal)
	{
		int newColumn = team.runtimeData.GetFieldIntForTile(currentTile, true);

		if (team == team.runtimeData.playerTeam)
		{
			if (towardsGoal)
				newColumn++;
			else
				newColumn--;
		}
		else
		{
			if (towardsGoal)
				newColumn--;
			else
				newColumn++;
		}

		newColumn = Mathf.Clamp(newColumn, 0, team.runtimeData.columns - 1);

		Tile newTile = team.runtimeData.field[newColumn, team.runtimeData.GetFieldIntForTile(currentTile, false)];
		AssignToTile(newTile);
	}

	public void MoveVertical(bool up)
	{
		int newRow = team.runtimeData.GetFieldIntForTile(currentTile, false);

		if (up)
			newRow++;
		else
			newRow--;

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
