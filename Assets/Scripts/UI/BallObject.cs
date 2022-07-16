using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallObject : MonoBehaviour
{
	private Image image;

	private GameController gameController;

	private void Start()
	{
		image = GetComponent<Image>();

		gameController = FindObjectOfType<GameController>();
	}
	public void MoveToTileObject(TileObject tileObject)
	{
		LeanTween.move(gameObject, tileObject.transform.position, 0.3f);
	}
	public void DisplayPosession(Athlete athlete)
	{
		//LeanTween.move(gameObject, athlete.athleteGameObject.transform.position, 0.2f).setOnComplete(() => CompletePossession(athlete));
		transform.SetParent(athlete.athleteGameObject.transform);
		transform.localPosition = Vector3.zero;
		image.color = athlete.team.teamColor;
	}

	public void DisplayReset(Vector3 spawnPosition)
	{
		image.color = Color.white;

		transform.SetParent(gameController.ballGameObjectParent);
		transform.position = spawnPosition;
		transform.localScale = Vector3.zero;

		LeanTween.scale(gameObject, Vector3.one, 0.5f);
	}
}
