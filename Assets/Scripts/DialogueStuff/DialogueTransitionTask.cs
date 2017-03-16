using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTransitionTask : Task {

	protected override void Init ()
	{
		Services.GameManager.currentRoundNum += 1;
	}

	internal override void Update ()
	{
		SetStatus (TaskStatus.Success);
	}

	protected override void OnSuccess ()
	{
		if (Services.GameManager.currentRoundNum < 4) {
			Services.GameManager.StartRound ();
		}
	}
}
