using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishIntroSequence : Task {

    protected override void Init()
    {
        Services.DialogueUIManager.introSequence.SetActive(false);
        SetStatus(TaskStatus.Success);
    }
}
