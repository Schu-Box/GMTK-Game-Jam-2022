using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{

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
		Debug.Log("Dropped a " + diceObject.dice.value + " on DropZone ");
	}
}
