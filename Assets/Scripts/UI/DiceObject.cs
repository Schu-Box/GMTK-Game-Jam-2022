using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceObject : MonoBehaviour
{
    public Image diceImage;

	public Dice dice;

	public Draggable draggable;

	private GameController gameController;

    public void Setup(Dice newDice)
	{
		gameController = FindObjectOfType<GameController>();
		draggable = GetComponent<Draggable>();

		dice = newDice;

		diceImage.sprite = gameController.diceFaceSprites[dice.value - 1];
	}

	public void DisplayFail(float duration)
	{
		LeanTween.value(gameObject, UpdateColor, diceImage.color, gameController.color_diceFail, duration);
	}

	private void UpdateColor(Color value)
	{
		diceImage.color = value;
	}

	public void DisplayCleared()
	{
		LeanTween.scale(gameObject, Vector3.zero, GameController.animationSpeed_DiceClear).setEaseInCubic();
	}
}
