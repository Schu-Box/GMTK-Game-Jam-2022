using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DiceSlotObject : MonoBehaviour, IDropHandler
{
	public DiceSlot diceSlot;

	public List<GameObject> diceImages;
	public TextMeshProUGUI descriptionText;

	public Transform diceHolder;

	private GameController gameController;

	private void Awake()
	{
		gameController = FindObjectOfType<GameController>();
	}

	public void Setup(DiceSlot newDiceSlot)
	{
		diceSlot = newDiceSlot;

		for(int i = 0; i < diceImages.Count; i++)
			diceImages[i].SetActive(false);

		for(int i = 0; i < diceSlot.allowedValues.Count; i++)
			diceImages[diceSlot.allowedValues[i] - 1].SetActive(true);

		descriptionText.text = diceSlot.description;

		diceSlot.gameObject = this;
	}

	public void Activate(bool active)
	{
		gameObject.SetActive(active);
	}

	public void OnDrop(PointerEventData pointerEventData)
	{
		DiceObject d = pointerEventData.pointerDrag.GetComponent<DiceObject>();
		if (d != null)
		{
			AttemptToDropDice(d);
		}
	}

	public void AttemptToDropDice(DiceObject diceObject)
	{
		int value = diceObject.dice.value;
		//Debug.Log("Dropped a " + value + " on DropZone ");

		if (diceSlot.allowedValues.Contains(value) && diceSlot.athlete.team == diceObject.dice.owner && diceSlot.heldDice == null)
		{
			//diceSlot.TriggerWithValue(value);

			diceObject.draggable.SetNewParent(diceHolder);

			gameController.UserPlacedDiceInSlot(diceObject, this);
		}
	}
}
