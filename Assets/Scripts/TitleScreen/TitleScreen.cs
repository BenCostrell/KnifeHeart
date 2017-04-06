using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : Scene<TransitionData> {

    public GameObject titleScreen;

	// Use this for initialization
	void Start () {
        Services.TitleScreen = this;
        WaitForAnyInput waitToStart = new WaitForAnyInput();
        ActionTask startGame = new ActionTask(StartGame);

        waitToStart
            .Then(startGame);

        Services.TaskManager.AddTask(waitToStart);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartGame()
    {
        Services.SceneStackManager.Swap<VisualNovelScene>();
    }

    
}
