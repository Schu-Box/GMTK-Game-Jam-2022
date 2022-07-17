using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSlot
{
	[HideInInspector] public Athlete athlete;
	[HideInInspector] public Athlete.Action action;

	public ActionType actionType;
	public string description;

	public List<int> allowedValues = new List<int>();

	public bool ballRequired;
	public bool noBallRequired;

	[HideInInspector] public bool active = true;
	[HideInInspector] public Dice heldDice = null;

	[HideInInspector] public DiceSlotObject gameObject; //I kinda like this but could be confusing?

	//public DiceSlot(Athlete newAthlete, Athlete.Action newAction, string newDescription, List<int> newAllowedValues, bool ballReq = false, bool noBallReq = false)
	//{
	//	athlete = newAthlete;
	//	action = newAction;
	//	description = newDescription;

	//	for (int i = 0; i < newAllowedValues.Count; i++)
	//		allowedValues.Add(newAllowedValues[i]);

	//	ballRequired = ballReq;
	//	noBallRequired = noBallReq;
	//}

	public DiceSlot(DiceSlot template, Athlete initAthlete)
	{
		actionType = template.actionType;
		description = template.description;

		for (int i = 0; i < template.allowedValues.Count; i++)
			allowedValues.Add(template.allowedValues[i]);

		ballRequired = template.ballRequired;
		noBallRequired = template.noBallRequired;

		athlete = initAthlete;
		switch (actionType)
		{
			default:
			case ActionType.Move:
				action = athlete.MoveToTile;
				break;
			case ActionType.Kick:
				action = athlete.KickToTile;
				break;
		}
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
