using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDialogueChoiceTask : Task {

	protected override void Init ()
	{
		Services.EventManager.Register<ButtonPressed> (OnInputReceived);
        Services.EventManager.Register<AxisPressed>(OnAxisPressed);
	}

	internal override void Update ()
	{
		base.Update ();
	}

	protected override void OnSuccess ()
	{
        Services.EventManager.Unregister<ButtonPressed>(OnInputReceived);
    }

    private void OnAxisPressed(AxisPressed e)
    {
        if (e.playerNum == Services.VisualNovelScene.currentTurnPlayerNum)
        {
            Services.EventManager.Unregister<AxisPressed>(OnAxisPressed);
            RotateDialogueOptions rotateDialogue = new RotateDialogueOptions(Services.DialogueUIManager.dialogueRotationTime, -e.direction);
            ActionTask reregister = new ActionTask(Reregister);
            rotateDialogue.Then(reregister);
            Services.TaskManager.AddTask(rotateDialogue);
        }
    }

    void Reregister()
    {
        Services.EventManager.Register<AxisPressed>(OnAxisPressed);
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
