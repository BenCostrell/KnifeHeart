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

        for (int i = 0; i < 2; i++)
        {
            float xPos = Services.TransitionUIManager.blurbXSpacing;
            if (i == 0)
            {
                xPos *= -1;
            }
            float yPos = 0;

            foreach (Dialogue dialogue in Services.TransitionUIManager.dialoguesAccumulated[i])
            {
                GameObject blurb = GameObject.Instantiate(Services.PrefabDB.Blurb, 
                    Services.TransitionUIManager.abilityBlurbs.transform) as GameObject;
                blurb.GetComponentInChildren<Text>().text = dialogue.blurb;
                blurb.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
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

        if (timeElapsed <= scaleInDuration)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (GameObject blurb in blurbAbilityBoxes[i])
                {
                    blurb.transform.localScale = Vector2.Lerp(Vector2.zero, Services.TransitionUIManager.blurbScale * Vector2.one,
                        Easing.QuadEaseOut(timeElapsed / scaleInDuration));
                }
            }
        }
        if (timeElapsed >= scaleInDuration + delayDuration)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < blurbAbilityBoxes[i].Count; j++)
                {
                    GameObject blurb = blurbAbilityBoxes[i][j];
                    if ((timeElapsed - scaleInDuration - delayDuration) > flipDuration / 2)
                    {
                        blurb.GetComponentInChildren<Text>().text =
                            Services.TransitionUIManager.abilityNameDict[Services.TransitionUIManager.dialoguesAccumulated[i][j].abilityGiven];
                    }
                    blurb.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero,
                        Services.TransitionUIManager.numBlurbRotations * 360 * Vector3.right,
                        Easing.QuadEaseOut((timeElapsed - scaleInDuration - delayDuration) / flipDuration)));
                }
            }
        }

        if (timeElapsed >= scaleInDuration + flipDuration + delayDuration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
