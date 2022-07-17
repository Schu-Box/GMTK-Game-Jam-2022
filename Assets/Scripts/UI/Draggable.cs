using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo;
    private Vector3 positionToReturnTo;

    private CanvasGroup canvasGroup;

    //private GameController gameController;

    private void Start()
    {
        //gameController = FindObjectOfType<GameController>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
		if (GetDraggability())
		{
			positionToReturnTo = transform.position;
			parentToReturnTo = transform.parent;

            canvasGroup.blocksRaycasts = false;
		}
	}

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (GetDraggability())
        {
            Vector3 newPosition = transform.position;

            newPosition.x += pointerEventData.delta.x;
            newPosition.y += pointerEventData.delta.y;

            transform.position = newPosition;
        }
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if (GetDraggability())
        {
            ReturnToParent();

            canvasGroup.blocksRaycasts = true;
        }
    }

    public void SetNewParent(Transform newParent)
    {
        transform.SetParent(newParent);
        parentToReturnTo = newParent;
        positionToReturnTo = newParent.transform.position;

    }

    public void ReturnToParent()
    {
        transform.SetParent(parentToReturnTo);
        //transform.position = positionToReturnTo;

        LeanTween.move(gameObject, positionToReturnTo, GameController.animationSpeed_DiceSlot).setEaseInCubic();
    }

    private bool GetDraggability()
    {
        return true;
    }
}
