using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTransitionTask : Task {
    private float timeElapsed;
    private float duration;
    private RectTransform ponytail;
    private RectTransform pigtails;
    private Vector3 scaleTarget;
    private Vector3 ponytailInitialScale;
    private Vector3 pigtailsInitialScale;
    private Vector2 ponytailPosTarget;
    private Vector2 pigtailsPosTarget;
    private Vector2 ponytailInitialPos;
    private Vector2 pigtailsInitialPos;

    protected override void Init ()
	{
        timeElapsed = 0;
        duration = 1f;
        ponytail = Services.DialogueUIManager.ponytail.GetComponent<RectTransform>();
        pigtails = Services.DialogueUIManager.pigtails.GetComponent<RectTransform>();
        ponytailInitialPos = ponytail.anchoredPosition;
        ponytailInitialScale = ponytail.localScale;
        pigtailsInitialPos = pigtails.anchoredPosition;
        pigtailsInitialScale = pigtails.localScale;
        scaleTarget = 1.4f * Vector3.one;
        ponytailPosTarget = new Vector2(-550, -20);
        pigtailsPosTarget = new Vector2(550, -20);
        ponytail.gameObject.GetComponent<Image>().color = Color.white;
        pigtails.gameObject.GetComponent<Image>().color = Color.white;
	}

	internal override void Update ()
	{
        timeElapsed += Time.deltaTime;

        ponytail.localScale = Vector3.Lerp(ponytailInitialScale, scaleTarget, Easing.ExpoEaseOut(timeElapsed / duration));
        ponytail.anchoredPosition = Vector2.Lerp(ponytailInitialPos, ponytailPosTarget, Easing.ExpoEaseOut(timeElapsed / duration));
        pigtails.localScale = Vector3.Lerp(pigtailsInitialScale, scaleTarget, Easing.ExpoEaseOut(timeElapsed / duration));
        pigtails.anchoredPosition = Vector2.Lerp(pigtailsInitialPos, pigtailsPosTarget, Easing.ExpoEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
	}
}
