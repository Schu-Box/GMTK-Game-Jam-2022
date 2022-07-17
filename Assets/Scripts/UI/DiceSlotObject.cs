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

	public Image lockedImage;

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

	public void Lock(bool isLocked)
	{
		Debug.Log("Locking " + isLocked);

		if (isLocked)
		{
			lockedImage.gameObject.SetActive(true);
			lockedImage.transform.localScale = Vector3.zero;
			LeanTween.scale(lockedImage.gameObject, Vector3.one, 0.2f).setEaseOutBack();
		}
		else
		{
			LeanTween.scale(lockedImage.gameObject, Vector3.zero, 0.2f).setEaseInExpo().setOnComplete(() => lockedImage.gameObject.SetActive(false));
		}
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

		if (!diceSlot.locked && diceSlot.allowedValues.Contains(value) && diceSlot.athlete.team == diceObject.dice.owner && diceSlot.heldDice == null)
		{
			gameController.UserPlacedDiceInSlot(diceObject, this);
		}
		else
		{
			gameController.PlayAudio(gameController.invalidDicePlacement);
		}
	}
}
