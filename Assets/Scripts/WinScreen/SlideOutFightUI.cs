using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOutFightUI : Task {
	private float duration;
	private float timeElapsed;
	private Vector3 uiInitialPos_P1;
	private Vector3 uiInitialPos_P2;

	protected override void Init ()
	{
		duration = Services.WinScreenUIManager.UISlideOffTime;
		timeElapsed = 0;
		uiInitialPos_P1 = Services.FightUIManager.UI_P1.transform.position;
		uiInitialPos_P2 = Services.FightUIManager.UI_P2.transform.position;
	}

	internal override void Update ()
	{
		timeElapsed = Mathf.Min (duration, timeElapsed + Time.deltaTime);

		Services.FightUIManager.UI_P1.transform.position = Vector3.LerpUnclamped (uiInitialPos_P1, uiInitialPos_P1 + 3.5f * Vector3.left, 
			Easing.BackEaseIn (timeElapsed / duration));
		Services.FightUIManager.UI_P2.transform.position = Vector3.LerpUnclamped (uiInitialPos_P2, uiInitialPos_P2 + 3.5f * Vector3.right, 
			Easing.BackEaseIn (timeElapsed / duration));

		if (timeElapsed >= duration) {
			SetStatus (TaskStatus.Success);
		}
	}

}
