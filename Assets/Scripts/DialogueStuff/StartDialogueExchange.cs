using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogueExchange : Task {

	protected override void Init ()
	{
		SetStatus (TaskStatus.Success);
	}

	protected override void OnSuccess ()
	{
		Services.GameManager.StartRound ();
	}
}
