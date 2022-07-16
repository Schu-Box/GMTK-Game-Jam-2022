using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceObject : MonoBehaviour
{
    public Image diceImage;

	public Dice dice;

    public void Setup(Dice newDice)
	{
		dice = newDice;

		diceImage.sprite = FindObjectOfType<GameController>().diceFaceSprites[dice.value - 1];
	}
}
