using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOutTransitionUI : Task {
	private float timeElapsed;
	private List<GameObject> objectsToScale;

	protected override void Init ()
	{
		timeElapsed = 0;
		objectsToScale = new List<GameObject> ();
		objectsToScale.Add (Services.TransitionUIManager.wordsContainer);
		objectsToScale.Add (Services.TransitionUIManager.ready_P1);
		objectsToScale.Add (Services.TransitionUIManager.ready_P2);
	}

	internal override void Update ()
	{
		float duration = Services.TransitionUIManager.uiScaleOutTime;
		foreach (GameObject obj in objectsToScale) {
			obj.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.zero, 
				Easing.BackEaseIn (timeElapsed / duration));
		}
		timeElapsed += Time.deltaTime;

		if (objectsToScale[0].transform.localScale == Vector3.zero) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		Services.TransitionUIManager.transitionUI.SetActive (false);
	}

}
