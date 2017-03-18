using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTransitionTask : Task {

	protected override void Init ()
	{
		Services.VisualNovelSceneManager.currentRoundNum += 1;
	}

	internal override void Update ()
	{
		SetStatus (TaskStatus.Success);
	}

	protected override void OnSuccess ()
	{
		if (Services.VisualNovelSceneManager.currentRoundNum < 4) {
			Services.VisualNovelSceneManager.StartRound ();
		} else {
			Services.VisualNovelSceneManager.TransitionToFight ();
		}
	}
}
