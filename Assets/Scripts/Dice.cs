using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
	public int value;
	public Team owner;

	public DiceObject diceGameObject;

    public Dice(Team teamOwner)
	{
		value = Random.Range(1, 7);
		owner = teamOwner;
	}
}
