using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDialogueChoiceTask : Task {

    private float timeSinceArrowPulse;
    private float period;
    private RectTransform upArrow;
    private RectTransform downArrow;
    private Vector2 upArrowBasePos;
    private Vector2 downArrowBasePos;

	protected override void Init ()
	{
		Services.EventManager.Register<ButtonPressed> (OnInputReceived);
        Services.EventManager.Register<AxisPressed>(OnAxisPressed);
        timeSinceArrowPulse = 0;
        upArrow = Services.DialogueUIManager.optionArrows[0].GetComponent<RectTransform>();
        downArrow = Services.DialogueUIManager.optionArrows[1].GetComponent<RectTransform>();
        upArrowBasePos = upArrow.anchoredPosition;
        downArrowBasePos = downArrow.anchoredPosition;
        period = Services.DialogueUIManager.optionArrowBounceTime;
	}

	internal override void Update ()
	{
        timeSinceArrowPulse += Time.deltaTime;

        if (timeSinceArrowPulse < period / 2)
        {
            upArrow.anchoredPosition = Vector2.Lerp(
                upArrowBasePos, 
                upArrowBasePos + Services.DialogueUIManager.optionArrowBounceDistance * Vector2.up,
                Easing.QuadEaseOut(timeSinceArrowPulse / (period / 2)));
            downArrow.anchoredPosition = Vector2.Lerp(
                downArrowBasePos,
                downArrowBasePos + Services.DialogueUIManager.optionArrowBounceDistance * Vector2.down,
                Easing.QuadEaseOut(timeSinceArrowPulse / (period / 2)));
        }
        else if (timeSinceArrowPulse < period) {
            upArrow.anchoredPosition = Vector2.Lerp(
                upArrowBasePos + Services.DialogueUIManager.optionArrowBounceDistance * Vector2.up,
                upArrowBasePos,
                Easing.QuadEaseIn((timeSinceArrowPulse - (period/2)) / (period / 2)));
            downArrow.anchoredPosition = Vector2.Lerp(
                downArrowBasePos + Services.DialogueUIManager.optionArrowBounceDistance * Vector2.down, 
                downArrowBasePos,
                Easing.QuadEaseIn((timeSinceArrowPulse - (period/2)) / (period / 2)));
        }
        else
        {
            timeSinceArrowPulse -= period;
        }

	}

	protected override void OnSuccess ()
	{
        Services.EventManager.Unregister<ButtonPressed>(OnInputReceived);
        Services.EventManager.Unregister<AxisPressed>(OnAxisPressed);
        upArrow.anchoredPosition = upArrowBasePos;
        downArrow.anchoredPosition = downArrowBasePos;
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
