using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToContinueDialogue : WaitForAnyInput {
    private float timeSinceIndicatorChanged;
    private Vector3 baseSize;
    private GameObject indicator;
    private float duration;

    protected override void Init()
    {
        base.Init();
        indicator = Services.DialogueUIManager.continueIndicator;
        indicator.SetActive(true);
        timeSinceIndicatorChanged = 0;
        baseSize = indicator.transform.localScale;
        duration = Services.DialogueUIManager.indicatorFlashUptime;
    }

    internal override void Update()
    {
        timeSinceIndicatorChanged += Time.deltaTime;

        if (timeSinceIndicatorChanged < duration/2)
        {
            indicator.transform.localScale = Vector3.Lerp(baseSize, 1.5f * baseSize, Easing.QuadEaseOut(timeSinceIndicatorChanged / duration));
        }
        else if (timeSinceIndicatorChanged < duration)
        {
            indicator.transform.localScale = Vector3.Lerp(1.5f * baseSize, baseSize, Easing.QuadEaseIn(timeSinceIndicatorChanged / duration));
        }
        else
        {
            timeSinceIndicatorChanged = 0;
        }

    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        indicator.transform.localScale = baseSize;
        indicator.SetActive(false);
    }
}
