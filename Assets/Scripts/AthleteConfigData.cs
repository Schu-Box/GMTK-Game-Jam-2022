using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ActionType
{
	Move,
	Kick
}

[CreateAssetMenu(menuName = "Athlete Config")]
public class AthleteConfigData : SerializedScriptableObject
{
	public string name;
	public Sprite sprite;

	public List<DiceSlot> diceSlots;
}
