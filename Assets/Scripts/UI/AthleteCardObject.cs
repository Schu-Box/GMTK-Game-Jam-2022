using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AthleteCardObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Athlete athlete;

    public TextMeshProUGUI athleteTitle;
    public Image athleteImage;
    public List<DiceSlotObject> diceSlotObjects;
    
    public void SetForAthlete(Athlete a)
	{
        athlete = a;

        athleteTitle.text = athlete.name;
        //athleteImage.sprite = athlete.sprite;

        for(int i = diceSlotObjects.Count - 1; i >= 0; i--)
		{
            if(i < athlete.diceSlots.Count)
            {
                diceSlotObjects[i].gameObject.SetActive(true);
                diceSlotObjects[i].Setup(athlete.diceSlots[i]);
			}
			else
			{
                diceSlotObjects[i].gameObject.SetActive(false);
			}
		}
	}

    public void OnPointerExit(PointerEventData eventData)
	{
        if(eventData.pointerCurrentRaycast.gameObject != null)
		{
            if (eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                Debug.Log("IS CHILD");
                return;
            }
        }

        Debug.Log("Exited");

        athlete.athleteGameObject.Highlight(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entered");
        athlete.athleteGameObject.Highlight(true);
    }
}
