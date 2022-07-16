using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileObject : MonoBehaviour
{
	public Tile tile;

	private Image image;
	private GameController gameController;

	public void Setup(Tile newTile)
	{
		tile = newTile;
		tile.tileGameObject = this;

		image = GetComponent<Image>();
		gameController = FindObjectOfType<GameController>();

		if (tile.tileType == TileType.Goal)
			image.sprite = gameController.goalImage;

		image.color = tile.owner.teamColor;
	}
}
