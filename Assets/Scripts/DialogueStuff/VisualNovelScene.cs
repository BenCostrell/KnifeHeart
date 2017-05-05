using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNovelScene : Scene<TransitionData> {

	public TextAsset dialogueFile;
    public TextAsset rpsDialogueFile;
	private List<Ability.Type> abilityPool;
	private List<Ability.Type> currentRoundAbilityPool;
    public List<Ability.Type>[] abilityLists;
	public int currentTurnPlayerNum;
	public int currentRoundNum;
    public int initiatingPlayer;
    public string[] rpsDialogueArray;
    private Vector2[,,] comicShiftArray;
    public GameObject canvas;

    internal override void Init()
    {
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
        comicShiftArray = new Vector2[3, 3, 3]
        {
            {
                {900 * Vector2.up, 900 * Vector2.left, 900 * Vector2.right },
                {1600 * Vector2.right, 1600 * Vector2.left, 1600 * Vector2.right },
                {Vector2.zero, Vector2.zero, Vector2.zero }
            },
            {
                {1600 * Vector2.left, 1600 * Vector2.right, Vector2.zero },
                {1600 * Vector2.left, 1600 * Vector2.right, Vector2.zero },
                {1600 * Vector2.left, 1600 * Vector2.right, Vector2.zero }
            },
            {
                {900 * Vector2.up, 1600 * Vector2.left, 1600 * Vector2.right },
                {1600 * Vector2.left, Vector2.zero, Vector2.zero },
                {1600 * Vector2.left, 1600 * Vector2.right, Vector2.zero }
            }
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
                comicTasks.Add (new SlideInPanel(page.GetChild(j).gameObject, true, comicShiftArray[comicNum, i, j],
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
            new SlideInCrowd(),
            new ShowRpsDialogueOptions(),
            new WaitForRpsDialogueSelection(),
            new TransitionFromSelectionToDialogue(),
            new PopUpDialogueBox(false),
            new TypeRpsDialogue(),
            new WaitToContinueDialogue(),
            new PopUpDialogueBox(true),
            new TypeDialogue(true),
            new WaitToContinueDialogue(),
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
            new PopUpDialogueBox(false),
            new TypeDialogue (false),
            new WaitToContinueDialogue (),
            new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer),
            new ShowDialogueOptions (false),
            new WaitForDialogueChoiceTask (),
            new HighlightSelectedOption (),
            new ActionTask(Services.DialogueUIManager.SetPose),
            new PopUpDialogueBox(false),
            new TypeDialogue (false),
            new WaitToContinueDialogue (),
            new DialogueTransitionTask ()
        });
    }

    TaskQueue TransitionToFightSequence()
    {
        Services.GameInfo.player1Abilities = abilityLists[0];
        Services.GameInfo.player2Abilities = abilityLists[1];
        return new TaskQueue(new List<Task>()
        {
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

    public void ProcessRpsChoices(string choice_P1, string choice_P2, float choiceTime_P1, float choiceTime_P2)
    {
        int winningPlayerNum = 0;
        string[] dialogueArray = null;
        if ((choice_P1 == "") && (choice_P2 == ""))
        {
            Debug.Log("nothing chosen");
            winningPlayerNum = Random.Range(1, 3);
        }
        else if (choice_P1 == "")
        {
            Debug.Log("player 1 chose nothing");
            Debug.Log("player 2 chose " + choice_P2);
            winningPlayerNum = 2;
        }
        else if (choice_P2 == "")
        {
            Debug.Log("player 1 chose " + choice_P1);
            Debug.Log("player 2 chose nothing");
            winningPlayerNum = 1;
        }

        if (choice_P1 == "BE AGGRESSIVE")
        {
            if (choice_P2 == "BE AGGRESSIVE")
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
            else if (choice_P2 == "BE NICE")
            {
                Debug.Log("player 2 beats rock with paper");
                winningPlayerNum = 2;
            }
            else if (choice_P2 == "BE PASSIVE AGGRESSIVE")
            {
                Debug.Log("player 1 beats scissors with rock");
                winningPlayerNum = 1;
            }
        }
        else if (choice_P1 == "BE NICE")
        {
            if (choice_P2 == "BE AGGRESSIVE")
            {
                Debug.Log("player 1 beats rock with paper");
                winningPlayerNum = 1;
            }
            else if (choice_P2 == "BE NICE")
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
            else if (choice_P2 == "BE PASSIVE AGGRESSIVE")
            {
                Debug.Log("player 2 beats paper with scissors");
                winningPlayerNum = 2;
            }
        }
        else if (choice_P1 == "BE PASSIVE AGGRESSIVE")
        {
            if (choice_P2 == "BE AGGRESSIVE")
            {
                Debug.Log("player 2 beats scissors with rock");
                winningPlayerNum = 2;
            }
            else if (choice_P2 == "BE NICE")
            {
                Debug.Log("player 1 beats paper with scissors");
                winningPlayerNum = 1;
            }
            else if (choice_P2 == "BE PASSIVE AGGRESSIVE")
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
    }

    
}
