using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDialogueBox : Task
{
    private float timeElapsed;
    private float duration;
    private Vector3 baseSize;
    private RectTransform textBoxTransform;
    private bool crowdDialogue;

    public PopUpDialogueBox(bool crowd)
    {
        crowdDialogue = crowd;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.DialogueUIManager.dialogueTextBoxPopUpTime;
        textBoxTransform = Services.DialogueUIManager.dialogueTextBox.GetComponent<RectTransform>();
        baseSize = textBoxTransform.localScale;
        textBoxTransform.localScale = Vector3.zero;
        Services.DialogueUIManager.dialogueContainer.SetActive(true);
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text = "";
        if (!crowdDialogue)
        {
            Services.DialogueUIManager.dialogueTextBox.GetComponent<Image>().sprite =
                Services.DialogueUIManager.dialogueTextBoxImages[Services.VisualNovelScene.currentTurnPlayerNum - 1];

        }
        else {
            Services.DialogueUIManager.dialogueTextBox.GetComponent<Image>().sprite =
                Services.DialogueUIManager.dialogueTextBoxImages[2];
        }
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        textBoxTransform.localScale = Vector3.Lerp(Vector3.zero, baseSize, Easing.QuadEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

}
