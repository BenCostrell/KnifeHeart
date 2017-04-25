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
        Task startTask = new WaitTask(0);
        Task comicSequence = ComicSequence(startTask, Services.ComicPanelManager.scenarios[currentRoundNum - 1].transform, 
            currentRoundNum - 1);
        Task roundSequence = RoundSequence(comicSequence);
        Task transitionToFight = TransitionToFightSequence(roundSequence);

        Services.TaskManager.AddTask(startTask);
    }

    Task ComicSequence(Task precedingTask, Transform scenarioTransform, int comicNum)
    {
        SetObjectStatus turnOnScenario = new SetObjectStatus(true, scenarioTransform.gameObject);
        precedingTask
            .Then(turnOnScenario);
        int numPages = scenarioTransform.childCount;
        Task currentTask = turnOnScenario;
        for (int i = 0; i < numPages; i++)
        {
            Transform page = scenarioTransform.GetChild(i);
            SetObjectStatus turnOnPage = new SetObjectStatus(true, page.gameObject);
            currentTask.Then(turnOnPage);
            currentTask = turnOnPage;
            int numPanels = page.childCount;
            for (int j = 0; j < numPanels; j++)
            {
                SlideInPanel slideInPanel = new SlideInPanel(page.GetChild(j).gameObject, true, comicShiftArray[comicNum, i, j], 
                    Services.ComicPanelManager.panelAppearTime);
                currentTask.Then(slideInPanel);
                currentTask = slideInPanel;
            }
            WaitToContinueFromComic waitToContinue = new WaitToContinueFromComic(page.gameObject, Services.ComicPanelManager.continueButton,
                Services.TransitionUIManager.readyPromptGrowTime, Services.TransitionUIManager.readyPromptShrinkTime);
            currentTask.Then(waitToContinue);
            currentTask = waitToContinue;
        }
        SetObjectStatus turnOffDialogueBox = new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer);
        SetObjectStatus turnOffBackground = new SetObjectStatus(false, Services.ComicPanelManager.comicBackground);
        currentTask
            .Then(turnOffDialogueBox)
            .Then(turnOffBackground);

        currentTask = turnOffBackground;

        return currentTask;
    }

    Task RoundSequence(Task precedingTask)
    {
        Task rpsSequence = RpsSequence(precedingTask);
        Task dialogueExchangeSequence = DialogueExchangeSequence(rpsSequence);

        return dialogueExchangeSequence;
    }

    Task RpsSequence(Task precedingTask)
    {
        SlideInCrowd slideInCrowd = new SlideInCrowd();
        ShowRpsDialogueOptions showOptions = new ShowRpsDialogueOptions();
        WaitForRpsDialogueSelection waitForSelection = new WaitForRpsDialogueSelection();
        TransitionFromSelectionToDialogue transition = new TransitionFromSelectionToDialogue();
        PopUpDialogueBox popUpDialogue1 = new PopUpDialogueBox(false);
        TypeRpsDialogue loserDialogue = new TypeRpsDialogue();
        WaitToContinueDialogue waitForInput1 = new WaitToContinueDialogue();
        PopUpDialogueBox popUpDialogue2 = new PopUpDialogueBox(false);
        TypeRpsDialogue winnerDialogue = new TypeRpsDialogue();
        WaitToContinueDialogue waitForInput2 = new WaitToContinueDialogue();
        PopUpDialogueBox popUpDialogue3 = new PopUpDialogueBox(true);
        TypeDialogue crowdReaction = new TypeDialogue(true);
        WaitToContinueDialogue waitForInput3 = new WaitToContinueDialogue();

        precedingTask
            .Then(slideInCrowd)
            .Then(showOptions)
            .Then(waitForSelection)
            .Then(transition)
            .Then(popUpDialogue1)
            .Then(loserDialogue)
            .Then(waitForInput1)
            .Then(popUpDialogue2)
            .Then(winnerDialogue)
            .Then(waitForInput2)
            .Then(popUpDialogue3)
            .Then(crowdReaction)
            .Then(waitForInput3);

        return waitForInput3;
    }

    Task DialogueExchangeSequence(Task precedingTask){
        SetObjectStatus turnOffDialogueBox1 = new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer);
        ShowDialogueOptions showFirstOptions = new ShowDialogueOptions (true);
		WaitForDialogueChoiceTask waitForFirstChoice = new WaitForDialogueChoiceTask ();
		HighlightSelectedOption highlightFirstChoice = new HighlightSelectedOption ();
        ActionTask posePlayerAfterFirstChoice = new ActionTask(Services.DialogueUIManager.SetPose);
        PopUpDialogueBox popUpDialogue1 = new PopUpDialogueBox(false);
        TypeDialogue typeFirstDialogue = new TypeDialogue (false);
		WaitToContinueDialogue waitAfterFirstDialogue = new WaitToContinueDialogue ();
        ShowAbilityPicked showFirstAbility = new ShowAbilityPicked();
        SetObjectStatus turnOffDialogueBox2 = new SetObjectStatus(false, Services.DialogueUIManager.dialogueContainer);
		ShowDialogueOptions showSecondOptions = new ShowDialogueOptions (false);
		WaitForDialogueChoiceTask waitForSecondChoice = new WaitForDialogueChoiceTask ();
		HighlightSelectedOption highlightSecondChoice = new HighlightSelectedOption ();
        ActionTask posePlayerAfterSecondChoice = new ActionTask(Services.DialogueUIManager.SetPose);
        PopUpDialogueBox popUpDialogue2 = new PopUpDialogueBox(false);
        TypeDialogue typeSecondDialogue = new TypeDialogue (false);
		WaitToContinueDialogue waitAfterSecondDialogue = new WaitToContinueDialogue ();
        ShowAbilityPicked showSecondAbility = new ShowAbilityPicked();
		DialogueTransitionTask transition = new DialogueTransitionTask ();

        precedingTask
            .Then(turnOffDialogueBox1)
            .Then(showFirstOptions)
            .Then(waitForFirstChoice)
            .Then(highlightFirstChoice)
            .Then(posePlayerAfterFirstChoice)
            .Then(popUpDialogue1)
            .Then(typeFirstDialogue)
            .Then(waitAfterFirstDialogue)
            //.Then(showFirstAbility)
            .Then(turnOffDialogueBox2)
            .Then(showSecondOptions)
            .Then(waitForSecondChoice)
            .Then(highlightSecondChoice)
            .Then(posePlayerAfterSecondChoice)
            .Then(popUpDialogue2)
            .Then(typeSecondDialogue)
            .Then(waitAfterSecondDialogue)
            //.Then(showSecondAbility)
            .Then(transition);

        return transition;
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

	Task TransitionToFightSequence(Task precedingTask){
		Services.GameInfo.player1Abilities = abilityLists[0];
		Services.GameInfo.player2Abilities = abilityLists[1];

        SlideOutCrowd slideOutCrowd = new SlideOutCrowd();
        SlideInFightBackground slideInBG = new SlideInFightBackground ();
		ShowFightinWords showWords = new ShowFightinWords ();
		WaitForReady waitForReady = new WaitForReady ();
		ScaleOutTransitionUI scaleOut = new ScaleOutTransitionUI ();
		FinishTransition finish = new FinishTransition ();
        ActionTask startFight = new ActionTask(StartFight);

        precedingTask
            .Then(slideOutCrowd)
            .Then(slideInBG)
            .Then(showWords)
            .Then(waitForReady)
            .Then(scaleOut)
            .Then(finish)
            .Then(startFight);

        return startFight;
	}

    void StartFight()
    {
        Services.SceneStackManager.PushScene<FightScene>(new TransitionData(currentRoundNum));
    }
}
