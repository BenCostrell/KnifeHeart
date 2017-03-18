using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNovelSceneManager : MonoBehaviour {

	public TextAsset dialogueFile;
	private List<Ability.Type> abilityPool;
	private List<Ability.Type> currentRoundAbilityPool;
	private List<Ability.Type> abilityList_P1;
	private List<Ability.Type> abilityList_P2;
	public int currentTurnPlayerNum;
	public int currentRoundNum;

	void Start () {
		GenerateDialogueData ();
		InitializeAbilityPool ();
		Services.DialogueUIManager.SetUpUI ();
		Services.EventManager.Register<DialoguePicked> (PickAbility);
		currentRoundNum = 1;
		BeginDialogueSequence ();
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

	void BeginDialogueSequence(){
		TypeDialogue typeInitialDialogue = new TypeDialogue (false, true);
		WaitForAnyInput waitForInput = new WaitForAnyInput ();
		StartDialogueExchange startRound = new StartDialogueExchange ();
		typeInitialDialogue
			.Then (waitForInput)
			.Then (startRound);
		Services.TaskManager.AddTask (typeInitialDialogue);
	}

	public void StartRound(){
		currentTurnPlayerNum = 1;

		ShowDialogueOptions showFirstOptions = new ShowDialogueOptions (true);
		WaitForDialogueChoiceTask waitForFirstChoice = new WaitForDialogueChoiceTask ();
		HighlightSelectedOption highlightFirstChoice = new HighlightSelectedOption ();
		TypeDialogue typeFirstDialogue = new TypeDialogue (true, false);
		WaitForAnyInput waitAfterFirstDialogue = new WaitForAnyInput ();
		ShowDialogueOptions showSecondOptions = new ShowDialogueOptions (false);
		WaitForDialogueChoiceTask waitForSecondChoice = new WaitForDialogueChoiceTask ();
		HighlightSelectedOption highlightSecondChoice = new HighlightSelectedOption ();
		TypeDialogue typeSecondDialogue = new TypeDialogue (false, false);
		WaitForAnyInput waitAfterSecondDialogue = new WaitForAnyInput ();
		DialogueTransitionTask transition = new DialogueTransitionTask ();

		showFirstOptions
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

		Services.TaskManager.AddTask (showFirstOptions);
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

	public void TransitionToFight(){
		Services.GameInfo.player1Abilities = abilityList_P1;
		Services.GameInfo.player2Abilities = abilityList_P2;

		SlideInFightBackground slideInBG = new SlideInFightBackground ();
		ShowFightinWords showWords = new ShowFightinWords ();
		WaitForReady waitForReady = new WaitForReady ();
		ScaleOutTransitionUI scaleOut = new ScaleOutTransitionUI ();
		FinishTransition finish = new FinishTransition ();

		slideInBG
			.Then (showWords)
			.Then (waitForReady)
			.Then (scaleOut)
			.Then (finish);

		Services.TaskManager.AddTask (slideInBG);
	}
}
