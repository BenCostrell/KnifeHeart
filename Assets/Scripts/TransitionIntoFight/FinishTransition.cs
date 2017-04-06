using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTransition : Task {
	private Vector2 ponytailInitialPosition;
	private Vector2 pigtailsInitialPosition;
    private RectTransform ponytailRectTransform;
    private RectTransform pigtailsRectTransform;
	private float timeElapsed;

	protected override void Init ()
	{
        ponytailRectTransform = Services.DialogueUIManager.ponytail.GetComponent<RectTransform>();
        pigtailsRectTransform = Services.DialogueUIManager.pigtails.GetComponent<RectTransform>();

        ponytailInitialPosition = ponytailRectTransform.anchoredPosition;
        pigtailsInitialPosition = pigtailsRectTransform.anchoredPosition;
		Services.TransitionUIManager.transitionUI.SetActive (false);
	}

	internal override void Update ()
	{
		float duration = Services.TransitionUIManager.transitionEndTime;
		ponytailRectTransform.anchoredPosition = Vector2.Lerp (ponytailInitialPosition, ponytailInitialPosition + 500 * Vector2.left, 
			Easing.BackEaseIn (timeElapsed / duration));
		pigtailsRectTransform.anchoredPosition = Vector2.Lerp (pigtailsInitialPosition, pigtailsInitialPosition + 500 * Vector2.right, 
			Easing.BackEaseIn (timeElapsed / duration));
		timeElapsed += Time.deltaTime;

		if (timeElapsed > duration) {
			SetStatus (TaskStatus.Success);
		}
	}
}
