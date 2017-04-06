using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInFightBackground : Task {

	private float timeElapsed;
    private float duration;
    private RectTransform rectTransform;

    protected override void Init()
    {
        Services.TransitionUIManager.fightBackground.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            bool status;
            if (i == Services.VisualNovelScene.currentRoundNum - 1)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            Services.TransitionUIManager.fightBackground.transform.GetChild(i).gameObject.SetActive(status);
        }
        Services.DialogueUIManager.dialogueContainer.SetActive(false);
        rectTransform = Services.TransitionUIManager.fightBackground.GetComponent<RectTransform>();
        duration = Services.TransitionUIManager.fightBackgroundSlideInTime;
        timeElapsed = 0;
    }

	internal override void Update ()
	{
        timeElapsed += Time.deltaTime;
		rectTransform.anchoredPosition = Vector2.Lerp (1600 * Vector2.right, Vector2.zero, Easing.QuadEaseIn (timeElapsed / duration));

		if (timeElapsed >= duration) {
			SetStatus (TaskStatus.Success);
		}
	}
}
