using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowStartScreen : Task {

	protected override void Init ()
	{
		Services.DialogueUIManager.startScreen.SetActive (true);
		Services.EventManager.Register<ButtonPressed> (Continue);
	}

	void Continue(ButtonPressed e){
		SetStatus (TaskStatus.Success);
	}

	protected override void OnSuccess ()
	{
		Services.DialogueUIManager.startScreen.SetActive (false);
		Services.EventManager.Unregister<ButtonPressed> (Continue);
        Services.DialogueUIManager.introSequence.SetActive(true);
	}
}
