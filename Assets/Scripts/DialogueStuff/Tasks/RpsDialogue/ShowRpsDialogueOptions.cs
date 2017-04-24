using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRpsDialogueOptions : Task {

    private float timeElapsed;
    private GameObject[] optObjects;
    private GameObject[] arrowObjects;

    protected override void Init()
    {
        timeElapsed = 0;
        optObjects = Services.DialogueUIManager.rpsOptionObjects;
        arrowObjects = Services.DialogueUIManager.rpsArrows;
        for (int i = 0; i < optObjects.Length; i++)
        {
            optObjects[i].SetActive(true);
            optObjects[i].transform.localScale = Vector3.zero;
            arrowObjects[i].SetActive(true);
            arrowObjects[i].transform.localScale = Vector3.zero;
        }
    }

    internal override void Update()
    {
        float duration = Services.DialogueUIManager.optionAppearanceTime;
        float staggerTime = Services.DialogueUIManager.optionAppearanceStaggerTime;
        float totalDuration = duration + (2 * staggerTime);
        for (int i = 0; i < 3; i++)
        {
            float totalStaggerTime = i * staggerTime;
            GameObject obj = optObjects[i];
            GameObject arrowObj = arrowObjects[i];
            if ((timeElapsed <= duration + totalStaggerTime) && (timeElapsed >= totalStaggerTime))
            {
                obj.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one,
                    Easing.BackEaseOut((timeElapsed - totalStaggerTime) / duration));
                arrowObj.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one,
                    Easing.BackEaseOut((timeElapsed - totalStaggerTime) / duration));
            }
        }

        timeElapsed = Mathf.Min(totalDuration, timeElapsed + Time.deltaTime);

        if (timeElapsed == totalDuration)
        {
            SetStatus(TaskStatus.Success);
        }

    }
}
