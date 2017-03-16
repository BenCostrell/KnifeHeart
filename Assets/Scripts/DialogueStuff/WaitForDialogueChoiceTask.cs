using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDialogueChoiceTask : Task {
	protected override void Init ()
	{
		Services.EventManager.Register<ButtonPressed> (OnInputReceived);
	}

	internal override void Update ()
	{
		base.Update ();
	}

	protected override void OnSuccess ()
	{
	}

	private void OnInputReceived(ButtonPressed e){
		if (e.playerNum == Services.GameManager.currentTurnPlayerNum) {
			Dialogue dialogueSelected = Services.DialogueUIManager.GetDialogueFromInput (e.buttonTitle);
			Services.EventManager.Fire (new DialoguePicked (dialogueSelected, e.playerNum));
			Services.EventManager.Unregister<ButtonPressed> (OnInputReceived);
			SetStatus (TaskStatus.Success);
		}
	}
}
