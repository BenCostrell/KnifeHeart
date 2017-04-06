using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		InitializeUniversalServices ();

		Services.EventManager.Register<Reset> (Reset);
	}
	
    void Start()
    {
        Services.SceneStackManager.PushScene<TitleScreen>();
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
        Services.SceneStackManager = new SceneStackManager<TransitionData>(GameObject.FindGameObjectWithTag("SceneRoot"), 
            Services.PrefabDB.Scenes);
	}

	void Reset(Reset e){
		Services.EventManager.Clear ();
		Services.TaskManager.Clear ();
		if (Input.GetButton ("HardReset")) {
            Services.SceneStackManager.Swap<FightScene>(new TransitionData(3));
        } else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


}
