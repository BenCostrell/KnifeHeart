using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeDialogue : Task {
	private string dialogueText;
	private int characterIndex;
	private bool firstChoice;
	private bool initialDialogue;

	public TypeDialogue(bool firstChc, bool initDialogue){
		firstChoice = firstChc;
		initialDialogue = initDialogue;
	}

	protected override void Init ()
	{
		int playerNum = Services.VisualNovelSceneManager.currentTurnPlayerNum;
		characterIndex = 0;
		if (!initialDialogue) {
			dialogueText = Services.DialogueUIManager.queuedDialogue.mainText;
			Services.DialogueUIManager.dialogueText.GetComponent<Text> ().text = "";
			Services.DialogueUIManager.SetTextBoxColor (playerNum, false, true);
			if (playerNum == 1) {
				Services.DialogueUIManager.arrow_P1.SetActive (true);
				Services.DialogueUIManager.arrow_P2.SetActive (false);
			} else if (playerNum == 2) {
				Services.DialogueUIManager.arrow_P1.SetActive (false);
				Services.DialogueUIManager.arrow_P2.SetActive (true);
			}
		} else {
			dialogueText = Services.DialogueUIManager.initialDialogue;
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
		if (!initialDialogue && firstChoice) {
			Services.VisualNovelSceneManager.ChangePlayerTurn ();
		}
	}
}
