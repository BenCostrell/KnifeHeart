using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOutTransitionUI : Task {
	private float timeElapsed;
    private float duration;
	private List<GameObject> objectsToScale;

	protected override void Init ()
	{
		timeElapsed = 0;
        duration = Services.TransitionUIManager.uiScaleOutTime;

        //objectsToScale = new List<GameObject> ();
		//objectsToScale.Add (Services.TransitionUIManager.transitionUI);
		//objectsToScale.Add (Services.TransitionUIManager.ready_P1);
		//objectsToScale.Add (Services.TransitionUIManager.ready_P2);
	}

	internal override void Update ()
	{
        timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

        /*foreach (GameObject obj in objectsToScale) {
			obj.transform.localScale = Vector3.Lerp (Vector3.one, Vector3.zero, 
				Easing.QuadEaseIn (timeElapsed / duration));
		}*/
        for (int i = 0; i < 2; i++)
        {
            foreach (GameObject blurb in Services.TransitionUIManager.blurbAbilityBoxes[i])
            {
                blurb.GetComponent<RectTransform>().localScale = Vector3.Lerp(Services.TransitionUIManager.blurbScale * Vector3.one, 
                    Vector3.zero,
                    Easing.QuadEaseIn(timeElapsed / duration));
            }

        }

		if (timeElapsed == duration) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		Services.TransitionUIManager.transitionUI.SetActive (false);
        for (int i = 0; i < 2; i++)
        {
            foreach (GameObject blurb in Services.TransitionUIManager.blurbAbilityBoxes[i])
            {
                GameObject.Destroy(blurb);
            }

        }
    }

}
