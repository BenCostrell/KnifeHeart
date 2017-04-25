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

    public bool fightActive;
    public Player fallenPlayer;
    public int fallDamage;
    public float fallAnimationTime;
    public int roundNum;
    public bool lastComic;
    public float hitLagRatio;

	// Use this for initialization
	void Start () {
        
	}

    internal override void Init()
    {
        InitializeFightServices();
        lastComic = false;
        Services.TransitionComicManager.Init();
    }

    internal override void OnEnter(TransitionData data)
    {
        roundNum = data.roundNum;
        Services.FightUIManager.Init();
        SetUpArenas();
        InitiateFightSequence();
    }

    void InitializeFightServices()
    {
        Services.FightScene = this;
        Transform servicesObj = transform.FindChild("Services");
        Services.FightUIManager = servicesObj.FindChild("FightUIManager").gameObject.GetComponent<FightUIManager>();
        Services.WinScreenUIManager = servicesObj.FindChild("WinScreenUIManager").gameObject.GetComponent<WinScreenUIManager>();
        Services.TransitionComicManager = servicesObj.FindChild("TransitionComicManager").gameObject.GetComponent<TransitionComicManager>();
        Services.CameraController = Camera.main.GetComponent<CameraController>();
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
            Ability.Type.Blink,
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
        fightActive = true;
    }

    Player InitializePlayer(int playerNum){
        GameObject playerObj = Instantiate(Services.PrefabDB.Player, spawnPoints[roundNum - 1][playerNum - 1], 
            Quaternion.identity, transform) as GameObject;
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

    public GameObject GetActiveArena()
    {
        for (int i = 0; i < arenas.Length; i++)
        {
            if (arenas[i].activeSelf) return arenas[i];
        }
        return null;
    }

    void GameOver()
    {
        //Services.EventManager.Fire(new GameOver(fallenPlayer));
    }

    void LastComic()
    {
        lastComic = true;
    }

    public void PositionPlayers(int roundNumber)
    {
        foreach(Player player in players)
        {
            player.transform.position = spawnPoints[roundNumber - 1][player.playerNum - 1];
            player.StartListeningForInput();
            player.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            player.ResetCooldowns();
            if (player == fallenPlayer) player.damage = fallDamage;
            else player.damage = 0;
            foreach (Ability.Type ability in player.abilityList) Services.FightUIManager.ScaleCooldownUI(ability, player.playerNum, 1);
        }
        Services.FightUIManager.UpdateDamageUI();
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
                .Then(waitForFall);
            transitionComic
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
        ActionTask resumeCameraFollow1 = new ActionTask(Services.CameraController.ResumeCameraFollow);
        PositionPlayersTask positionPlayersForParkingLot = new PositionPlayersTask(4);
        WaitForFall waitForParkingLotFall = new WaitForFall();
        PlayerFallAnimation parkingLotFallAnimation = new PlayerFallAnimation();
        Task transitionToHell = ComicSequence(parkingLotFallAnimation, 4);
        ActionTask resumeCameraFollow2 = new ActionTask(Services.CameraController.ResumeCameraFollow);
        PositionPlayersTask positionPlayersForHell = new PositionPlayersTask(5);
        WaitForFall waitForHellFall = new WaitForFall();
        PlayerFallAnimation hellFallAnimation = new PlayerFallAnimation();
        Task winComic = ComicSequence(hellFallAnimation, 5);
        //ActionTask gameOver = new ActionTask(GameOver);
        SetObjectStatus turnOnResetText = new SetObjectStatus(true, Services.WinScreenUIManager.uiResetText);
        winComic.Then(turnOnResetText);

        initializePlayersForRooftop
            .Then(waitForRooftopFall)
            .Then(rooftopFallAnimation);

        transitionToParkingLot
            .Then(resumeCameraFollow1)
            .Then(positionPlayersForParkingLot)
            .Then(waitForParkingLotFall)
            .Then(parkingLotFallAnimation);

        transitionToHell
            .Then(resumeCameraFollow2)
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
        ActionTask resetCamera = new ActionTask(Services.CameraController.ResetCamera);
        precedingTask
            .Then(slideInComicBackground)
            .Then(resetCamera);
        Task currentTask = resetCamera;
        if (roundNumber == 3 || roundNumber == 4) {
            Task turnOffPreviousArena = new SetObjectStatus(false, arenas[roundNumber - 1]);
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
                SetPanelImage setPanelImage = new SetPanelImage(page.GetChild(j).gameObject);
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
            SetObjectStatus turnOnNextArena = new SetObjectStatus(true, arenas[roundNumber]);
            SetObjectStatus turnOffBackground = new SetObjectStatus(false, Services.TransitionComicManager.comicBackground);
            currentTask
                .Then(turnOnNextArena)
                .Then(turnOffBackground);
            currentTask = turnOffBackground;
        }
        
        return currentTask;
    }

    void TransitionBackToVN()
    {
        Services.SceneStackManager.PopScene();
    }

}
