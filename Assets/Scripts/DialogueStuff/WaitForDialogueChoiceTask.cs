using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDialogueChoiceTask : Task {

	private bool firstChoice;

	public WaitForDialogueChoiceTask(bool firstChc){
		firstChoice = firstChc;
	}


	protected override void Init ()
	{
		Services.GameManager.GenerateDialogueOptions (firstChoice);
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
			if (dialogueSelected != null) {
				Services.EventManager.Fire (new DialoguePicked (dialogueSelected, e.playerNum));
				SetStatus (TaskStatus.Success);
				Services.EventManager.Unregister<ButtonPressed> (OnInputReceived);
			}
		}
	}
}
