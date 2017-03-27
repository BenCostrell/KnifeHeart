using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeRpsDialogue : Task {
    private string dialogueText;
    private int characterIndex;

    protected override void Init()
    {
        int playerNum = Services.VisualNovelSceneManager.currentTurnPlayerNum;
        characterIndex = 0;
        Services.DialogueUIManager.dialogueContainer.SetActive(true);
        if (playerNum == Services.VisualNovelSceneManager.initiatingPlayer)
        {
            dialogueText = Services.VisualNovelSceneManager.rpsDialogueArray[0];
        }
        else
        {
            dialogueText = Services.VisualNovelSceneManager.rpsDialogueArray[1];
        }
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text = "";
        Services.DialogueUIManager.SetTextBoxColor(playerNum, false, true);
        if (playerNum == 1)
        {
            Services.DialogueUIManager.arrow_P1.SetActive(true);
            Services.DialogueUIManager.arrow_P2.SetActive(false);
        }
        else if (playerNum == 2)
        {
            Services.DialogueUIManager.arrow_P1.SetActive(false);
            Services.DialogueUIManager.arrow_P2.SetActive(true);
        }
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
        Services.VisualNovelSceneManager.ChangePlayerTurn();
    }

}
