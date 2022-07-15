using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AthleteObject : MonoBehaviour
{
	//private  Image image;
	public void FlipImage()
	{
		transform.eulerAngles = new Vector3(0, 180, 0);
	}

    public void MoveToTileObject(TileObject tileObject)
	{
		LeanTween.move(gameObject, tileObject.transform.position, 0.3f);
	}
}
