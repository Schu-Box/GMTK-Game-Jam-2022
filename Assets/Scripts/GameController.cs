using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public RuntimeData runtimeData = new RuntimeData();

    public Transform fieldGrid;
   
    public Sprite goalImage;

    public Transform athleteGameObjectParent;
    public GameObject athleteGameObjectPrefab;
    public Transform ballGameObjectParent;
    public GameObject ballGameObjectPrefab;

	private void Start()
	{
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

    public void GenerateAthleteGameObject(Athlete athlete)
	{
        AthleteObject newAthleteObject = Instantiate(athleteGameObjectPrefab, athleteGameObjectParent).GetComponent<AthleteObject>();
        athlete.athleteGameObject = newAthleteObject;

        if (athlete.team == runtimeData.opponentTeam)
            newAthleteObject.FlipImage();
	}

    public void GenerateBallGameObject(Ball ball)
	{
        BallObject newBallObject = Instantiate(ballGameObjectPrefab, ballGameObjectParent).GetComponent<BallObject>();
        ball.ballGameObject = newBallObject;
	}

    public void UserTriggerNextTurn()
	{
        runtimeData.NextTurn();
	}
}
