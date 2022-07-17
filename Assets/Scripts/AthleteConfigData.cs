using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public enum ActionType
{
	Move,
	Kick,
	Juke
}

[CreateAssetMenu(menuName = "Athlete Config")]
public class AthleteConfigData : SerializedScriptableObject
{
	public string name;
	public Sprite sprite;

	public List<DiceSlot> diceSlots;
}
