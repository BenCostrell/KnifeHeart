using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNovelScene : Scene<TransitionData> {

	public TextAsset dialogueFile;
    public TextAsset rpsDialogueFile;
	private List<Ability.Type> abilityPool;
	private List<Ability.Type> currentRoundAbilityPool;
    [HideInInspector]
    public List<Ability.Type>[] abilityLists;
    [HideInInspector]
	public int currentTurnPlayerNum;
    [HideInInspector]
	public int currentRoundNum;
    [HideInInspector]
    public int initiatingPlayer;
    [HideInInspector]
    public string[] rpsDialogueArray;
    private Vector2[,,] comicShiftArray;
    public List<List<List<Vector2>>> comicShifts;
    [HideInInspector]
    public GameObject canvas;
    [HideInInspector]
    public RpsOption[] lastRpsChoices;
    public enum RpsOption { Rock, Paper, Scissors, None }

    internal override void Init()
    {
        Services.MusicManager.StartFMODInstance();
        InitializeVNServices();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        currentRoundNum = 0;
        GenerateDialogueData();
        GenerateRpsDialogueData();
        InitializeAbilityPool();
        InitializeComicShiftArray();
        Services.EventManager.Register<DialoguePicked>(PickAbility);
        Services.DialogueUIManager.Init();
    }

    internal override void OnEnter(TransitionData data)
    {
        currentRoundNum += 1;
        Services.MusicManager.StartVNMusic();
        Services.ComicPanelManager.TurnOffComicPanels();
        Services.DialogueUIManager.SetUpUI();
        Services.TransitionUIManager.SetUpUI();
        StartSequence();
        Services.EventManager.Fire(new SceneTransition("VisualNovelScene"));
    }

    // Update is called once per frame
    void Update () {
		
	}

    void InitializeVNServices()
    {
        Services.DialogueDataManager = new DialogueDataManager();
        Services.DialogueUIManager = GameObject.FindGameObjectWithTag("DialogueUIManager").GetComponent<DialogueUIManager>();
        Services.TransitionUIManager = GameObject.FindGameObjectWithTag("TransitionUIManager").GetComponent<TransitionUIManager>();
        Services.VisualNovelScene = this;
        Services.ComicPanelManager = GameObject.FindGameObjectWithTag("ComicPanelManager").GetComponent<ComicPanelManager>();
    }

    void InitializeComicShiftArray()
    {
        comicShifts = new List<List<List<Vector2>>>
        {
            /// hallway prefight comic
            new List<List<Vector2>>{
                new List<Vector2> {1600 * Vector2.left, 1600 * Vector2.right, 900 * Vector2.up },
                new List<Vector2> {1600 * Vector2.right, 1600 * Vector2.left, 1600 * Vector2.right }
            },
            /// cafeteria prefight comic
            new List<List<Vector2>>
            {
                new List<Vector2> { 1600 * Vector2.left, 1600 * Vector2.right, 1600 * Vector2.left, 1600 * Vector2.right },
                new List<Vector2> {900 * Vector2.down, 900 * Vector2.up, 900 * Vector2.down}
            },
            /// rooftop prefight comic
            new List<List<Vector2>>
            {
                new List<Vector2> {1600 * Vector2.left, 1600 * Vector2.right },
                new List<Vector2> {1600 * Vector2.left, 1600 * Vector2.right, 1600 * Vector2.left},
                new List<Vector2> {1600 * Vector2.left, 1600 * Vector2.right}
            },
         };
    }

    void InitializeAbilityPool(){
		abilityPool = new List<Ability.Type> () { 
			Ability.Type.Fireball, 
			Ability.Type.Lunge, 
			Ability.Type.Shield, 
			Ability.Type.Sing, 
			Ability.Type.Wallop, 
			Ability.Type.Pull,
            Ability.Type.Blink
		};
        abilityLists = new List<Ability.Type>[2]
        {
            new List<Ability.Type>(),
            new List<Ability.Type>()
        };
	}

	Ability.Type GetRandomAbility(List<Ability.Type> abilityList){
		Ability.Type ability;
		if (abilityList.Count > 0) {
			int index = Random.Range (0, abilityList.Count);
			ability = abilityList [index];
			abilityList.Remove (ability);
		} else {
			ability = Ability.Type.None;
		}
		return ability;
	}

	void GenerateDialogueData(){
		Services.DialogueDataManager.ParseDialogueFile (dialogueFile);
	}

    void GenerateRpsDialogueData()
    {
        Services.DialogueDataManager.ParseRpsDialogueFile(rpsDialogueFile);
    }

    void StartSequence()
    {
        Services.ComicPanelManager.comicBackground.SetActive(true);
        TaskQueue comicSequence = ComicSequence(Services.ComicPanelManager.scenarios[currentRoundNum - 1].transform, currentRoundNum - 1);
        TaskQueue roundSequence = RoundSequence();
        TaskQueue transitionSequence = TransitionToFightSequence();

        Services.TaskManager.AddTaskQueue(comicSequence.Then(roundSequence).Then(transitionSequence));
    }

    TaskQueue ComicSequence(Transform scenarioTransform, int comicNum)
    {
        int numPages = scenarioTransform.childCount;
        TaskQueue comicTasks = new TaskQueue();
        comicTasks.Add(new SetObjectStatus(true, scenarioTransform.gameObject));
        for (int i = 0; i < numPages; i++)
        {
            Transform page = scenarioTransform.GetChild(i);
            comicTasks.Add(new SetObjectStatus(true, page.gameObject));
            int numPanels = page.childCount;
            for (int j = 0; j < numPanels; j++)
            {
                comicTasks.Add (new SlideInPanel(page.GetChild(j).gameObject, true, comicShifts[comicNum][i][j],
                    Services.ComicPanelManager.panelAppearTime));
            }
            comicTasks.Add(new WaitToContinueFromComic(page.gameObject, Services.ComicPanelManager.continueButton,
                Services.TransitionUIManager.readyPromptGrowTime, Services.TransitionUIManager.readyPromptShrinkTime));
        }

        comicTasks.Then(new TaskQueue(new List<Task>() {
            new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer),
            new SetObjectStatus(false, Services.ComicPanelManager.comicBackground)
        }));

        return comicTasks;
    }

    TaskQueue RoundSequence()
    {
        return RpsSequence().Then(DialogueExchangeSequence());
    }

    TaskQueue RpsSequence()
    {
        return new TaskQueue(new List<Task>()
        {
            new ActionTask(Services.DialogueUIManager.InRpsStage),
            new ShowPreRpsWords(Services.DialogueUIManager.rpsWordScaleInTime, Services.DialogueUIManager.rpsWordStaggerTime),
            new WaitForTime(0.5f),
            new ScaleOutRpsWords(Services.DialogueUIManager.rpsWords[0].transform.parent.gameObject,
            Services.DialogueUIManager.rpsWordScaleOutTime),
            new ShowRpsDialogueOptions(),
            new WaitForRpsDialogueSelection(),
            new TransitionFromSelectionToDialogue(),
            new PopUpDialogueBox(false, true),
            new TypeRpsDialogue(),
            new WaitToContinueDialogue(false),
            new ActionTask(ChangePlayerTurn),
            new PopUpDialogueBox(false, true),
            new TypeRpsDialogue(),
            new WaitToContinueDialogue(false),
            new SlideInCrowd(),
            new PopUpDialogueBox(true, false),
            new TypeDialogue(true),
            new WaitToContinueDialogue(true),
            new ActionTask(Services.DialogueUIManager.NotInRpsStage)
        });
    }

    TaskQueue DialogueExchangeSequence()
    {
        return new TaskQueue(new List<Task>() {
            new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer),
            new ShowDialogueOptions (true),
            new WaitForDialogueChoiceTask (),
            new HighlightSelectedOption (),
            new ActionTask(Services.DialogueUIManager.SetPose),
            new PopUpDialogueBox(false, false),
            new TypeDialogue (false),
            new WaitToContinueDialogue (false),
            new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer),
            new ActionTask(ChangePlayerTurn),
            new ShowDialogueOptions (false),
            new WaitForDialogueChoiceTask (),
            new HighlightSelectedOption (),
            new ActionTask(Services.DialogueUIManager.SetPose),
            new PopUpDialogueBox(false, false),
            new TypeDialogue (false),
            new WaitToContinueDialogue (false),
            new DialogueTransitionTask ()
        });
    }

    TaskQueue TransitionToFightSequence()
    {
        Services.GameInfo.player1Abilities = abilityLists[0];
        Services.GameInfo.player2Abilities = abilityLists[1];
        return new TaskQueue(new List<Task>()
        {
            new ActionTask(Services.MusicManager.StartPlayingTransition),
            new SlideOutCrowd(),
            new SlideInFightBackground (),
            new ShowFightinWords (),
            new AbilityShowingSequence(Services.TransitionUIManager.blurbScaleInTime,
            Services.TransitionUIManager.blurbFlipTime, Services.TransitionUIManager.blurbDelayBeforeFlipping),
            new WaitForReady (),
            new ScaleOutTransitionUI (),
            new FinishTransition (),
            new ActionTask(StartFight)
        });
    }

    void StartFight()
    {
        Services.SceneStackManager.PushScene<FightScene>(new TransitionData(currentRoundNum));
    }

    public void GenerateDialogueOptions(bool firstChoice){
		Dialogue[] dialogueOptions = new Dialogue[4];
		List<Ability.Type> abilityList;
		List<Ability.Type> playerContext = null;
		List<Ability.Type> fullAbilityKey;

        playerContext = new List<Ability.Type>(abilityLists[currentTurnPlayerNum - 1]);

		if (firstChoice) {
			abilityList = new List<Ability.Type> (abilityPool);
			currentRoundAbilityPool = new List<Ability.Type> ();
			currentRoundAbilityPool.Add (GetRandomAbility (abilityList));
			currentRoundAbilityPool.Add (GetRandomAbility (abilityList));
			currentRoundAbilityPool.Add (GetRandomAbility (abilityList));
			currentRoundAbilityPool.Add (GetRandomAbility (abilityList));
		}

		for (int i = 0; i < currentRoundAbilityPool.Count; i++) {
			fullAbilityKey = new List<Ability.Type> (playerContext);
			fullAbilityKey.Add (currentRoundAbilityPool [i]);
			dialogueOptions [i] = Services.DialogueDataManager.GetDialogue (fullAbilityKey, firstChoice);
		}

		Services.DialogueUIManager.SetDialogueOptions (dialogueOptions);
	}

	void PickAbility(DialoguePicked e){
		int playerNum = e.pickedByPlayerNum;
		Ability.Type ability = e.dialogue.abilityGiven;
        abilityLists[playerNum - 1].Add(ability);
		abilityPool.Remove (ability);
		currentRoundAbilityPool.Remove (ability);
        Services.TransitionUIManager.dialoguesAccumulated[playerNum - 1].Add(e.dialogue);
		Debug.Log (ability.ToString () + " picked");
	}

	public void ChangePlayerTurn(){
		if (currentTurnPlayerNum == 1) {
			currentTurnPlayerNum = 2;
		} else if (currentTurnPlayerNum == 2) {
			currentTurnPlayerNum = 1;
		}
	}

    public void ProcessRpsChoices(RpsOption choice_P1, RpsOption choice_P2, float choiceTime_P1, float choiceTime_P2)
    {
        int winningPlayerNum = 0;
        string[] dialogueArray = null;
        if ((choice_P1 == RpsOption.None) && (choice_P2 == RpsOption.None))
        {
            Debug.Log("nothing chosen");
            winningPlayerNum = Random.Range(1, 3);
        }
        else if (choice_P1 == RpsOption.None)
        {
            Debug.Log("player 1 chose nothing");
            Debug.Log("player 2 chose " + choice_P2);
            winningPlayerNum = 2;
        }
        else if (choice_P2 == RpsOption.None)
        {
            Debug.Log("player 1 chose " + choice_P1);
            Debug.Log("player 2 chose nothing");
            winningPlayerNum = 1;
        }

        if (choice_P1 == RpsOption.Rock)
        {
            if (choice_P2 == RpsOption.Rock)
            {
                if (choiceTime_P1 < choiceTime_P2)
                {
                    Debug.Log("player 1 played faster rock");
                    winningPlayerNum = 1;
                }
                else if (choiceTime_P1 > choiceTime_P2)
                {
                    Debug.Log("player 2 played faster rock");
                    winningPlayerNum = 2;
                }
                else
                {
                    Debug.Log("tied rock");
                    winningPlayerNum = Random.Range(1, 3);
                }
            }
            else if (choice_P2 == RpsOption.Paper)
            {
                Debug.Log("player 2 beats rock with paper");
                winningPlayerNum = 2;
            }
            else if (choice_P2 == RpsOption.Scissors)
            {
                Debug.Log("player 1 beats scissors with rock");
                winningPlayerNum = 1;
            }
        }
        else if (choice_P1 == RpsOption.Paper)
        {
            if (choice_P2 == RpsOption.Rock)
            {
                Debug.Log("player 1 beats rock with paper");
                winningPlayerNum = 1;
            }
            else if (choice_P2 == RpsOption.Paper)
            {
                if (choiceTime_P1 < choiceTime_P2)
                {
                    Debug.Log("player 1 played faster paper");
                    winningPlayerNum = 1;
                }
                else if (choiceTime_P1 > choiceTime_P2)
                {
                    Debug.Log("player 2 played faster paper");
                    winningPlayerNum = 2;
                }
                else
                {
                    Debug.Log("tied paper");
                    winningPlayerNum = Random.Range(1, 3);
                }
            }
            else if (choice_P2 == RpsOption.Scissors)
            {
                Debug.Log("player 2 beats paper with scissors");
                winningPlayerNum = 2;
            }
        }
        else if (choice_P1 == RpsOption.Scissors)
        {
            if (choice_P2 == RpsOption.Rock)
            {
                Debug.Log("player 2 beats scissors with rock");
                winningPlayerNum = 2;
            }
            else if (choice_P2 == RpsOption.Paper)
            {
                Debug.Log("player 1 beats paper with scissors");
                winningPlayerNum = 1;
            }
            else if (choice_P2 == RpsOption.Scissors)
            {
                if (choiceTime_P1 < choiceTime_P2)
                {
                    Debug.Log("player 1 played faster scissors");
                    winningPlayerNum = 1;
                }
                else if (choiceTime_P1 > choiceTime_P2)
                {
                    Debug.Log("player 2 played faster scissors");
                    winningPlayerNum = 2;
                }
                else
                {
                    Debug.Log("tied scissors");
                    winningPlayerNum = Random.Range(1, 3);
                }
            }
        }
        if (winningPlayerNum == 1)
        {
            dialogueArray = Services.DialogueDataManager.GetRpsDialogue(currentRoundNum, choice_P1, choice_P2);
        }
        else if (winningPlayerNum == 2)
        {
            dialogueArray = Services.DialogueDataManager.GetRpsDialogue(currentRoundNum, choice_P2, choice_P1);
        }

        initiatingPlayer = winningPlayerNum;
        currentTurnPlayerNum = 3 - winningPlayerNum;
        rpsDialogueArray = dialogueArray;
        lastRpsChoices = new RpsOption[2] { choice_P1, choice_P2 };
    }

    
}
