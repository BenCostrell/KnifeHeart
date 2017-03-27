using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTransitionTask : Task {
    private float timeElapsed;
    private float duration;

    protected override void Init ()
	{
		Services.VisualNovelSceneManager.currentRoundNum += 1;
        timeElapsed = 0;
        duration = 1f;
	}

	internal override void Update ()
	{
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
	}

	protected override void OnSuccess ()
	{
        if (Services.VisualNovelSceneManager.currentRoundNum == 4)
        {
            Services.VisualNovelSceneManager.TransitionToFight();
        }
	}
}
