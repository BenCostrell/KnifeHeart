using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFightinWords : Task {

	private float timeElapsed;
	private int numWords;

	protected override void Init ()
	{
		Services.TransitionUIManager.transitionUI.SetActive (true);
		Services.TransitionUIManager.readyPrompt_P1.SetActive (false);
		Services.TransitionUIManager.readyPrompt_P2.SetActive (false);
		Services.TransitionUIManager.ready_P1.SetActive (false);
		Services.TransitionUIManager.ready_P2.SetActive (false);

		GameObject[] words = Services.TransitionUIManager.fightWords;
		numWords = words.Length;
		for (int i = 0; i < numWords; i++) {
			words [i].SetActive(true);
			words [i].transform.localScale = Vector3.zero;
		}
		timeElapsed = 0;
	}

	internal override void Update ()
	{
		float duration = Services.TransitionUIManager.fightWordGrowthTime;
		float staggerTime = Services.TransitionUIManager.fightWordStaggerTime;
		float totalDuration = duration + ((numWords - 1) * staggerTime);
		GameObject[] wordObjects = Services.TransitionUIManager.fightWords;
		for (int i = 0; i < numWords; i++) {
			float totalStaggerTime = i * staggerTime;
			GameObject word = wordObjects [i];
			if ((timeElapsed <= duration + totalStaggerTime) && (timeElapsed >= totalStaggerTime)) {
				word.transform.localScale = Vector3.LerpUnclamped (Vector3.zero, Vector3.one, 
					Easing.BackEaseOut ((timeElapsed - totalStaggerTime) / duration));
			}
		} 

		timeElapsed = Mathf.Min (totalDuration, timeElapsed + Time.deltaTime);

		if (timeElapsed == totalDuration) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		Services.DialogueUIManager.ponytail.GetComponent<Animator> ().SetTrigger ("getAngry");
		Services.DialogueUIManager.pigtails.GetComponent<Animator> ().SetTrigger ("getAngry");
	}
}
