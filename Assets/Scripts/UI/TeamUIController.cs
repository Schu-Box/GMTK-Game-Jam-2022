using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamUIController : MonoBehaviour
{
    public bool leftSide = true;
    public GameObject userToggleObject;
    public Image userToggleImage;
    public TextMeshProUGUI userToggleText;

    public Transform athleteGameObjectParent;
    public Transform athleteCardGameObjectParent;

    public Transform diceGameObjectParent;

    public Transform teamTurnParent;
    private Vector3 teamTurnParentOnPosition;
    private Vector3 teamTurnParentOffPosition;

    public GameObject scoreBoard;
    public TextMeshProUGUI scoreboardText;
    private float scoreBoardOffset = 20f;
    private float scoreBoardScaleIncrease = 1.2f;
    private Color defaultUserToggleImageColor;

    private GameController gameController;


	private void Start()
	{
        defaultUserToggleImageColor = userToggleImage.color;

        gameController = FindObjectOfType<GameController>();
        gameController.DeleteAllChidlren(athleteGameObjectParent);
        gameController.DeleteAllChidlren(athleteCardGameObjectParent);
        gameController.DeleteAllChidlren(diceGameObjectParent);

        teamTurnParentOnPosition = teamTurnParent.transform.localPosition;
        teamTurnParentOffPosition = teamTurnParentOnPosition;
        teamTurnParentOffPosition.x = teamTurnParentOnPosition.x * 1.5f;

        teamTurnParent.transform.localPosition = teamTurnParentOffPosition;
    }

    public void GenerateAthleteGameObject(Athlete athlete)
    {
        AthleteCardObject newAthleteCardObject = Instantiate(gameController.athleteCardGameObjectPrefab, athleteCardGameObjectParent).GetComponent<AthleteCardObject>();
        newAthleteCardObject.SetForAthlete(athlete);

        AthleteObject newAthleteObject = Instantiate(gameController.athleteGameObjectPrefab, athleteGameObjectParent).GetComponent<AthleteObject>();
        newAthleteObject.SetForAthlete(athlete);
        athlete.athleteGameObject = newAthleteObject;

        if (athlete.team == gameController.runtimeData.opponentTeam)
            newAthleteObject.FlipImage();
    }

    public void GenerateDiceGameObject(Dice dice)
	{
        DiceObject newDiceObject = Instantiate(gameController.diceGameObjectPrefab, diceGameObjectParent).GetComponent<DiceObject>();
        newDiceObject.Setup(dice);
		dice.diceGameObject = newDiceObject;
	}

    public void QueueDisplayScore(int newScore)
	{
        gameController.AddToAnimationQueue(() => DisplayScore(newScore, GameController.animationSpeed_GoalText));
	}
    private void DisplayScore(int newScore, float duration)
	{
        scoreboardText.text = newScore.ToString();

        gameController.PlayAudio(gameController.goalScored);

        Vector3 scoreBoardStartPosition = scoreBoard.transform.position;
        Vector3 newScoreBoardPosition = scoreBoardStartPosition;
        newScoreBoardPosition.y += scoreBoardOffset;
        LeanTween.move(scoreBoard, newScoreBoardPosition, duration / 2f).setEaseInCubic().setOnComplete(() =>
        {
                LeanTween.move(scoreBoard, scoreBoardStartPosition, duration / 2f).setEaseOutBack();
                
        });

        Vector3 newScoreBoardScale = scoreBoard.transform.localScale * scoreBoardScaleIncrease;
        LeanTween.scale(scoreBoard, newScoreBoardScale, duration / 2f).setEaseOutExpo().setOnComplete(() =>
        {
            LeanTween.scale(scoreBoard, Vector3.one, duration / 2f).setEaseInCubic();
        });

        gameController.CompleteQueueActionAfterDelay(duration);
	}

    public void QueueDisplayTurnStart()
	{
        gameController.AddToAnimationQueue(() => DisplayTurnStart(GameController.animationSpeed_turnStart));
	}
    private void DisplayTurnStart(float duration)
	{
        gameController.DisplayStartTurn(duration);

        LeanTween.moveLocal(teamTurnParent.gameObject, teamTurnParentOnPosition, duration).setEaseOutBack();

        gameController.CompleteQueueActionAfterDelay(duration);
    }

    public void QueueDisplayTurnEnd()
    {
        gameController.AddToAnimationQueue(() => DisplayTurnEnd(GameController.animationSpeed_turnEnd));
    }
    private void DisplayTurnEnd(float duration)
    {
        gameController.DisplayEndTurn(duration);

        LeanTween.moveLocal(teamTurnParent.gameObject, teamTurnParentOffPosition, duration).setEaseInBack();

        gameController.CompleteQueueActionAfterDelay(duration);
    }

    public void ChangeUserControl()
    {
        Debug.Log("Changing?");

        Team team;
        if (leftSide) //HACK - should be assigned in Setup
        {
            team = gameController.runtimeData.playerTeam;
        }
        else
        {
            team = gameController.runtimeData.opponentTeam;
        }

        team.userControlled = !team.userControlled;

        if (team.userControlled)
        {
            userToggleText.text = "Player";
            userToggleImage.color = team.teamColor;
        }
        else
        {
            userToggleText.text = "Computer";
            userToggleImage.color = defaultUserToggleImageColor;
        }
    }
}
