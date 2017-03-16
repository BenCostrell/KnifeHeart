using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public TextAsset dialogueFile;
	private List<Ability.Type> abilityPool;
	private List<Ability.Type> currentRoundAbilityPool;
	private List<Ability.Type> abilityList_P1;
	private List<Ability.Type> abilityList_P2;
	public int currentTurnPlayerNum;
	public int currentRoundNum;


	// Use this for initialization
	void Start () {
		InitializeServices ();
		GenerateDialogueData ();
		InitializeAbilityPool ();
		Services.DialogueUIManager.SetUpUI ();
		Services.EventManager.Register<Reset> (Reset);
		Services.EventManager.Register<DialoguePicked> (PickAbility);

		currentRoundNum = 1;

		StartRound ();
	}
	
	// Update is called once per frame
	void Update () {
		Services.InputManager.GetInput ();
		Services.TaskManager.Update ();
	}

	void InitializeServices(){
		Services.EventManager = new EventManager ();
		Services.GameManager = this;
		Services.PrefabDB = Resources.Load<PrefabDB> ("Prefabs/PrefabDB");
		Services.TaskManager = new TaskManager ();
		Services.DialogueDataManager = new DialogueDataManager ();
		Services.DialogueUIManager = GameObject.FindGameObjectWithTag ("DialogueUIManager").GetComponent<DialogueUIManager> ();
		Services.GameInfo = new GameInfo ();
		Services.InputManager = new InputManager ();
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

	public void StartRound(){
		currentTurnPlayerNum = 1;

		WaitForDialogueChoiceTask waitForFirstChoice = new WaitForDialogueChoiceTask (true);
		TypeDialogue typeFirstDialogue = new TypeDialogue (true);
		WaitForDialogueChoiceTask waitForSecondChoice = new WaitForDialogueChoiceTask (false);
		TypeDialogue typeSecondDialogue = new TypeDialogue (false);
		DialogueTransitionTask transition = new DialogueTransitionTask ();

		waitForFirstChoice
			.Then (typeFirstDialogue)
			.Then (waitForSecondChoice)
			.Then (typeSecondDialogue)
			.Then (transition);

		Services.TaskManager.AddTask (waitForFirstChoice);
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

	void Reset(Reset e){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
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
}
