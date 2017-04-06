using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeDialogue : Task {
	private string dialogueText;
	private int characterIndex;
	private bool crowdDialogue;

	public TypeDialogue(bool crowdDialog){
		crowdDialogue = crowdDialog;
	}

	protected override void Init ()
	{
		int playerNum = Services.VisualNovelScene.currentTurnPlayerNum;
		characterIndex = 0;
        Services.DialogueUIManager.dialogueContainer.SetActive(true);
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text = "";
        if (!crowdDialogue) {
			dialogueText = Services.DialogueUIManager.queuedDialogue.mainText;
			if (playerNum == 1) {
				Services.DialogueUIManager.arrow_P1.SetActive (true);
				Services.DialogueUIManager.arrow_P2.SetActive (false);
			} else if (playerNum == 2) {
				Services.DialogueUIManager.arrow_P1.SetActive (false);
				Services.DialogueUIManager.arrow_P2.SetActive (true);
			}
		} else {
            dialogueText = "CROWD: " + Services.VisualNovelScene.rpsDialogueArray[2];
            Services.DialogueUIManager.arrow_P1.SetActive(false);
            Services.DialogueUIManager.arrow_P2.SetActive(false);
		}
	}

	internal override void Update ()
	{
		Services.DialogueUIManager.dialogueText.GetComponent<Text> ().text += dialogueText [characterIndex];
		characterIndex += 1;
		if (Services.DialogueUIManager.dialogueText.GetComponent<Text> ().text == dialogueText) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		Services.VisualNovelScene.ChangePlayerTurn ();
	}
}
