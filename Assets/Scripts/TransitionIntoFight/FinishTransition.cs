using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTransition : Task {
	private Vector3 ponytailInitialPosition;
	private Vector3 pigtailsInitialPosition;
	private float timeElapsed;

	protected override void Init ()
	{
		ponytailInitialPosition = Services.DialogueUIManager.ponytail.transform.position;
		pigtailsInitialPosition = Services.DialogueUIManager.pigtails.transform.position;
		Services.TransitionUIManager.transitionUI.SetActive (false);
	}

	internal override void Update ()
	{
		float duration = Services.TransitionUIManager.transitionEndTime;
		Services.DialogueUIManager.ponytail.transform.position = Vector3.Lerp (ponytailInitialPosition, ponytailInitialPosition + 4 * Vector3.left, 
			Easing.BackEaseIn (timeElapsed / duration));
		Services.DialogueUIManager.pigtails.transform.position = Vector3.Lerp (pigtailsInitialPosition, pigtailsInitialPosition + 4 * Vector3.right, 
			Easing.BackEaseIn (timeElapsed / duration));
		timeElapsed += Time.deltaTime;

		if (timeElapsed > duration) {
			SetStatus (TaskStatus.Success);
		}
	}
}
