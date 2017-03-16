using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeDialogue : Task {
	private string dialogueText;
	private int characterIndex;

	protected override void Init ()
	{
		dialogueText = Services.DialogueUIManager.queuedDialogue.mainText;
		characterIndex = 0;
		Services.DialogueUIManager.dialogueText.GetComponent<Text> ().text = "";
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
		Services.GameManager.ChangePlayerTurn ();
	}
}
