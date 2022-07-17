using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileObject : MonoBehaviour, IPointerClickHandler
{
	public Tile tile;

	public Image borderImage;
	public Image fillImage;

	public GameObject highlightPulse;

	//private Image image;
	private GameController gameController;

	private bool clickable = false;

	public void Setup(Tile newTile)
	{
		tile = newTile;
		tile.tileGameObject = this;

		//image = GetComponent<Image>();
		gameController = FindObjectOfType<GameController>();

		if (tile.tileType == TileType.Goal)
			fillImage.sprite = gameController.goalImage;

		if (tile.owner != null)
		{
			fillImage.color = tile.owner.teamColor;

			//if (tile.owner == gameController.runtimeData.playerTeam)
			//	transform.localEulerAngles = new Vector3(0, 0, 90);
			//else
			//	transform.localEulerAngles = new Vector3(0, 0, 270);
		}
		else
			fillImage.color = Color.gray;
	}

	public void Highlight(bool enabled)
	{
		if (enabled)
		{
			clickable = true;
			//borderImage.color = Color.yellow;
			highlightPulse.SetActive(true);
		}
		else
		{
			clickable = false;
			//borderImage.color = Color.white;
			highlightPulse.SetActive(false);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(clickable)
		{
			gameController.UserClickedTile(tile);
		}
	}
}
