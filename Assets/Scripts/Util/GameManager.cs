using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private bool easyResetAvailable;
    [HideInInspector]
    public int[] playerRoundLosses;

	// Use this for initialization
	void Awake () {
		InitializeUniversalServices ();
        Services.MusicManager.Init();
        Services.EventManager.Register<Reset> (Reset);
        Cursor.visible = false;
        easyResetAvailable = false;
        playerRoundLosses = new int[2] { 0, 0 };
        Services.EventManager.Register<PlayerFall>(RoundLoss);
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

		//Set 16:9 for Mac
		//Screen.SetResolution (1600, 900, true);

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
        Services.MusicManager = GameObject.FindWithTag("MusicManager").GetComponent<MusicManager>();
	}

    public void SetEasyResetAvailable()
    {
        easyResetAvailable = true;
    }

	void Reset(Reset e){
		if (Input.GetButton ("SoftReset") || easyResetAvailable) {
            /*if (Services.SceneStackManager.CurrentScene != null)
            {
                if (Services.SceneStackManager.CurrentScene.GetType() == typeof(FightScene))
                {
                    if (Services.FightScene.roundNum == 3) SoftReset();
                }
                else SoftReset();
            }
            else HardReset();
        } else {*/
            HardReset();
        }
    }

    void SoftReset()
    {
        Services.EventManager.Clear();
        Services.EventManager.Register<Reset>(Reset);
        Services.TaskManager.Clear();
        Services.SceneStackManager.Swap<FightScene>(new TransitionData(3));
        Services.MusicManager.ResetMusic();
    }

    void HardReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Services.MusicManager.ResetMusic();
    }

    void RoundLoss(PlayerFall e)
    {
        playerRoundLosses[e.fallenPlayer.playerNum - 1] += 1;
    }


}
