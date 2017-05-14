using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeRpsDialogue : Task {
    private string dialogueText;
    private int characterIndex;
    private Text textComponent;

    protected override void Init()
    {
        textComponent = Services.DialogueUIManager.dialogueText.GetComponent<Text>();
        int playerNum = Services.VisualNovelScene.currentTurnPlayerNum;
        characterIndex = 0;
        if (playerNum == Services.VisualNovelScene.initiatingPlayer)
        {
            dialogueText = Services.VisualNovelScene.rpsDialogueArray[1];
        }
        else
        {
            dialogueText = Services.VisualNovelScene.rpsDialogueArray[0];
        }

        dialogueText = Services.DialogueDataManager.ParseTextForLineBreaks(dialogueText, textComponent);
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text = "";
    }

    internal override void Update()
    {
        textComponent.text += dialogueText[characterIndex];
        characterIndex += 1;
        if (textComponent.text == dialogueText)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
    }

}
