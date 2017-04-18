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
        Services.EventManager.Unregister<AxisPressed>(OnAxisPressed);
    }

    private void OnAxisPressed(AxisPressed e)
    {
        if (e.playerNum == Services.VisualNovelScene.currentTurnPlayerNum)
        {
            Services.EventManager.Unregister<AxisPressed>(OnAxisPressed);
            Services.EventManager.Unregister<ButtonPressed>(OnInputReceived);
            RotateDialogueOptions rotateDialogue = new RotateDialogueOptions(Services.DialogueUIManager.dialogueRotationTime, -e.direction);
            ActionTask reregister = new ActionTask(Reregister);
            rotateDialogue.Then(reregister);
            Services.TaskManager.AddTask(rotateDialogue);
        }
    }

    void Reregister()
    {
        Services.EventManager.Register<AxisPressed>(OnAxisPressed);
        Services.EventManager.Register<ButtonPressed>(OnInputReceived);
    }

    private void OnInputReceived(ButtonPressed e){
		if (e.playerNum == Services.VisualNovelScene.currentTurnPlayerNum && e.buttonTitle == "A") {
			Dialogue dialogueSelected = Services.DialogueUIManager.GetDialogueFromSelectedOption ();
			if (dialogueSelected != null) {
				Services.EventManager.Fire (new DialoguePicked (dialogueSelected, e.playerNum));
				SetStatus (TaskStatus.Success);
			}
		}
	}
}
