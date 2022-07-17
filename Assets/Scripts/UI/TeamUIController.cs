using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamUIController : MonoBehaviour
{
    public Transform athleteGameObjectParent;
    public Transform athleteCardGameObjectParent;

    public Transform diceGameObjectParent;

    public Transform teamTurnParent;
    private Vector3 teamTurnParentOnPosition;
    private Vector3 teamTurnParentOffPosition;

    public TextMeshProUGUI scoreboardText;

    private GameController gameController;

	private void Start()
	{
        gameController = FindObjectOfType<GameController>();
        gameController.DeleteAllChidlren(athleteGameObjectParent);
        gameController.DeleteAllChidlren(athleteCardGameObjectParent);
        gameController.DeleteAllChidlren(diceGameObjectParent);

        teamTurnParentOnPosition = teamTurnParent.transform.localPosition;
        teamTurnParentOffPosition = teamTurnParentOnPosition;
        teamTurnParentOffPosition.x = teamTurnParentOnPosition.x * 1.5f;

        Debug.Log(teamTurnParent.transform.localPosition);

        teamTurnParent.transform.localPosition = teamTurnParentOffPosition;
    }

    public void GenerateAthleteGameObject(Athlete athlete)
    {
        AthleteCardObject newAthleteCardObject = Instantiate(gameController.athleteCardGameObjectPrefab, athleteCardGameObjectParent).GetComponent<AthleteCardObject>();
        newAthleteCardObject.SetForAthlete(athlete);

        AthleteObject newAthleteObject = Instantiate(gameController.athleteGameObjectPrefab, athleteGameObjectParent).GetComponent<AthleteObject>();
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

        //TODO: Animate

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
}
