using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFightinWords : Task {

	private float timeElapsed;
    private List<Transform> wordObjects;

	protected override void Init ()
	{
		Services.TransitionUIManager.transitionUI.SetActive (true);
		Services.TransitionUIManager.readyPrompt_P1.SetActive (false);
		Services.TransitionUIManager.readyPrompt_P2.SetActive (false);
		Services.TransitionUIManager.ready_P1.SetActive (false);
		Services.TransitionUIManager.ready_P2.SetActive (false);

        wordObjects = new List<Transform>();

        GameObject wordContainerForThisRound = Services.TransitionUIManager.wordGroups[Services.VisualNovelScene.currentRoundNum - 1];
        foreach(Image image in wordContainerForThisRound.GetComponentsInChildren<Image>(true))
        {
            wordObjects.Add(image.transform);
        }

        for (int i = 0; i < wordObjects.Count; i++)
        {
            wordObjects[i].gameObject.SetActive(true);
            wordObjects[i].localScale = Vector3.zero;
        }
		timeElapsed = 0;
	}

	internal override void Update ()
	{
		float duration = Services.TransitionUIManager.fightWordGrowthTime;
		float staggerTime = Services.TransitionUIManager.fightWordStaggerTime;
		float totalDuration = duration + ((wordObjects.Count - 1) * staggerTime);
		for (int i = 0; i < wordObjects.Count; i++) {
			float totalStaggerTime = i * staggerTime;
			if ((timeElapsed <= duration + totalStaggerTime) && (timeElapsed >= totalStaggerTime)) {
				wordObjects[i].localScale = Vector3.LerpUnclamped (Vector3.zero, Vector3.one, 
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
		//Services.DialogueUIManager.ponytail.GetComponent<Animator> ().SetTrigger ("getAngry");
		//Services.DialogueUIManager.pigtails.GetComponent<Animator> ().SetTrigger ("getAngry");
	}
}
