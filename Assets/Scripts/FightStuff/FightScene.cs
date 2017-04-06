using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightScene : Scene<TransitionData> {

	public Player[] players;
    public Sprite[] playerSprites;
    public RuntimeAnimatorController[] playerAnimators;

    public Vector3[] hallwaySpawns;
    public Vector3[] cafeteriaSpawns;
    public Vector3[] roofSpawns;
    public Vector3[] parkingLotSpawns;
    public Vector3[] hellSpawns;
    private Vector3[][] spawnPoints;

    public GameObject[] arenas;

    public Player fallenPlayer;
    public int fallDamage;
    public float fallAnimationTime;
    public int roundNum;
    public bool lastComic;

	// Use this for initialization
	void Start () {
        
	}

    internal override void Init()
    {
        InitializeFightServices();
        lastComic = false;
        SetUpArenas();
        InitiateFightSequence();
    }

    internal override void OnEnter(TransitionData data)
    {
        roundNum = data.roundNum;
        InitiateFightSequence();
    }

    void InitializeFightServices()
    {
        Services.FightScene = this;
        Services.FightUIManager = GameObject.FindGameObjectWithTag("FightUIManager").GetComponent<FightUIManager>();
        Services.WinScreenUIManager = GameObject.FindGameObjectWithTag("WinScreenUIManager").GetComponent<WinScreenUIManager>();
        Services.TransitionComicManager = GameObject.FindGameObjectWithTag("TransitionComicManager").GetComponent<TransitionComicManager>();
        if (Services.GameInfo.player1Abilities.Count == 0)
        {
            SetPlayerAbilities();
        }
    }

    // for testing purposes only
    void SetPlayerAbilities()
    {
        Services.GameInfo.player1Abilities = new List<Ability.Type>() {
            Ability.Type.Fireball,
            Ability.Type.Lunge,
            Ability.Type.Pull
        };
        Services.GameInfo.player2Abilities = new List<Ability.Type>() {
            Ability.Type.Shield,
            Ability.Type.Sing,
            Ability.Type.Wallop
        };
    }

	void InitializePlayers(){
        players = new Player[2]
        {
            InitializePlayer(1),
            InitializePlayer(2)
        };
	}

	Player InitializePlayer(int playerNum){
        GameObject playerObj = Instantiate(Services.PrefabDB.Player, spawnPoints[roundNum - 1][playerNum - 1], 
            Quaternion.identity) as GameObject;
        Player player = playerObj.GetComponent<Player>();
		List<Ability.Type> abilityList;

		if (playerNum == 1) {
			abilityList = Services.GameInfo.player1Abilities;

		} else {
			abilityList = Services.GameInfo.player2Abilities;
		}
		player.playerNum = playerNum;
        playerObj.GetComponent<SpriteRenderer>().sprite = playerSprites[playerNum - 1];
        playerObj.GetComponent<Animator>().runtimeAnimatorController = playerAnimators[playerNum - 1];
		player.abilityList = abilityList;
        return player;
	}

    void SetUpArenas()
    {
        spawnPoints = new Vector3[5][]
        {
            hallwaySpawns,
            cafeteriaSpawns,
            roofSpawns,
            parkingLotSpawns,
            hellSpawns
        };
        for(int i = 0; i < arenas.Length; i++)
        {
            if (i == roundNum - 1)
            {
                arenas[i].SetActive(true);
            }
            else
            {
                arenas[i].SetActive(false);
            }
        }
    }

    void GameOver()
    {
        Services.EventManager.Fire(new GameOver(fallenPlayer));
    }

    void LastComic()
    {
        lastComic = true;
    }

    void IncrementRoundNum()
    {
        roundNum += 1;
    }

    void PositionPlayers()
    {
        foreach(Player player in players)
        {
            player.transform.position = spawnPoints[roundNum - 1][player.playerNum - 1];
            player.StartListeningForInput();
            player.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

        fallenPlayer.TakeHit(fallDamage, 0, 0, Vector3.zero);

    }

    void InitiateFightSequence()
    {
        if (roundNum == 3)
        {
            InitiateFinalFightSequence();
        }
        else
        {
            ActionTask initializePlayers = new ActionTask(InitializePlayers);
            WaitForFall waitForFall = new WaitForFall();
            Task transitionComic = ComicSequence(waitForFall, roundNum);
            ActionTask transitionBackToVN = new ActionTask(TransitionBackToVN);

            initializePlayers
                .Then(waitForFall)
                .Then(transitionComic)
                .Then(transitionBackToVN);

            Services.TaskManager.AddTask(initializePlayers);
        }
    }

    void InitiateFinalFightSequence()
    {
        ActionTask initializePlayersForRooftop = new ActionTask(InitializePlayers);
        WaitForFall waitForRooftopFall = new WaitForFall();
        PlayerFallAnimation rooftopFallAnimation = new PlayerFallAnimation();
        Task transitionToParkingLot = ComicSequence(rooftopFallAnimation, 3);
        ActionTask positionPlayersForParkingLot = new ActionTask(PositionPlayers);
        WaitForFall waitForParkingLotFall = new WaitForFall();
        PlayerFallAnimation parkingLotFallAnimation = new PlayerFallAnimation();
        Task transitionToHell = ComicSequence(parkingLotFallAnimation, 4);
        ActionTask positionPlayersForHell = new ActionTask(PositionPlayers);
        WaitForFall waitForHellFall = new WaitForFall();
        PlayerFallAnimation hellFallAnimation = new PlayerFallAnimation();
        Task winComic = ComicSequence(hellFallAnimation, 5);
        ActionTask gameOver = new ActionTask(GameOver);
        winComic.Then(gameOver);

        initializePlayersForRooftop
            .Then(waitForRooftopFall)
            .Then(rooftopFallAnimation);

        transitionToParkingLot
            .Then(positionPlayersForParkingLot)
            .Then(waitForParkingLotFall)
            .Then(parkingLotFallAnimation);

        transitionToHell
            .Then(positionPlayersForHell)
            .Then(waitForHellFall)
            .Then(hellFallAnimation);

        Services.TaskManager.AddTask(initializePlayersForRooftop);
    }

    Task ComicSequence(Task precedingTask, int roundNumber)
    {
        Transform comicTransform = Services.TransitionComicManager.fightEndComics[roundNumber - 1].transform;
        SlideInPanel slideInComicBackground = new SlideInPanel(Services.TransitionComicManager.comicBackground, true, 1600 * Vector2.right,
            Services.TransitionComicManager.panelAppearTime);
        precedingTask
            .Then(slideInComicBackground);
        Task currentTask = slideInComicBackground;
        if (roundNumber != 1) {
            Task turnOffPreviousArena = new SetObjectStatus(false, arenas[roundNumber - 2]);
            currentTask.Then(turnOffPreviousArena);
            currentTask = turnOffPreviousArena;
        }
        SetObjectStatus turnOnTransition = new SetObjectStatus(true, comicTransform.gameObject);
        currentTask
            .Then(turnOnTransition);
        int numPages = comicTransform.childCount;
        currentTask = turnOnTransition;
        for (int i = 0; i < numPages; i++)
        {
            Transform page = comicTransform.GetChild(i);
            SetObjectStatus turnOnPage = new SetObjectStatus(true, page.gameObject);
            currentTask.Then(turnOnPage);
            currentTask = turnOnPage;
            int numPanels = page.childCount;
            for (int j = 0; j < numPanels; j++)
            {
                SetPanelImage setPanelImage = new SetPanelImage(page.GetChild(j).gameObject, j, i, arenas[roundNumber-1]);
                SlideInPanel slideInPanel = new SlideInPanel(page.GetChild(j).gameObject, true, 
                    Services.TransitionComicManager.comicShifts[roundNumber-1][i][j], Services.TransitionComicManager.panelAppearTime);
                currentTask
                    .Then(setPanelImage)
                    .Then(slideInPanel);
                currentTask = slideInPanel;
            }
            if ((i == numPages - 1) && (roundNumber == 5))
            {
                ActionTask itsTheLastComic = new ActionTask(LastComic);
                currentTask.Then(itsTheLastComic);
                currentTask = itsTheLastComic;
            }
            WaitToContinueFromComic waitToContinue = new WaitToContinueFromComic(page.gameObject,
                Services.TransitionComicManager.continueButton, Services.TransitionComicManager.continuePromptGrowTime, 
                Services.TransitionComicManager.continuePromptShrinkTime);
            currentTask.Then(waitToContinue);
            currentTask = waitToContinue;
        }
        if (roundNumber == 3 || roundNumber == 4)
        {
            Task turnOnNextArena = new SetObjectStatus(true, arenas[roundNumber - 1]);
            SetObjectStatus turnOffBackground = new SetObjectStatus(false, Services.TransitionComicManager.comicBackground);
            ActionTask incrementRoundNum = new ActionTask(IncrementRoundNum);
            currentTask
                .Then(turnOnNextArena)
                .Then(turnOffBackground)
                .Then(incrementRoundNum);
            currentTask = incrementRoundNum;
        }
        
        return currentTask;
    }

    void TransitionBackToVN()
    {
        Services.SceneStackManager.PopScene();
    }

}
