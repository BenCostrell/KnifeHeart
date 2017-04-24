using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToContinueFromComic : Task {
    private GameObject comicPage;
    private bool grow;
    private float scaleTime;
    private float growTime;
    private float shrinkTime;
    GameObject continueButton;

    public WaitToContinueFromComic(GameObject page, GameObject contButton, float grwTime, float shrnkTime)
    {
        comicPage = page;
        continueButton = contButton;
        growTime = grwTime;
        shrinkTime = shrnkTime;
    }


    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(Continue);
        continueButton.SetActive(true);
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
            totalScaleTime = growTime;
            startScale = Vector3.one;
            targetScale = 1.2f * Vector3.one;
            easedTime = Easing.QuadEaseOut(scaleTime / totalScaleTime);
        }
        else {
            totalScaleTime = shrinkTime;
            startScale = 1.2f * Vector3.one;
            targetScale = Vector3.one;
            easedTime = Easing.QuadEaseIn(scaleTime / totalScaleTime);
        }
        if (scaleTime < totalScaleTime)
        {
            continueButton.transform.localScale = Vector3.Lerp(startScale, targetScale, easedTime);
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
        bool lastComic;
        Services.EventManager.Unregister<ButtonPressed>(Continue);
        if (Services.FightScene != null)
        {
            lastComic = Services.FightScene.lastComic;
        }
        else
        {
            lastComic = false;
        }
        if (!lastComic)
        {
            comicPage.SetActive(false);
        }
        continueButton.SetActive(false);
    }
}
