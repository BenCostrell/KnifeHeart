using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeRpsDialogue : Task {
    private string dialogueText;
    private int characterIndex;

    protected override void Init()
    {
        int playerNum = Services.VisualNovelScene.currentTurnPlayerNum;
        characterIndex = 0;
        Services.DialogueUIManager.dialogueContainer.SetActive(true);
        if (playerNum == Services.VisualNovelScene.initiatingPlayer)
        {
            dialogueText = Services.VisualNovelScene.rpsDialogueArray[1];
        }
        else
        {
            dialogueText = Services.VisualNovelScene.rpsDialogueArray[0];
        }
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text = "";
        Services.DialogueUIManager.dialogueTextBox.GetComponent<Image>().sprite =
                Services.DialogueUIManager.dialogueTextBoxImages[playerNum - 1];
    }

    internal override void Update()
    {
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text += dialogueText[characterIndex];
        characterIndex += 1;
        if (Services.DialogueUIManager.dialogueText.GetComponent<Text>().text == dialogueText)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.VisualNovelScene.ChangePlayerTurn();
    }

}
