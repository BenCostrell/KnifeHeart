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
        // for testing purposes, so you can load straight into the fight without picking abilities
        if (SceneManager.GetActiveScene().name == "fightRoom")
        {
            GetComponentInChildren<FightScene>().Init();
            GetComponentInChildren<FightScene>().OnEnter(new TransitionData(3));
			GameObject.Find ("PrefabEditor").SetActive (false);
        }
        else
        {
            Services.SceneStackManager.PushScene<TitleScreen>();
        }
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
		if (Input.GetButton ("SoftReset")) {
            if (Services.SceneStackManager.CurrentScene != null)
            {
                if (Services.SceneStackManager.CurrentScene.GetType() == typeof(FightScene))
                {
                    if (Services.FightScene.roundNum == 3) SoftReset();
                }
                else SoftReset();
            }
            else HardReset();
        } else {
            HardReset();
        }
    }

    void SoftReset()
    {
        Services.EventManager.Clear();
        Services.EventManager.Register<Reset>(Reset);
        Services.TaskManager.Clear();
        Services.SceneStackManager.Swap<FightScene>(new TransitionData(3));
    }

    void HardReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
