using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public TextAsset dialogueFile;
	private List<Ability.Type> abilityPool;


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
	}

	Ability.Type GetRandomAbility(){
		int index = Random.Range (0, abilityPool.Count);
		Ability.Type ability = abilityPool [index];
		abilityPool.Remove (ability);
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
			option1 = Services.DialogueDataManager.GetDialogue (GetRandomAbility ());
			option2 = Services.DialogueDataManager.GetDialogue (GetRandomAbility ());
			option3 = Services.DialogueDataManager.GetDialogue (GetRandomAbility ());
			option4 = Services.DialogueDataManager.GetDialogue (GetRandomAbility ());
		}

		Services.DialogueUIManager.SetDialogueOptions (option1, option2, option3, option4);
	}

	void Reset(Reset e){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}
