using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSlot
{
	public Athlete athlete;
	public Athlete.Action action;
	public string description;

	public List<int> allowedValues = new List<int>();

	public bool ballRequired;
	public bool noBallRequired;

	public bool active = true;
	public Dice heldDice = null;

	public DiceSlotObject gameObject; //I kinda like this but could be confusing?

	public DiceSlot(Athlete newAthlete, Athlete.Action newAction, string newDescription, List<int> newAllowedValues, bool ballReq = false, bool noBallReq = false)
	{
		athlete = newAthlete;
		action = newAction;
		description = newDescription;

		for (int i = 0; i < newAllowedValues.Count; i++)
			allowedValues.Add(newAllowedValues[i]);

		ballRequired = ballReq;
		noBallRequired = noBallReq;
	}

	public void Activate(bool activate)
	{
		active = activate;

		gameObject.Activate(active);
	}

	public void FillWithDice(Dice dice)
	{
		heldDice = dice;
	}

	public void ClearDice()
	{
		if(heldDice != null)
		{
			heldDice.diceGameObject.DisplayCleared();
			heldDice = null;
		}
	}

	//public void TriggerWithValue(int value)
	//{
	//	Debug.Log("Success");

	//	int passedValue = value;

	//	if (valueModifiers.Contains(ValueModifier.ClampedTo1))
	//		passedValue = 1;

	//	if (valueModifiers.Contains(ValueModifier.OddBecomesNegative) && value % 2 == 1)
	//		passedValue = -passedValue;

	//	action.Invoke(passedValue);
	//}
}
