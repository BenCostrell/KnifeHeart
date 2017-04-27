using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForReady : Task {
	private float scaleTime;
	private bool p1Ready;
	private bool p2Ready;
	private bool grow;
    private Vector3 baseScale;

	protected override void Init ()
	{
		Services.EventManager.Register<ButtonPressed> (Continue);
		scaleTime = 0;
		p1Ready = false;
		p2Ready = false;
		grow = true;
		Services.TransitionUIManager.readyPrompt_P1.SetActive (true);
		Services.TransitionUIManager.readyPrompt_P2.SetActive (true);
        baseScale = Services.TransitionUIManager.readyPrompt_P1.transform.localScale;
	}

	internal override void Update ()
	{
		GameObject indicator_P1 = Services.TransitionUIManager.readyPrompt_P1;
		GameObject indicator_P2 = Services.TransitionUIManager.readyPrompt_P2;
		GameObject readyP1 = Services.TransitionUIManager.ready_P1;
		GameObject readyP2 = Services.TransitionUIManager.ready_P2;
		float totalScaleTime;
        Vector3 startScale;
		Vector3 targetScale;
		float easedTime;
		if (grow) {
			totalScaleTime = Services.TransitionUIManager.readyPromptGrowTime;
            startScale = baseScale;
			targetScale = 1.2f * baseScale;
			easedTime = Easing.ExpoEaseOut (scaleTime / totalScaleTime);
		} else {
			totalScaleTime = Services.TransitionUIManager.readyPromptShrinkTime;
			startScale = 1.2f * baseScale;
			targetScale = baseScale;
			easedTime = Easing.QuadEaseOut (scaleTime / totalScaleTime);
		}
		if (scaleTime < totalScaleTime) {
			if (!p1Ready) {
				indicator_P1.transform.localScale = Vector3.Lerp (startScale, targetScale, easedTime);
			} 
			if (!p2Ready) {
				indicator_P2.transform.localScale = Vector3.Lerp (startScale, targetScale, easedTime);
			} 
		} else {
			grow = !grow;
			scaleTime = 0;
		}

		scaleTime += Time.deltaTime;
	}


	void Continue (ButtonPressed e){
		if (e.buttonTitle == "A") {
			if (e.playerNum == 1) {
				p1Ready = true;
				Services.TransitionUIManager.readyPrompt_P1.SetActive (false);
				Services.TransitionUIManager.ready_P1.SetActive (true);
			} else if (e.playerNum == 2) {
				p2Ready = true;
				Services.TransitionUIManager.readyPrompt_P2.SetActive (false);
				Services.TransitionUIManager.ready_P2.SetActive (true);
			}
		}
		if (p1Ready && p2Ready) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
        Services.TransitionUIManager.readyPrompt_P1.transform.localScale = baseScale;
        Services.TransitionUIManager.readyPrompt_P2.transform.localScale = baseScale;
        Services.EventManager.Unregister<ButtonPressed> (Continue); 
	}
}
