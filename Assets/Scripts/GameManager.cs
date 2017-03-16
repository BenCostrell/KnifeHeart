using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public TextAsset dialogueFile;
	private List<Ability.Type> abilityPool;
	private List<Ability.Type> abilityList_P1;
	private List<Ability.Type> abilityList_P2;
	public int currentTurnPlayerNum;


	// Use this for initialization
	void Start () {
		InitializeServices ();
		GenerateDialogueData ();
		InitializeAbilityPool ();
		Services.DialogueUIManager.SetUpUI ();
		Services.EventManager.Register<Reset> (Reset);

		StartRound (1);
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
		int index = Random.Range (0, abilityList.Count);
		Ability.Type ability = abilityList [index];
		abilityList.Remove (ability);
		return ability;
	}

	void GenerateDialogueData(){
		Services.DialogueDataManager.ParseDialogueFile (dialogueFile);
	}

	void StartRound(int roundNum){
		Dialogue option1 = null;
		Dialogue option2 = null;
		Dialogue option3 = null;
		Dialogue option4 = null;

		if (roundNum == 1) {
			List<Ability.Type> abilityList = new List<Ability.Type> (abilityPool);
			option1 = Services.DialogueDataManager.GetDialogue (GetRandomAbility (abilityList));
			option2 = Services.DialogueDataManager.GetDialogue (GetRandomAbility (abilityList));
			option3 = Services.DialogueDataManager.GetDialogue (GetRandomAbility (abilityList));
			option4 = Services.DialogueDataManager.GetDialogue (GetRandomAbility (abilityList));
		}

		Services.DialogueUIManager.SetDialogueOptions (option1, option2, option3, option4);

		currentTurnPlayerNum = 1;

		WaitForDialogueChoiceTask waitForFirstChoice = new WaitForDialogueChoiceTask ();
		TypeDialogue typeFirstDialogue = new TypeDialogue ();
		WaitForDialogueChoiceTask waitForSecondChoice = new WaitForDialogueChoiceTask ();
		TypeDialogue typeSecondDialogue = new TypeDialogue ();

		waitForFirstChoice
			.Then (typeFirstDialogue)
			.Then (waitForSecondChoice)
			.Then (typeSecondDialogue);

		Services.TaskManager.AddTask (waitForFirstChoice);
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
	}

	public void ChangePlayerTurn(){
		if (currentTurnPlayerNum == 1) {
			currentTurnPlayerNum = 2;
		} else if (currentTurnPlayerNum == 2) {
			currentTurnPlayerNum = 1;
		}
	}
}
