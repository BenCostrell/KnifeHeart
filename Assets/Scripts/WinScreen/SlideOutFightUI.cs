using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOutFightUI : Task {
	private float duration;
	private float timeElapsed;
	private Vector2 uiInitialPos_P1;
	private Vector2 uiInitialPos_P2;
	private RectTransform uiTransformP1;
	private RectTransform uiTransformP2;

	protected override void Init ()
	{
		duration = Services.WinScreenUIManager.UISlideOffTime;
		timeElapsed = 0;
		uiTransformP1 = Services.FightUIManager.UI_P1.GetComponent<RectTransform> ();
		uiTransformP2 = Services.FightUIManager.UI_P2.GetComponent<RectTransform> ();
		uiInitialPos_P1 = uiTransformP1.anchoredPosition;
		uiInitialPos_P2 = uiTransformP2.anchoredPosition;
	}

	internal override void Update ()
	{
		timeElapsed = Mathf.Min (duration, timeElapsed + Time.deltaTime);

		uiTransformP1.anchoredPosition = Vector2.LerpUnclamped (uiInitialPos_P1, uiInitialPos_P1 + 640 * Vector2.left, 
			Easing.BackEaseIn (timeElapsed / duration));
		uiTransformP2.anchoredPosition = Vector3.LerpUnclamped (uiInitialPos_P2, uiInitialPos_P2 + 640 * Vector2.right, 
			Easing.BackEaseIn (timeElapsed / duration));

		if (timeElapsed >= duration) {
			SetStatus (TaskStatus.Success);
		}
	}

}
