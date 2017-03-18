using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDialogueOptions : Task {

	private bool firstChoice;
	private float timeElapsed;
	private int numAvailOptions;

	public ShowDialogueOptions(bool firstChc){
		firstChoice = firstChc;
		timeElapsed = 0;
	}

	protected override void Init ()
	{
		Services.VisualNovelSceneManager.GenerateDialogueOptions (firstChoice);
		GameObject[] optObjects = Services.DialogueUIManager.optionObjects;
		numAvailOptions = 0;
		for (int i = 0; i < optObjects.Length; i++) {
			if (optObjects [i].GetComponentInChildren<Text> ().text != "") {
				numAvailOptions += 1;
				optObjects [i].SetActive (true);
				optObjects [i].transform.localScale = Vector3.zero;
			} else {
				optObjects [i].SetActive (false);
			}
		}
	}

	internal override void Update ()
	{
		float duration = Services.DialogueUIManager.optionAppearanceTime;
		float staggerTime = Services.DialogueUIManager.optionAppearanceStaggerTime;
		float totalDuration = duration + ((numAvailOptions - 1) * staggerTime);
		GameObject[] optObjects = Services.DialogueUIManager.optionObjects;
		for (int i = 0; i < numAvailOptions; i++) {
			float totalStaggerTime = i * staggerTime;
			GameObject obj = optObjects [i];
			if ((timeElapsed <= duration + totalStaggerTime) && (timeElapsed >= totalStaggerTime)) {
				obj.transform.localScale = Vector3.LerpUnclamped (Vector3.zero, Vector3.one, 
					Easing.BackEaseOut ((timeElapsed - totalStaggerTime) / duration));
			}
		} 
			
		timeElapsed = Mathf.Min (totalDuration, timeElapsed + Time.deltaTime);

		if (timeElapsed == totalDuration) {
			SetStatus (TaskStatus.Success);
		}

	}
}
