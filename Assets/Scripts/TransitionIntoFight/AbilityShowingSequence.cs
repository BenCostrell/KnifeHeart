using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AbilityShowingSequence : Task
{
    private float timeElapsed;
    private float scaleInDuration;
    private float flipDuration;
    private float delayDuration;
    private List<GameObject>[] blurbAbilityBoxes;
    private List<Vector2>[] targetPositions;
    private List<Vector2>[] initialPositions;
    
    public AbilityShowingSequence(float scaleInDur, float flipDur, float delayDur)
    {
        scaleInDuration = scaleInDur;
        flipDuration = flipDur;
        delayDuration = delayDur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        blurbAbilityBoxes = new List<GameObject>[2]
        {
            new List<GameObject>(),
            new List<GameObject>()
        };
        targetPositions = new List<Vector2>[2]
        {
            new List<Vector2>(),
            new List<Vector2>()
        };
        initialPositions = new List<Vector2>[2]
        {
            new List<Vector2>(),
            new List<Vector2>()
        };

        for (int i = 0; i < 2; i++)
        {
            float shiftDirection = 1;
            if (i == 0)
            {
                shiftDirection *= -1;
            }
            float yPos = 0;
            float xStagger = 0;
            for (int j = 0; j < Services.TransitionUIManager.dialoguesAccumulated[i].Count; j++)
            {
                xStagger = (j % 2) * Services.TransitionUIManager.blurbXStagger;
                Dialogue dialogue = Services.TransitionUIManager.dialoguesAccumulated[i][j];
                GameObject blurb = GameObject.Instantiate(Services.PrefabDB.Blurb, 
                    Services.TransitionUIManager.abilityBlurbs.transform) as GameObject;
                blurb.GetComponentInChildren<Image>().sprite = Services.TransitionUIManager.blurbBoxes[j];
                blurb.GetComponentInChildren<Text>().text = dialogue.blurb;
                Vector2 targetPos = new Vector2((Services.TransitionUIManager.blurbXSpacing + xStagger) * shiftDirection, yPos);
                Vector2 initialPos = targetPos + (Services.TransitionUIManager.blurbInitialOffset * shiftDirection * Vector2.right);
                initialPositions[i].Add(initialPos);
                targetPositions[i].Add(targetPos);
                blurb.GetComponent<RectTransform>().anchoredPosition = initialPos;
                yPos -= Services.TransitionUIManager.blurbYSpacing;
                blurb.transform.localScale = Vector2.zero;
                blurbAbilityBoxes[i].Add(blurb);
            }
        }

        Services.TransitionUIManager.blurbAbilityBoxes = blurbAbilityBoxes;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        float staggerTime = 0;

        for (int i = 0; i < 2; i++)
        {
            staggerTime = 0;
            for (int j = 0; j < blurbAbilityBoxes[i].Count; j++)
            {
                if (timeElapsed >= staggerTime && timeElapsed <= (scaleInDuration + staggerTime))
                {
                    RectTransform blurbTransform = blurbAbilityBoxes[i][j].GetComponent<RectTransform>();
                    blurbTransform.localScale = Vector2.Lerp(Vector2.zero, Services.TransitionUIManager.blurbScale * Vector2.one,
                        Easing.QuadEaseOut((timeElapsed - staggerTime) / scaleInDuration));
                    blurbTransform.anchoredPosition = Vector2.Lerp(initialPositions[i][j], targetPositions[i][j],
                        Easing.QuadEaseOut((timeElapsed - staggerTime) / scaleInDuration));
                }
                staggerTime += Services.TransitionUIManager.blurbStaggerTime;
            }
        }


        for (int i = 0; i < 2; i++)
        {
            staggerTime = 0;
            for (int j = 0; j < blurbAbilityBoxes[i].Count; j++)
            {
                if (timeElapsed >= (scaleInDuration + delayDuration + staggerTime) 
                    && timeElapsed <= (scaleInDuration+delayDuration+staggerTime+flipDuration))
                {
                    GameObject blurb = blurbAbilityBoxes[i][j];
                    if ((timeElapsed - scaleInDuration - delayDuration - staggerTime) > flipDuration / 2)
                    {
                        blurb.GetComponentInChildren<Text>().text =
                            Services.TransitionUIManager.abilityNameDict[Services.TransitionUIManager.dialoguesAccumulated[i][j].abilityGiven];
                    }
                    blurb.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero,
                        Services.TransitionUIManager.numBlurbRotations * 360 * Vector3.right,
                        Easing.QuadEaseOut((timeElapsed - scaleInDuration - delayDuration - staggerTime) / flipDuration)));
                }
                staggerTime += Services.TransitionUIManager.blurbStaggerTime;
            }
        }
        

        if (timeElapsed >= (scaleInDuration + flipDuration + delayDuration + staggerTime))
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
