using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToContinueDialogue : WaitForAnyInput {
    private float timeSinceIndicatorChanged;

    protected override void Init()
    {
        base.Init();
        Services.DialogueUIManager.continueIndicator.SetActive(true);
        timeSinceIndicatorChanged = 0;
    }

    internal override void Update()
    {
        GameObject indicator = Services.DialogueUIManager.continueIndicator;
        if (timeSinceIndicatorChanged > Services.DialogueUIManager.indicatorFlashUptime)
        {
            indicator.SetActive(!indicator.activeSelf);
            timeSinceIndicatorChanged = 0;
        }

        timeSinceIndicatorChanged += Time.deltaTime;
    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        Services.DialogueUIManager.continueIndicator.SetActive(false);
    }
}
