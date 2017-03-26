using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForAnyInput : Task {
	private float timeSinceIndicatorChanged;

	protected override void Init ()
	{
		Services.EventManager.Register<ButtonPressed> (Continue);
		Services.DialogueUIManager.continueIndicator.SetActive (true);
		timeSinceIndicatorChanged = 0;
	}

	internal override void Update ()
	{
		GameObject indicator = Services.DialogueUIManager.continueIndicator;
		if (timeSinceIndicatorChanged > Services.DialogueUIManager.indicatorFlashUptime) {
			indicator.SetActive (!indicator.activeSelf);
			timeSinceIndicatorChanged = 0;
		}

		timeSinceIndicatorChanged += Time.deltaTime;
	}


	void Continue (ButtonPressed e){
		SetStatus (TaskStatus.Success);
	}

	protected override void OnSuccess ()
	{
		Services.EventManager.Unregister<ButtonPressed> (Continue);
		Services.DialogueUIManager.continueIndicator.SetActive (false);
	}
}
