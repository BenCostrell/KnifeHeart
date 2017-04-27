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
        GameObject dialogueContainer = Services.DialogueUIManager.dialogueContainer;
        RectTransform containerTransform = dialogueContainer.GetComponent<RectTransform>();
        timeElapsed = 0;
        duration = Services.DialogueUIManager.dialogueTextBoxPopUpTime;
        textBoxTransform = Services.DialogueUIManager.dialogueTextBox.GetComponent<RectTransform>();
        baseSize = textBoxTransform.localScale;
        textBoxTransform.localScale = Vector3.zero;
        dialogueContainer.SetActive(true);
        Services.DialogueUIManager.dialogueText.GetComponent<Text>().text = "";
        if (!crowdDialogue)
        {
            Services.DialogueUIManager.dialogueTextBox.GetComponent<Image>().sprite =
                Services.DialogueUIManager.dialogueTextBoxImages[Services.VisualNovelScene.currentTurnPlayerNum - 1];
            float shift = 0;
            if (!Services.DialogueUIManager.inRpsStage)
            {
                if (Services.DialogueUIManager.queuedDialogue.abilityGiven == Ability.Type.Lunge)
                {
                    shift = 100;
                    if (Services.VisualNovelScene.currentTurnPlayerNum == 2)
                    {
                        shift *= -1;
                    }
                }
            }
            
            containerTransform.anchoredPosition = new Vector2(shift, containerTransform.anchoredPosition.y);

        }
        else {
            Services.DialogueUIManager.dialogueTextBox.GetComponent<Image>().sprite =
                Services.DialogueUIManager.dialogueTextBoxImages[2];
            containerTransform.anchoredPosition = new Vector2(0, containerTransform.anchoredPosition.y);
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
