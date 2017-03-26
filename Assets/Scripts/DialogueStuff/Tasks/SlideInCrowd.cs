using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInCrowd : Task {
    private RectTransform crowdLeft;
    private RectTransform crowdRight;
    private Vector2 leftInitialPos;
    private Vector2 rightInitialPos;
    private float duration;
    private float timeElapsed;

    protected override void Init()
    {
        Services.DialogueUIManager.crowdImage.SetActive(true);
        crowdLeft = Services.DialogueUIManager.crowdImage.transform.GetChild(0).GetComponent<RectTransform>();
        crowdRight = Services.DialogueUIManager.crowdImage.transform.GetChild(1).GetComponent<RectTransform>();
        leftInitialPos = crowdLeft.anchoredPosition;
        rightInitialPos = crowdRight.anchoredPosition;
        crowdLeft.anchoredPosition += 800 * Vector2.left;
        crowdRight.anchoredPosition += 800 * Vector2.right; 
        timeElapsed = 0;
        duration = Services.DialogueUIManager.crowdSlideTime;
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(duration, timeElapsed + Time.deltaTime);

        crowdLeft.anchoredPosition = Vector2.LerpUnclamped(leftInitialPos + 800 * Vector2.left, leftInitialPos,
            Easing.BackEaseIn(timeElapsed / duration));
        crowdRight.anchoredPosition = Vector2.LerpUnclamped(rightInitialPos + 800 * Vector2.right, rightInitialPos,
            Easing.BackEaseIn(timeElapsed / duration));

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
    }

}
