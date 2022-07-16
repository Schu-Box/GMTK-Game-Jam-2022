using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamUIController : MonoBehaviour
{
    public Transform athleteGameObjectParent;
    public Transform athleteCardGameObjectParent;

    public Transform diceGameObjectParent;

    private GameController gameController;

	private void Start()
	{
        gameController = FindObjectOfType<GameController>();
        gameController.DeleteAllChidlren(athleteGameObjectParent);
        gameController.DeleteAllChidlren(athleteCardGameObjectParent);
        gameController.DeleteAllChidlren(diceGameObjectParent);
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
        //dice.diceObject = newDiceObject;
	}
}
