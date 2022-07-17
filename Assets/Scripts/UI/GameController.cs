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

    public static float animationSpeed_DiceSlot = 0.2f;
    public static float animationSpeed_DiceFail = 0.3f;
    public static float animationSpeed_DiceClear = 0.3f;
    public static float animationSpeed_Move = 0.3f;
    public static float animationSpeed_BallMove = 0.2f;
    public static float animationSpeed_BallPossession = 0.2f;
    public static float animationSpeed_BallReset = 0.3f;
    public static float animationSpeed_DiceRollGrow = 0.2f;
    public static float animationSpeed_DiceRollShow = 0.6f;
    public static float animationSpeed_DiceDestroy = 0.15f;
    public static float animationSpeed_GoalBall = 0.7f;
    public static float animationSpeed_GoalText = 0f; //this is nothing as of now

    public Color color_diceFail;

    private bool userInteractionAllowed = false;

    private void Start()
    {
        DeleteAllChidlren(ballGameObjectParent);

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);

        runtimeData.Setup(this);

        playerTeamController.scoreboardText.color = runtimeData.playerTeam.teamColor;
        opponentTeamController.scoreboardText.color = runtimeData.opponentTeam.teamColor;
    }

    public void SetFieldObjects()
    {
        int fieldGridInt = 0;
        for (int i = 0; i < runtimeData.rows; i++)
        {
            for (int j = 0; j < runtimeData.columns; j++)
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
        newBallObject.Setup(ball);
    }

    public void DeleteAllChidlren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public void UserTriggerNextTurn()
    {
        runtimeData.NextTurn();

        if (runtimeData.playerTurn)
        {
            AllowUserInteraction(true);
        }
    }

    public void AllowUserInteraction(bool allowed)
    {
        userInteractionAllowed = allowed;

        if (userInteractionAllowed)
        {
            foreach (Athlete athlete in runtimeData.playerTeam.athletesInPlay)
                athlete.UpdateDiceSlots();

            foreach (Athlete athlete in runtimeData.opponentTeam.athletesInPlay)
                athlete.UpdateDiceSlots();
        }
    }

    public void UserPlacedDiceInSlot(DiceObject diceObject, DiceSlotObject slotObject)
    {
        runtimeData.PlayDiceInDiceSlot(diceObject.dice, slotObject.diceSlot);

        Athlete athlete = slotObject.diceSlot.athlete;

        foreach (Tile tile in runtimeData.field)
        {
            tile.tileGameObject.Highlight(false);
        }

        List<Tile> optionalTiles = runtimeData.GetAdjacentTiles(athlete.currentTile, diceObject.dice.value);

        foreach (Tile tile in optionalTiles)
        {
            tile.tileGameObject.Highlight(true);
        }

        if (optionalTiles.Count == 0)
        {
            AddToAnimationQueue(() => DisplayFailedDice(diceObject, animationSpeed_DiceFail));
        }
    }

    public void DisplayFailedDice(DiceObject diceObject, float duration)
	{
        diceObject.DisplayFail(duration);

        CompleteQueueActionAfterDelay(duration);

        AwaitUserEndTurn();
    }

    public void UserClickedTile(Tile clickedTile)
	{
        foreach(Tile tile in runtimeData.field)
		{
            tile.tileGameObject.Highlight(false);
		}

        runtimeData.ResolveActiveAthleteAction(clickedTile);
	}

    public void AwaitUserEndTurn()
	{
        //Debug.Log(runtimeData.playerTeam.diceRolled.Count);

        if(runtimeData.playerTeam.diceRolled.Count == 0)
		{
            //Debug.Log("User ran out of dice, end turn");
            runtimeData.NextTurn();
		}            
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
        LeanTween.delayedCall(timer, () => CompleteQueueAction());    }

    private void CompleteQueueAction()
	{
        actionQueue.Remove(actionQueue[0]);
        queueRunning = false;

        if(actionQueue.Count == 0)
		{
            AllowUserInteraction(true);
		}
    }
}
