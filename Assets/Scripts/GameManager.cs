using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public TextAsset dialogueFile;


	// Use this for initialization
	void Start () {
		InitializeServices ();
		GenerateDialogueData ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void InitializeServices(){
		Services.EventManager = new EventManager ();
		Services.GameManager = this;
		Services.PrefabDB = Resources.Load<PrefabDB> ("Prefabs/PrefabDB");
		Services.TaskManager = new TaskManager ();
		Services.DialogueDataManager = new DialogueDataManager ();
		Services.DialogueUIManager = GameObject.FindGameObjectWithTag ("DialogueUIManager").GetComponent<DialogueUIManager> ();
		Services.GameInfo = new GameInfo ();
	}

	void GenerateDialogueData(){
		Services.DialogueDataManager.ParseDialogueFile (dialogueFile);
	}

	void StartRound(int roundNum){
		
	}
}
