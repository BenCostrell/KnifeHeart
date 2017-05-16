using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOutCrowd : Task {

	private RectTransform crowdLeft;
	private RectTransform crowdRight;
	private Vector2 leftInitialPos;
	private Vector2 rightInitialPos;
	private float duration;
	private float timeElapsed;

	protected override void Init ()
	{
		crowdLeft = Services.DialogueUIManager.crowdImage.transform.GetChild (0).GetComponent<RectTransform> ();
		crowdRight = Services.DialogueUIManager.crowdImage.transform.GetChild (1).GetComponent<RectTransform> ();
		leftInitialPos = crowdLeft.anchoredPosition;
		rightInitialPos = crowdRight.anchoredPosition;
		timeElapsed = 0;
		duration = Services.DialogueUIManager.crowdSlideTime;
	}

	internal override void Update ()
	{
        timeElapsed = Mathf.Min(duration, timeElapsed + Time.deltaTime);

        crowdLeft.anchoredPosition = Vector2.LerpUnclamped (leftInitialPos, leftInitialPos + 800 * Vector2.left, 
			Easing.BackEaseIn(timeElapsed / duration));
		crowdRight.anchoredPosition = Vector2.LerpUnclamped (rightInitialPos, rightInitialPos + 800 * Vector2.right, 
			Easing.BackEaseIn(timeElapsed / duration));

		if (timeElapsed == duration) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess(){
		Services.DialogueUIManager.crowdImage.SetActive (false);
        crowdLeft.anchoredPosition = leftInitialPos;
        crowdRight.anchoredPosition = rightInitialPos;
    }
}
