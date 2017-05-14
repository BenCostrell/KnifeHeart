using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeDialogue : Task {
	private string dialogueText;
	private int characterIndex;
	private bool crowdDialogue;
    private Text textComponent;

	public TypeDialogue(bool crowdDialog){
		crowdDialogue = crowdDialog;
	}

	protected override void Init ()
	{
        textComponent = Services.DialogueUIManager.dialogueText.GetComponent<Text>();
        int playerNum = Services.VisualNovelScene.currentTurnPlayerNum;
		characterIndex = 0;
        if (!crowdDialogue) {
			dialogueText = Services.DialogueUIManager.queuedDialogue.mainText;

		} else {
            dialogueText = "CROWD: " + Services.VisualNovelScene.rpsDialogueArray[2];
        }
        dialogueText = Services.DialogueDataManager.ParseTextForLineBreaks(dialogueText, textComponent);
	}

	internal override void Update ()
	{
		textComponent.text += dialogueText [characterIndex];
		characterIndex += 1;
		if (textComponent.text == dialogueText) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
    { 
	}
}
