using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public static float animationSpeed_Move = 0.3f;
    public static float animationSpeed_BallPossession = 0.2f;
    public static float animationSpeed_BallReset = 0.3f;
    public static float animationSpeed_DiceRollGrow = 0.2f;
    public static float animationSpeed_DiceRollShow = 0.6f;
    public static float animationSpeed_DiceDestroy = 0.15f;

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

    public void AwaitUserTileSelection(Tile startingTile, int range, bool blockedByAthletes)
	{
        List<Tile> optionalTiles = runtimeData.GetAdjacentTiles(startingTile, range, blockedByAthletes);

        foreach (Tile tile in optionalTiles)
		{
            Debug.Log(tile.tileGameObject.name);
            tile.tileGameObject.Highlight(true);
        }

        Debug.Log("optionalTiles are " + optionalTiles.Count);
	}

    public void UserClickedTile(Tile clickedTile)
	{
        foreach(Tile tile in runtimeData.field)
		{
            tile.tileGameObject.Highlight(false);
		}

        runtimeData.ResolveActiveAthleteAction(clickedTile);
	}

    public List<UnityAction> actionQueue = new List<UnityAction>();

    public void AddToAnimationQueue(UnityAction nextAction, bool immediate = false)
    {
        if (immediate)
            actionQueue.Insert(0, nextAction);
        else
            actionQueue.Add(nextAction);
    }

    private bool queueRunning = false;
    private void Update()
    {
        if (!queueRunning && actionQueue.Count > 0)
        {
            //Debug.Log("Q " + queueRunning + " with " + actionQueue.Count);

            StartAction(actionQueue[0]);
        }
    }

    public void StartAction(UnityAction action)
    {
        queueRunning = true;

        action();
    }

    public void CompleteQueueActionAfterDelay(float timer)
    {
        LeanTween.delayedCall(timer, () => CompleteQueueAction());
    }

    private void CompleteQueueAction()
	{
        actionQueue.Remove(actionQueue[0]);
        queueRunning = false;
    }
}
