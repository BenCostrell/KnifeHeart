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
        Services.EventManager.Unregister<ButtonPressed>(OnInputReceived);
    }

    private void OnInputReceived(ButtonPressed e){
		if (e.playerNum == Services.VisualNovelScene.currentTurnPlayerNum) {
			Dialogue dialogueSelected = Services.DialogueUIManager.GetDialogueFromInput (e.buttonTitle);
			GameObject optionObjectSelected = Services.DialogueUIManager.GetOptionObjectFromInput (e.buttonTitle);
			if (dialogueSelected != null) {
				Services.EventManager.Fire (new DialoguePicked (dialogueSelected, e.playerNum, optionObjectSelected));
				SetStatus (TaskStatus.Success);
			}
		}
	}
}
