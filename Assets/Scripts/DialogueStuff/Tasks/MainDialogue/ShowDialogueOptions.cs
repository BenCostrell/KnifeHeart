using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDialogueOptions : Task {

	private bool firstChoice;
	private float timeElapsed;
	private int numAvailOptions;
    private RectTransform activePlayer;
    private RectTransform inactivePlayer;
    private Vector3 activeScaleTarget;
    private Vector3 inactiveScaleTarget;
    private Vector3 activeInitialScale;
    private Vector3 inactiveInitialScale;
    private Vector2 activePosTarget;
    private Vector2 inactivePosTarget;
    private Vector2 activeInitialPos;
    private Vector2 inactiveInitialPos;
	public ShowDialogueOptions(bool firstChc){
		firstChoice = firstChc;
	}

	protected override void Init ()
	{
        timeElapsed = 0;
        Services.VisualNovelScene.GenerateDialogueOptions (firstChoice);
		GameObject[] optObjects = Services.DialogueUIManager.optionObjects;
        optObjects[0].transform.parent.localRotation = Quaternion.identity;
		numAvailOptions = 0;
		for (int i = 0; i < optObjects.Length; i++) {
			if (optObjects [i].GetComponentInChildren<Text> ().text != "") {
				numAvailOptions += 1;
				optObjects [i].SetActive (true);
				optObjects [i].transform.localScale = Vector3.zero;
			} else {
				optObjects [i].SetActive (false);
			}
		}
        for (int i = 0; i < numAvailOptions; i++)
        {
            optObjects[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0,
                Mathf.Sin(i * 360 / numAvailOptions * Mathf.Deg2Rad),
                -Mathf.Cos(i * 360 / numAvailOptions * Mathf.Deg2Rad))
                * Services.DialogueUIManager.optionWheelRadius;
            optObjects[i].transform.localRotation = Quaternion.identity;
        }

        if (Services.VisualNovelScene.currentTurnPlayerNum == 1)
        {
            activePlayer = Services.DialogueUIManager.ponytail.GetComponent<RectTransform>();
            inactivePlayer = Services.DialogueUIManager.pigtails.GetComponent<RectTransform>();
        }
        else
        {
            activePlayer = Services.DialogueUIManager.pigtails.GetComponent<RectTransform>();
            inactivePlayer = Services.DialogueUIManager.ponytail.GetComponent<RectTransform>();
        }
        activeInitialScale = activePlayer.localScale;
        inactiveInitialScale = inactivePlayer.localScale;
        activeScaleTarget = 1.5f * Vector3.one;
        inactiveScaleTarget = 1.2f * Vector3.one;
        activeInitialPos = activePlayer.anchoredPosition;
        inactiveInitialPos = inactivePlayer.anchoredPosition;
        activePosTarget = new Vector2(activeInitialPos.x, 10);
        inactivePosTarget = new Vector2(inactiveInitialPos.x, -80);
        activePlayer.gameObject.GetComponent<Image>().color = Color.white;
        inactivePlayer.gameObject.GetComponent<Image>().color = Color.gray;
	}

	internal override void Update ()
	{
		float duration = Services.DialogueUIManager.optionAppearanceTime;
		float staggerTime = Services.DialogueUIManager.optionAppearanceStaggerTime;
		float totalDuration = duration + ((numAvailOptions - 1) * staggerTime);
		GameObject[] optObjects = Services.DialogueUIManager.optionObjects;
		for (int i = 0; i < numAvailOptions; i++) {
			float totalStaggerTime = i * staggerTime;
			GameObject obj = optObjects [i];
			if ((timeElapsed <= duration + totalStaggerTime) && (timeElapsed >= totalStaggerTime)) {
				obj.transform.localScale = Vector3.LerpUnclamped (Vector3.zero, Vector3.one, 
					Easing.BackEaseOut ((timeElapsed - totalStaggerTime) / duration));
			}
		}

        activePlayer.localScale = Vector3.Lerp(activeInitialScale, activeScaleTarget, 
            Easing.ExpoEaseOut(timeElapsed / (0.5f * totalDuration)));
        activePlayer.anchoredPosition = Vector2.Lerp(activeInitialPos, activePosTarget, 
            Easing.ExpoEaseOut(timeElapsed / (0.5f * totalDuration)));
        inactivePlayer.localScale = Vector3.Lerp(inactiveInitialScale, inactiveScaleTarget, 
            Easing.ExpoEaseOut(timeElapsed / (0.5f * totalDuration)));
        inactivePlayer.anchoredPosition = Vector2.Lerp(inactiveInitialPos, inactivePosTarget, 
            Easing.ExpoEaseOut(timeElapsed / (0.5f * totalDuration)));

        timeElapsed = Mathf.Min (totalDuration, timeElapsed + Time.deltaTime);

		if (timeElapsed == totalDuration) {
			SetStatus (TaskStatus.Success);
		}

	}
}
