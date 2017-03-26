using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFromSelectionToDialogue : Task {
    private float timeElapsed;
    private float duration;

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.DialogueUIManager.rpsDialogueDelay;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.DialogueUIManager.rpsTimer.SetActive(false);
        Services.DialogueUIManager.SetRpsOptionUIStatus(false);
        Services.DialogueUIManager.rpsReady_P1.SetActive(false);
        Services.DialogueUIManager.rpsReady_P2.SetActive(false);
    }

}
