using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelectedOption : Task {
	private GameObject selectedOption;
	private float timeElapsed;

	protected override void Init ()
	{
		selectedOption = Services.DialogueUIManager.selectedOption;
	}

	internal override void Update ()
	{
		GameObject[] options = Services.DialogueUIManager.optionObjects;
		float shrinkTime = Services.DialogueUIManager.unselectedOptionShrinkTime;
		float highlightTime = Services.DialogueUIManager.selectedOptionHighlightTime;
		for (int i = 0; i < options.Length; i++) {
			if (options [i] != selectedOption) {
				options [i].transform.localScale = Vector3.Lerp (Vector3.one, Vector3.zero, Easing.ExpoEaseOut (timeElapsed / shrinkTime));
			} else {
				options [i].transform.localScale = Vector3.Lerp (Vector3.one, 1.2f * Vector3.one, Easing.ExpoEaseOut (timeElapsed / highlightTime));
			}
		}

		timeElapsed += Time.deltaTime;

		if (timeElapsed >= highlightTime) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		Services.DialogueUIManager.SetOptionUIStatus (false);
	}
}
