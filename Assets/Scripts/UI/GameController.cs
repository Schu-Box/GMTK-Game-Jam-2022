using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TeamUIController playerTeamController;
    public TeamUIController opponentTeamController;
    public RuntimeData runtimeData = new RuntimeData();

    public Transform fieldGrid;
   
    public Sprite goalImage;

    public Transform ballGameObjectParent;
    public GameObject ballGameObjectPrefab;
    public GameObject diceGameObjectPrefab;
    public GameObject athleteCardGameObjectPrefab;
    public GameObject athleteGameObjectPrefab;


    public List<Sprite> diceFaceSprites;

    private void Start()
	{
        DeleteAllChidlren(ballGameObjectParent);

        StartCoroutine(DelayedStart());
	}

    private IEnumerator DelayedStart()
	{
        yield return new WaitForSeconds(0.5f);

        runtimeData.Setup(this);
	}

	public void SetFieldObjects()
	{
        int fieldGridInt = 0;
        for(int i = 0; i < runtimeData.rows; i++)
		{
            for(int j = 0; j < runtimeData.columns; j++)
			{
                Tile associatedTile = runtimeData.field[j, i];
                fieldGrid.GetChild(fieldGridInt).GetComponent<TileObject>().Setup(associatedTile);

                fieldGridInt++;
            }
        }
    }

    public void GenerateBallGameObject(Ball ball)
	{
        BallObject newBallObject = Instantiate(ballGameObjectPrefab, ballGameObjectParent).GetComponent<BallObject>();
        ball.ballGameObject = newBallObject;
	}

    public void DeleteAllChidlren(Transform parent)
	{
        for(int i = 0; i < parent.childCount; i++)
		{
            Destroy(parent.GetChild(i).gameObject);
		}
	}

    public void UserTriggerNextTurn()
	{
        runtimeData.NextTurn();
	}
}
