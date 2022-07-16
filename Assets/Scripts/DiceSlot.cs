using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ValueModifier
{
	ClampedTo1,
	OddBecomesNegative
}

public class DiceSlot
{
	public Athlete athlete;

	public List<int> allowedValues = new List<int>();
	public List<ValueModifier> valueModifiers = new List<ValueModifier>();

	public string description;

	public delegate void Action(int value);
	Action action;

	public DiceSlot(Athlete newAthlete, Action newAction, string newDescription, List<int> newAllowedValues, List<ValueModifier> newValueModifiers)
	{
		athlete = newAthlete;

		for (int i = 0; i < newAllowedValues.Count; i++)
			allowedValues.Add(newAllowedValues[i]);

		for (int i = 0; i < newValueModifiers.Count; i++)
			valueModifiers.Add(newValueModifiers[i]);

		description = newDescription;

		action = newAction;
	}

	public void TriggerWithValue(int value)
	{
		Debug.Log("Success");

		int passedValue = value;

		if (valueModifiers.Contains(ValueModifier.ClampedTo1))
			passedValue = 1;

		if (valueModifiers.Contains(ValueModifier.OddBecomesNegative) && value % 2 == 1)
			passedValue = -passedValue;

		action.Invoke(passedValue);
	}
}
