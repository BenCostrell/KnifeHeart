using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInWinScreenSprites : Task {

	private float duration;
	private float timeElapsed;
	private Vector3 ponytailTargetPos;
	private Vector3 pigtailsTargetPos;
	private bool spritesOn;

	protected override void Init ()
	{
		duration = Services.WinScreenUIManager.spriteSlideInTime;
		timeElapsed = 0;
		ponytailTargetPos = Services.WinScreenUIManager.ponytail.transform.position;
		pigtailsTargetPos = Services.WinScreenUIManager.pigtails.transform.position;
		spritesOn = false;
	}

	internal override void Update ()
	{
		if (!spritesOn) {
			Services.WinScreenUIManager.ponytail.SetActive (true);
			Services.WinScreenUIManager.pigtails.SetActive (true);
			spritesOn = true;
		}

		timeElapsed += Time.deltaTime;

		Services.WinScreenUIManager.ponytail.transform.position = Vector3.Lerp (ponytailTargetPos + 20f * Vector3.left, ponytailTargetPos, 
			Easing.ExpoEaseOut (timeElapsed / duration));
		Services.WinScreenUIManager.pigtails.transform.position = Vector3.Lerp (pigtailsTargetPos + 20f * Vector3.right, pigtailsTargetPos, 
			Easing.ExpoEaseOut (timeElapsed / duration));

		if (timeElapsed >= duration) {
			SetStatus (TaskStatus.Success);
		}
	}
}
