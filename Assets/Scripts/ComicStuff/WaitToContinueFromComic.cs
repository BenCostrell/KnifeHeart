using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToContinueFromComic : Task {
    private GameObject comicPage;
    private bool grow;
    private float scaleTime;

    public WaitToContinueFromComic(GameObject page)
    {
        comicPage = page;
    }


    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(Continue);
        Services.ComicPanelManager.continueButton.SetActive(true);
        grow = true;
        scaleTime = 0;
    }

    internal override void Update()
    {
        float totalScaleTime;
        Vector3 startScale;
        Vector3 targetScale;
        float easedTime;
        if (grow)
        {
            totalScaleTime = Services.TransitionUIManager.readyPromptGrowTime;
            startScale = Vector3.one;
            targetScale = 1.2f * Vector3.one;
            easedTime = Easing.ExpoEaseOut(scaleTime / totalScaleTime);
        }
        else {
            totalScaleTime = Services.TransitionUIManager.readyPromptShrinkTime;
            startScale = 1.2f * Vector3.one;
            targetScale = Vector3.one;
            easedTime = Easing.QuadEaseOut(scaleTime / totalScaleTime);
        }
        if (scaleTime < totalScaleTime)
        {
            Services.ComicPanelManager.continueButton.transform.localScale = Vector3.Lerp(startScale, targetScale, easedTime);
        }
        else {
            grow = !grow;
            scaleTime = 0;
        }

        scaleTime += Time.deltaTime;
    }

    void Continue (ButtonPressed e)
    {
        if (e.buttonTitle == "A")
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.EventManager.Unregister<ButtonPressed>(Continue);
        comicPage.SetActive(false);
        Services.ComicPanelManager.continueButton.SetActive(false);
    }
}
