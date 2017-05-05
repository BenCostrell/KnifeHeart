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

    void Update()
    {
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
        Services.EventManager.Fire(new SceneTransition("FightScene"));
    }

    void InitializeFightServices()
    {
        Services.FightScene = this;
        Services.FightUIManager = GetComponentInChildren<FightUIManager>();
        Services.WinScreenUIManager = GetComponentInChildren<WinScreenUIManager>();
        Services.TransitionComicManager = GetComponentInChildren<TransitionComicManager>();
        Services.CameraController = GetComponentInChildren<CameraController>();
        Debug.Assert(Services.CameraController != null);
        if (Services.GameInfo.player1Abilities.Count == 0)
        {
            SetPlayerAbilities();
        }
    }

    // for testing purposes only
    void SetPlayerAbilities()
    {
        Services.GameInfo.player1Abilities = new List<Ability.Type>() {
			Ability.Type.Shield,
			Ability.Type.Wallop,
			Ability.Type.Pull
        };
        Services.GameInfo.player2Abilities = new List<Ability.Type>() {
			Ability.Type.Sing,
			Ability.Type.Fireball,
			Ability.Type.Lunge
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
            Services.FightUIManager.ScaleCooldownUI(Ability.Type.BasicAttack, player.playerNum, 1);
            player.stageEdgeBoundaryCollider.enabled = true;
        }
        Services.FightUIManager.UpdateDamageUI();
    }

    void InitiateFightSequence()
    {
        if (roundNum == 3)
        {
            Services.TaskManager.AddTaskQueue(FinalFightSequence());
        }
        else
        {
            TaskQueue fightSequence = new TaskQueue(new List<Task>() {
                new ActionTask(InitializePlayers),
                new WaitForFall()
            });

            fightSequence
                .Then(ComicSequence(roundNum)).Add(new ActionTask(TransitionBackToVN));

            Services.TaskManager.AddTaskQueue(fightSequence);
        }
    }

    TaskQueue FinalFightSequence()
    {
        TaskQueue rooftopSequence = new TaskQueue(new List<Task>() {
            new ActionTask(InitializePlayers),
            new WaitForFall(),
            new PlayerFallAnimation()
        });

        rooftopSequence.Then(ComicSequence(3));

        TaskQueue parkingLotSequence = new TaskQueue(new List<Task>() {
            new ActionTask(Services.CameraController.ResumeCameraFollow),
            new PositionPlayersTask(4),
            new WaitForFall(),
            new PlayerFallAnimation()
        });

        parkingLotSequence.Then(ComicSequence(4));

        TaskQueue hellSequence = new TaskQueue(new List<Task>()
        {
            new ActionTask(Services.CameraController.ResumeCameraFollow),
            new PositionPlayersTask(5),
            new WaitForFall(),
            new PlayerFallAnimation()
        });

        hellSequence.Then(ComicSequence(5));

        TaskQueue cleanUpSequence = new TaskQueue(new List<Task>()
        {
            new SetObjectStatus(true, Services.WinScreenUIManager.uiResetText)
        });

        TaskQueue finalFightSequence = rooftopSequence
            .Then(parkingLotSequence)
            .Then(hellSequence)
            .Then(cleanUpSequence);

        return finalFightSequence;
    }

    TaskQueue ComicSequence(int roundNumber)
    {
        Transform comicTransform = Services.TransitionComicManager.fightEndComics[roundNumber - 1].transform;

        TaskQueue comicSequence = new TaskQueue(new List<Task>()
        {
            new SlideInPanel(Services.TransitionComicManager.comicBackground, true, 1600 * Vector2.right,
            Services.TransitionComicManager.panelAppearTime),
            new ActionTask(Services.CameraController.ResetCamera)

        });
        if (roundNumber == 3 || roundNumber == 4)
        {
            comicSequence.Add(new SetObjectStatus(false, arenas[roundNumber - 1]));
        }

        comicSequence.Add(new SetObjectStatus(true, comicTransform.gameObject));

        int numPages = comicTransform.childCount;
        for (int i = 0; i < numPages; i++)
        {
            Transform page = comicTransform.GetChild(i);
            comicSequence.Add(new SetObjectStatus(true, page.gameObject));
            int numPanels = page.childCount;
            for (int j = 0; j < numPanels; j++)
            {
                comicSequence.Then(new TaskQueue(new List<Task>() {
                    new SetPanelImage(page.GetChild(j).gameObject),
                    new SlideInPanel(page.GetChild(j).gameObject, true,
                    Services.TransitionComicManager.comicShifts[roundNumber - 1][i][j], Services.TransitionComicManager.panelAppearTime)
                }));
            }
            if ((i == numPages - 1) && (roundNumber == 5))
            {
                comicSequence.Add(new ActionTask(LastComic));
            }
            comicSequence.Add(new WaitToContinueFromComic(page.gameObject,
                Services.TransitionComicManager.continueButton, Services.TransitionComicManager.continuePromptGrowTime,
                Services.TransitionComicManager.continuePromptShrinkTime));
        }
        if (roundNumber == 3 || roundNumber == 4)
        {
            comicSequence.Then(new TaskQueue(new List<Task>() {
                new SetObjectStatus(true, arenas[roundNumber]),
                new SetObjectStatus(false, Services.TransitionComicManager.comicBackground)
            }));
        }

        return comicSequence;
    }

    void TransitionBackToVN()
    {
        foreach (Player player in players) player.StopListeningForInput();
        Services.SceneStackManager.PopScene();
    }

}
