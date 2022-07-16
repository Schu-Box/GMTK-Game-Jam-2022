using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
	public int value;
    public Dice()
	{
		value = Random.Range(1, 7);
	}
}
