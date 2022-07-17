using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

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

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI titleText2;
    public GameObject scoreboard;

    public GameObject endTurnButton;
    public TextMeshProUGUI endTurnText;

    public TextMeshProUGUI turnsRemainingText;

    public List<Sprite> diceFaceSprites;

    public AudioClip diceTraySlideOn;
    public AudioClip diceTraySlideOff;
    public AudioClip diceRoll;
    public AudioClip diceSlotInserted;
    public AudioClip diceFail;
    public AudioClip tileSelected;
    public AudioClip movement;
    public AudioClip kick;
    public AudioClip possessionGained;
    public AudioClip tackle;
    public AudioClip diceRoll_Single;
    public AudioClip invalidDicePlacement;
    public AudioClip goalScored;

    public static float animationSpeed_matchIntro = 0.2f;
    public static float animationSpeed_startMatch = 0.3f;
    public static float animationSpeed_turnStart = 0.5f;
    public static float animationSpeed_turnEnd = 0.5f;
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
    public static float animationSpeed_GoalText = 0.6f; //this is nothing as of now
    public static float animationSpeed_EndMatch = 0.3f;

    public Color color_diceFail;

    private bool userInteractionAllowed = false;

    private AudioSource audioSource;

    private void Start()
    {
        DeleteAllChidlren(ballGameObjectParent);

        StartCoroutine(DelayedStart());

        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);

        runtimeData.Setup(this);

        playerTeamController.scoreboardText.color = runtimeData.playerTeam.teamColor;
        opponentTeamController.scoreboardText.color = runtimeData.opponentTeam.teamColor;

        playerTeamController.ChangeUserControl(); //Default player to player
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

    public void UserStartMatch()
	{
        Debug.Log("Starting");

        runtimeData.matchStarted = true;

        AddToAnimationQueue(() => DisplayMatchIntroCleanup(animationSpeed_matchIntro));
        AddToAnimationQueue(() => DisplayStartMatch(animationSpeed_startMatch));

		opponentTeamController.QueueDisplayTurnEnd(); //HACK to play dice audio at a proper time
		runtimeData.StartNextTurn();
	}

    private void DisplayMatchIntroCleanup(float duration)
	{
        LeanTween.value(titleText.gameObject, SetValueCallback, Color.white, Color.clear, duration).setEaseInCubic();
        //Shouldn't need repeat
        LeanTween.scale(playerTeamController.userToggleObject, Vector3.zero, duration).setEaseOutCubic();
        LeanTween.scale(opponentTeamController.userToggleObject, Vector3.zero, duration).setEaseOutCubic();

        CompleteQueueActionAfterDelay(duration);
    }

    public void SetValueCallback(Color value)
	{
        titleText.color = value;
        titleText2.color = value;
	}

    private void DisplayStartMatch(float duration)
	{
        LeanTween.scale(scoreboard, Vector3.one, animationSpeed_startMatch).setEaseOutCubic();
        LeanTween.scale(playerTeamController.scoreBoard, Vector3.one, animationSpeed_startMatch).setEaseOutCubic();
        LeanTween.scale(opponentTeamController.scoreBoard, Vector3.one, animationSpeed_startMatch).setEaseOutCubic();
        LeanTween.scale(turnsRemainingText.transform.parent.gameObject, Vector3.one, animationSpeed_startMatch).setEaseOutCubic();

        CompleteQueueActionAfterDelay(duration);
	}

    public void UserPressedEndTurnButton()
    {
        ClearInteractableTiles();

		if (!runtimeData.matchStarted)
		{
            UserStartMatch();
		}
        else if (runtimeData.matchEnded)
		{
            SceneManager.LoadScene(0);
		}
        else
        {
            runtimeData.EndTurn();

            if (runtimeData.playerTurn)
            {
                AwaitNextMove();
            }
            else
            {
                AwaitNextMove();
            }
        }
    }

    public void ActivateEndTurnButton(bool activate)
	{
        if (activate)
            LeanTween.scale(endTurnButton, Vector3.one, 0.2f);
        else
            LeanTween.scale(endTurnButton, Vector3.zero, 0.2f);
	}

    public void AwaitNextMove()
    {
        if(!runtimeData.matchStarted)
		{
            ActivateEndTurnButton(true);
		}
        else if (!runtimeData.matchEnded) //May be unneccsary now
        {
            foreach (Athlete athlete in runtimeData.GetActiveTeam().athletesInPlay)
                athlete.UpdateDiceSlots();

            if (runtimeData.GetActiveTeam().userControlled)
            {
                userInteractionAllowed = true;

                ActivateEndTurnButton(true);
                endTurnText.text = "End Turn";
            }
            else
            {
                userInteractionAllowed = false;

                ActivateEndTurnButton(false);

                ComputerAction computerAction = runtimeData.GetActiveTeam().DetermineNextMove();
                if (computerAction != null)
                {
                    runtimeData.PlayDiceInDiceSlot(computerAction.dice, computerAction.diceSlot);
                    runtimeData.ResolveActiveAthleteAction(computerAction.selectedTile);
                }
                else
                {
                    runtimeData.EndTurn();
                }
            }
        }
    }

    public void ClearInteractableTiles()
	{
        foreach (Tile tile in runtimeData.field)
        {
            tile.tileGameObject.Highlight(false);
        }
    }

    public void UserPlacedDiceInSlot(DiceObject diceObject, DiceSlotObject diceSlotObject)
	{
        runtimeData.PlayDiceInDiceSlot(diceObject.dice, diceSlotObject.diceSlot);
    }

    public void QueueDisplayDicePlacedInSlot(DiceObject diceObject, DiceSlotObject slotObject)
	{
        AddToAnimationQueue(() => DisplayDicePlacedInSlot(diceObject, slotObject, animationSpeed_DiceSlot));
	}
    private void DisplayDicePlacedInSlot(DiceObject diceObject, DiceSlotObject slotObject, float duration)
    {
        CompleteQueueActionAfterDelay(duration);

        PlayAudio(diceSlotInserted);

        diceObject.draggable.SetNewParent(slotObject.diceHolder);
        diceObject.draggable.ReturnToParent();

        Athlete athlete = slotObject.diceSlot.athlete;

		ClearInteractableTiles();
        List<Tile> optionalTiles = new List<Tile>();

        if (slotObject.diceSlot.actionType == ActionType.Juke)
            optionalTiles = runtimeData.GetValidDiagonals(athlete.currentTile);
        else
            optionalTiles = runtimeData.GetValidTiles(athlete.currentTile, diceObject.dice.value);

        if (slotObject.diceSlot.athlete.team.userControlled)
		{
			foreach (Tile tile in optionalTiles)
                tile.tileGameObject.Highlight(true);
		}

        if (optionalTiles.Count == 0)
            AddToAnimationQueue(() => DisplayFailedDice(diceObject, animationSpeed_DiceFail));
    }

    public void DisplayFailedDice(DiceObject diceObject, float duration)
	{
        PlayAudio(diceFail);

        diceObject.DisplayFail(duration);

        CompleteQueueActionAfterDelay(duration);

		CheckForNoDice();
	}

    public void UserClickedTile(Tile clickedTile)
	{
        PlayAudio(tileSelected);

        ClearInteractableTiles();

        runtimeData.ResolveActiveAthleteAction(clickedTile);
	}

    public void CheckForNoDice()
	{
        if(runtimeData.GetActiveTeam().diceRolled.Count == 0)
		{
            runtimeData.EndTurn();
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

        if (Input.GetKeyDown(KeyCode.R))
		{
            SceneManager.LoadScene(0);
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
            AwaitNextMove();
		}
    }

    public void QueueDisplayTackle()
	{
        AddToAnimationQueue(() => DisplayTackle(0f)); //This is just for sound purposes
	}
    private void DisplayTackle(float duration)
	{
        Debug.Log("Tackle audio");

        PlayAudio(tackle);

        CompleteQueueActionAfterDelay(duration);
	}

    public void DisplayEndTurn(float duration)
	{
        //PlayAudio(diceTraySlideOff);
        PlayAudio(diceRoll); //This would ideally be done per dice roll, but it always happens at start of turn so whatever

        LeanTween.scale(turnsRemainingText.gameObject, Vector3.zero, duration).setEaseInExpo();
	}

    public void DisplayStartTurn(float duration)
	{
        PlayAudio(diceTraySlideOn);

        turnsRemainingText.text = "Turns Remaining: " + (runtimeData.turnsPerMatch - runtimeData.turn + 1);
        LeanTween.scale(turnsRemainingText.gameObject, Vector3.one, duration).setEaseOutExpo();
    }

    public void QueueDisplayEndMatch()
	{
        AddToAnimationQueue(() => DisplayEndMatch(animationSpeed_EndMatch));
    }
    private void DisplayEndMatch(float duration)
	{
        ActivateEndTurnButton(true);
        endTurnText.text = "Rematch";

        turnsRemainingText.text = "Game Over";

        LeanTween.scale(turnsRemainingText.gameObject, Vector3.one, duration).setEaseOutElastic();

        CompleteQueueActionAfterDelay(duration);
    }


    public void PlayAudio(AudioClip clip, float volume = 1f)
	{
        audioSource.PlayOneShot(clip, volume);
	}
}
