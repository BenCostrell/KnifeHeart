using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInFightBackground : Task {

	private float timeElapsed;

	protected override void Init ()
	{
		Services.TransitionUIManager.fightBackground.SetActive (true);
		Services.DialogueUIManager.dialogueContainer.SetActive (false);
	}

	internal override void Update ()
	{
		float duration = Services.TransitionUIManager.fightBackgroundSlideInTime;
		Services.TransitionUIManager.fightBackground.transform.position = Vector3.Lerp (13 * Vector3.right, Vector3.zero, 
			Easing.QuadEaseIn (timeElapsed / duration));
		timeElapsed += Time.deltaTime;

		if (Services.TransitionUIManager.fightBackground.transform.position == Vector3.zero) {
			SetStatus (TaskStatus.Success);
		}
	}
}
