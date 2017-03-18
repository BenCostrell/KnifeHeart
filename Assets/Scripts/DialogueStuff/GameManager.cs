using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		if (Services.EventManager == null) {
			InitializeUniversalServices ();
		}
		if (SceneManager.GetActiveScene ().name == "VisualNovelScene") {
			InitializeVNServices ();
		} else if (SceneManager.GetActiveScene ().name == "fightRoom") {
			InitializeFightServices ();
			// for testing purposes only
			if (Services.GameInfo.player1Abilities.Count == 0) {
				SetPlayerAbilities ();
			}
		}

		Services.EventManager.Register<Reset> (Reset);
	}
	
	// Update is called once per frame
	void Update () {
		Services.InputManager.GetInput ();
		Services.TaskManager.Update ();
	}

	void InitializeUniversalServices(){
		Services.EventManager = new EventManager ();
		Services.GameManager = this;
		Services.PrefabDB = Resources.Load<PrefabDB> ("Prefabs/PrefabDB");
		Services.TaskManager = new TaskManager ();
		Services.GameInfo = new GameInfo ();
		Services.InputManager = new InputManager ();
	}

	void InitializeVNServices(){
		Services.DialogueDataManager = new DialogueDataManager ();
		Services.DialogueUIManager = GameObject.FindGameObjectWithTag ("DialogueUIManager").GetComponent<DialogueUIManager> ();
		Services.TransitionUIManager = GameObject.FindGameObjectWithTag ("TransitionUIManager").GetComponent<TransitionUIManager> ();
		Services.VisualNovelSceneManager = GameObject.FindGameObjectWithTag ("VisualNovelSceneManager").GetComponent<VisualNovelSceneManager> ();
	}

	void InitializeFightServices(){
		Services.FightSceneManager = GameObject.FindGameObjectWithTag ("FightSceneManager").GetComponent<FightSceneManager> ();
		Services.FightUIManager = GameObject.FindGameObjectWithTag ("FightUIManager").GetComponent<FightUIManager> ();
	}

	void Reset(Reset e){
		if (Input.GetButton ("HardReset")) {
			SceneManager.LoadScene ("VisualNovelStage");
		} else {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	// for testing purposes only
	void SetPlayerAbilities (){
		Services.GameInfo.player1Abilities = new List<Ability.Type> () {
			Ability.Type.Fireball,
			Ability.Type.Lunge,
			Ability.Type.Pull
		};
		Services.GameInfo.player2Abilities = new List<Ability.Type> () {
			Ability.Type.Shield,
			Ability.Type.Sing,
			Ability.Type.Wallop
		};
	}


}
