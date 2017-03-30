using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightSceneManager : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public Sprite player1Sprite;
	public Sprite player2Sprite;
    public Vector3[] roofSpawns;
    public Vector3[] parkingLotSpawns;
    public Vector3[] hellSpawns;
	public RuntimeAnimatorController player1Anim;
	public RuntimeAnimatorController player2Anim;

    public GameObject rooftopArena;
    public GameObject parkingLotArena;
    public GameObject hellArena;

    public Player fallenPlayer;
    public int fallDamage;
    private int roundNum;

	// Use this for initialization
	void Start () {
        roundNum = 1;
        SetUpArenas();
        InitiateFightSequence();
	}
	
	// Update is called once per frame
	void Update () {
	}



	void InitializePlayers(){
		player1 = Instantiate (Services.PrefabDB.Player, roofSpawns[0], Quaternion.identity) as GameObject;
		player2 = Instantiate (Services.PrefabDB.Player, roofSpawns[1], Quaternion.identity) as GameObject;

		InitializePlayer (player1, 1);
		InitializePlayer (player2, 2);
	}

	void InitializePlayer(GameObject player, int playerNum){
		Sprite playerSprite;
		RuntimeAnimatorController playerAnim;
		List<Ability.Type> abilityList;

		if (playerNum == 1) {
			playerSprite = player1Sprite;
			playerAnim = player1Anim;
			abilityList = Services.GameInfo.player1Abilities;

		} else {
			playerSprite = player2Sprite;
			playerAnim = player2Anim;
			abilityList = Services.GameInfo.player2Abilities;
		}

		Player pc = player.GetComponent<Player> ();
		pc.playerNum = playerNum;
		player.GetComponent<SpriteRenderer> ().sprite = playerSprite;
		player.GetComponent<Animator> ().runtimeAnimatorController = playerAnim;
		pc.abilityList = abilityList;
	}

    void SetUpArenas()
    {
        rooftopArena.SetActive(true);
        parkingLotArena.SetActive(false);
        hellArena.SetActive(false);
    }

    void GameOver()
    {
        Services.EventManager.Fire(new GameOver(fallenPlayer));
    }

    void IncrementRoundNum()
    {
        roundNum += 1;
    }

    void PositionPlayers()
    {
        Vector3[] spawns;
        if(roundNum == 2)
        {
            spawns = parkingLotSpawns;
        }
        else
        {
            spawns = hellSpawns;
        }

        player1.transform.position = spawns[0];
        player2.transform.position = spawns[1];

        player1.GetComponent<Player>().StartListeningForInput();
        player2.GetComponent<Player>().StartListeningForInput();

        fallenPlayer.TakeHit(fallDamage, 0, 0, Vector3.zero);

    }

    void InitiateFightSequence()
    {
        ActionTask initializePlayersForRooftop = new ActionTask(InitializePlayers);
        WaitForFall waitForRooftopFall = new WaitForFall();
        Task transitionToParkingLot = ComicSequence(waitForRooftopFall, 
            Services.TransitionComicManager.transitionToParkingLot.transform,
            new Vector2[,]
            {
                { 1600 * Vector2.left, 1600 * Vector2.right }
            },
            rooftopArena, parkingLotArena);
        ActionTask positionPlayersForParkingLot = new ActionTask(PositionPlayers);
        WaitForFall waitForParkingLotFall = new WaitForFall();
        Task transitionToHell = ComicSequence(waitForParkingLotFall,
            Services.TransitionComicManager.transitionToHell.transform,
            new Vector2[,]
            {
                { 1600 * Vector2.left, 1600 * Vector2.right }
            },
            parkingLotArena, hellArena);
        ActionTask positionPlayersForHell = new ActionTask(PositionPlayers);
        WaitForFall waitForHellFall = new WaitForFall();
        ActionTask gameOver = new ActionTask(GameOver);

        initializePlayersForRooftop
            .Then(waitForRooftopFall);

        transitionToParkingLot
            .Then(positionPlayersForParkingLot)
            .Then(waitForParkingLotFall);

        transitionToHell
            .Then(positionPlayersForHell)
            .Then(waitForHellFall)
            .Then(gameOver);

        Services.TaskManager.AddTask(initializePlayersForRooftop);
    }

    Task ComicSequence(Task precedingTask, Transform transitionTransform, Vector2[,] shifts, GameObject previousArena, GameObject newArena)
    {
        SlideInPanel slideInComicBackground = new SlideInPanel(Services.TransitionComicManager.comicBackground, true, 1600 * Vector2.right,
            Services.TransitionComicManager.panelAppearTime);
        Task turnOffPreviousArena = new SetObjectStatus(false, previousArena);
        SetObjectStatus turnOnTransition = new SetObjectStatus(true, transitionTransform.gameObject);
        precedingTask
            .Then(slideInComicBackground)
            .Then(turnOffPreviousArena)
            .Then(turnOnTransition);
        int numPages = transitionTransform.childCount;
        Task currentTask = turnOnTransition;
        for (int i = 0; i < numPages; i++)
        {
            Transform page = transitionTransform.GetChild(i);
            SetObjectStatus turnOnPage = new SetObjectStatus(true, page.gameObject);
            currentTask.Then(turnOnPage);
            currentTask = turnOnPage;
            int numPanels = page.childCount;
            for (int j = 0; j < numPanels; j++)
            {
                SetPanelImage setPanelImage = new SetPanelImage(page.GetChild(j).gameObject, j, newArena);
                SlideInPanel slideInPanel = new SlideInPanel(page.GetChild(j).gameObject, true, shifts[i, j],
                    Services.TransitionComicManager.panelAppearTime);
                currentTask
                    .Then(setPanelImage)
                    .Then(slideInPanel);
                currentTask = slideInPanel;
            }
            WaitToContinueFromComic waitToContinue = new WaitToContinueFromComic(page.gameObject,
                Services.TransitionComicManager.continueButton, Services.TransitionComicManager.continuePromptGrowTime, 
                Services.TransitionComicManager.continuePromptShrinkTime);
            currentTask.Then(waitToContinue);
            currentTask = waitToContinue;
        }
        Task turnOnNextArena = new SetObjectStatus(true, newArena);
        SetObjectStatus turnOffBackground = new SetObjectStatus(false, Services.TransitionComicManager.comicBackground);
        ActionTask incrementRoundNum = new ActionTask(IncrementRoundNum);
        currentTask
            .Then(turnOnNextArena)
            .Then(turnOffBackground)
            .Then(incrementRoundNum);
        currentTask = incrementRoundNum;

        return currentTask;
    }

}
