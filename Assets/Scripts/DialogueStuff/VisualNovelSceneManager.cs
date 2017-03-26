using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNovelSceneManager : MonoBehaviour {

	public TextAsset dialogueFile;
    public TextAsset rpsDialogueFile;
	private List<Ability.Type> abilityPool;
	private List<Ability.Type> currentRoundAbilityPool;
	private List<Ability.Type> abilityList_P1;
	private List<Ability.Type> abilityList_P2;
	public int currentTurnPlayerNum;
	public int currentRoundNum;
    public int initiatingPlayer;
    public string[] rpsDialogueArray;

	void Start () {
		GenerateDialogueData ();
        GenerateRpsDialogueData();
		InitializeAbilityPool ();
		Services.DialogueUIManager.SetUpUI ();
		Services.EventManager.Register<DialoguePicked> (PickAbility);
		currentRoundNum = 1;
		BeginVisualNovelSequence ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void InitializeAbilityPool(){
		abilityPool = new List<Ability.Type> () { 
			Ability.Type.Fireball, 
			Ability.Type.Lunge, 
			Ability.Type.Shield, 
			Ability.Type.Sing, 
			Ability.Type.Wallop, 
			Ability.Type.Pull
		};
		abilityList_P1 = new List<Ability.Type> ();
		abilityList_P2 = new List<Ability.Type> ();
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

	void BeginVisualNovelSequence(){
        ShowStartScreen showStartScreen = new ShowStartScreen ();
        Task introSequence = IntroSequence(showStartScreen);
        Task round1Sequence = RoundSequence(introSequence);
        Task round2Sequence = RoundSequence(round1Sequence);
        Task round3Sequence = RoundSequence(round2Sequence);
        
		Services.TaskManager.AddTask (showStartScreen);
	}

    Task IntroSequence(Task precedingTask)
    {
        SlideInPanel slidePanel1_1 = new SlideInPanel(Services.DialogueUIManager.introSequence.transform.GetChild(1).gameObject, true);
        SlideInPanel showPanel1_2 = new SlideInPanel(Services.DialogueUIManager.introSequence.transform.GetChild(2).gameObject, false);
        SlideInPanel slidePanel2_1 = new SlideInPanel(Services.DialogueUIManager.introSequence.transform.GetChild(3).gameObject, true);
        SlideInPanel showPanel2_2 = new SlideInPanel(Services.DialogueUIManager.introSequence.transform.GetChild(4).gameObject, false);
        WaitForAnyInput waitForInput = new WaitForAnyInput();
        FinishIntroSequence finishIntroSeq = new FinishIntroSequence();

        precedingTask
            .Then(slidePanel1_1)
            .Then(showPanel1_2)
            .Then(slidePanel2_1)
            .Then(showPanel2_2)
            .Then(waitForInput)
            .Then(finishIntroSeq);

        return finishIntroSeq;
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
        TypeRpsDialogue loserDialogue = new TypeRpsDialogue();
        WaitForAnyInput waitForInput1 = new WaitForAnyInput();
        TypeRpsDialogue winnerDialogue = new TypeRpsDialogue();
        WaitForAnyInput waitForInput2 = new WaitForAnyInput();
        TypeDialogue crowdReaction = new TypeDialogue(true);
        WaitForAnyInput waitForInput3 = new WaitForAnyInput();

        precedingTask
            .Then(slideInCrowd)
            .Then(showOptions)
            .Then(waitForSelection)
            .Then(transition)
            .Then(loserDialogue)
            .Then(waitForInput1)
            .Then(winnerDialogue)
            .Then(waitForInput2)
            .Then(crowdReaction)
            .Then(waitForInput3);

        return waitForInput3;
    }

    Task DialogueExchangeSequence(Task precedingTask){
		ShowDialogueOptions showFirstOptions = new ShowDialogueOptions (true);
		WaitForDialogueChoiceTask waitForFirstChoice = new WaitForDialogueChoiceTask ();
		HighlightSelectedOption highlightFirstChoice = new HighlightSelectedOption ();
		TypeDialogue typeFirstDialogue = new TypeDialogue (false);
		WaitForAnyInput waitAfterFirstDialogue = new WaitForAnyInput ();
		ShowDialogueOptions showSecondOptions = new ShowDialogueOptions (false);
		WaitForDialogueChoiceTask waitForSecondChoice = new WaitForDialogueChoiceTask ();
		HighlightSelectedOption highlightSecondChoice = new HighlightSelectedOption ();
		TypeDialogue typeSecondDialogue = new TypeDialogue (false);
		WaitForAnyInput waitAfterSecondDialogue = new WaitForAnyInput ();
		DialogueTransitionTask transition = new DialogueTransitionTask ();

		precedingTask
            .Then(showFirstOptions)
			.Then (waitForFirstChoice)
			.Then (highlightFirstChoice)
			.Then (typeFirstDialogue)
			.Then (waitAfterFirstDialogue)
			.Then (showSecondOptions)
			.Then (waitForSecondChoice)
			.Then (highlightSecondChoice)
			.Then (typeSecondDialogue)
			.Then (waitAfterSecondDialogue)
			.Then (transition);

        return transition;
	}

    public void GenerateDialogueOptions(bool firstChoice){
		Dialogue[] dialogueOptions = new Dialogue[4];
		List<Ability.Type> abilityList;
		List<Ability.Type> playerContext = null;
		List<Ability.Type> fullAbilityKey;

		if (currentTurnPlayerNum == 1) {
			playerContext = new List<Ability.Type> (abilityList_P1);
		} else if (currentTurnPlayerNum == 2) {
			playerContext = new List<Ability.Type> (abilityList_P2);
		}

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
			dialogueOptions [i] = Services.DialogueDataManager.GetDialogue (fullAbilityKey);
		}

		Services.DialogueUIManager.SetDialogueOptions (dialogueOptions);
	}

	void PickAbility(DialoguePicked e){
		int playerNum = e.pickedByPlayerNum;
		Ability.Type ability = e.dialogue.abilityGiven;
		if (playerNum == 1) {
			abilityList_P1.Add (ability);
		} else if (playerNum == 2) {
			abilityList_P2.Add (ability);
		}
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

        if (choice_P1 == "ROCK")
        {
            if (choice_P2 == "ROCK")
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
            else if (choice_P2 == "PAPER")
            {
                Debug.Log("player 2 beats rock with paper");
                winningPlayerNum = 2;
            }
            else if (choice_P2 == "SCISSORS")
            {
                Debug.Log("player 1 beats scissors with rock");
                winningPlayerNum = 1;
            }
        }
        else if (choice_P1 == "PAPER")
        {
            if (choice_P2 == "ROCK")
            {
                Debug.Log("player 1 beats rock with paper");
                winningPlayerNum = 1;
            }
            else if (choice_P2 == "PAPER")
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
            else if (choice_P2 == "SCISSORS")
            {
                Debug.Log("player 2 beats paper with scissors");
                winningPlayerNum = 2;
            }
        }
        else if (choice_P1 == "SCISSORS")
        {
            if (choice_P2 == "ROCK")
            {
                Debug.Log("player 2 beats scissors with rock");
                winningPlayerNum = 2;
            }
            else if (choice_P2 == "PAPER")
            {
                Debug.Log("player 1 beats paper with scissors");
                winningPlayerNum = 1;
            }
            else if (choice_P2 == "SCISSORS")
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

	public void TransitionToFight(){
		Services.GameInfo.player1Abilities = abilityList_P1;
		Services.GameInfo.player2Abilities = abilityList_P2;

        SlideOutCrowd slideOutCrowd = new SlideOutCrowd();
        SlideInFightBackground slideInBG = new SlideInFightBackground ();
		ShowFightinWords showWords = new ShowFightinWords ();
		WaitForReady waitForReady = new WaitForReady ();
		ScaleOutTransitionUI scaleOut = new ScaleOutTransitionUI ();
		FinishTransition finish = new FinishTransition ();

        slideOutCrowd
            .Then(slideInBG)
            .Then(showWords)
            .Then(waitForReady)
            .Then(scaleOut)
            .Then(finish);

		Services.TaskManager.AddTask (slideOutCrowd);
	}
}
