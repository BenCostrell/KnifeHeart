using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RotateDialogueOptions : Task
{
    private float timeElapsed;
    private float duration;
    private Vector3 initialRotation;
    private Vector3 initialOptionRotation;
    private Transform optionBaseTransform;
    private int direction;
    private Vector3 previousRotation;
    private Vector3 previousOptionRotation;
    private int numAvailableOptions;

    public RotateDialogueOptions(float dur, int dir)
    {
        duration = dur;
        direction = dir;
    }

    protected override void Init()
    {
        optionBaseTransform = Services.DialogueUIManager.optionObjects[0].transform.parent;
        initialRotation = optionBaseTransform.localRotation.eulerAngles;
        previousRotation = initialRotation;
        initialOptionRotation = Services.DialogueUIManager.optionObjects[0].transform.localEulerAngles;
        previousOptionRotation = initialOptionRotation;
        timeElapsed = 0;
        numAvailableOptions = 0;
        foreach(GameObject optionObj in Services.DialogueUIManager.optionObjects)
        {
            if (optionObj.activeSelf) numAvailableOptions += 1;
        }
        duration *= 4 / numAvailableOptions;
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(duration, timeElapsed + Time.deltaTime);

        Vector3 targetRotation = Vector3.LerpUnclamped(initialRotation, initialRotation + (360/numAvailableOptions * direction * Vector3.right),
            Easing.BackEaseOut(timeElapsed / duration));
        optionBaseTransform.Rotate(targetRotation - previousRotation);
        previousRotation = targetRotation;

        List<GameObject> optionObjects = new List<GameObject>();

        Vector3 targetOptionRotation = Vector3.LerpUnclamped(initialOptionRotation,
                initialOptionRotation + (360/numAvailableOptions * -direction * Vector3.right), Easing.BackEaseOut(timeElapsed / duration));
        foreach (GameObject optionObj in Services.DialogueUIManager.optionObjects)
        {
            optionObj.transform.Rotate(targetOptionRotation - previousOptionRotation);
            optionObjects.Add(optionObj);
        }
        previousOptionRotation = targetOptionRotation;


        optionObjects.Sort(delegate (GameObject a, GameObject b)
        {
            if (a.transform.position.z > b.transform.position.z) return 1;
            else if (a.transform.position.z < b.transform.position.z) return -1;
            else return 0;

        });

        for (int i = 0; i < optionObjects.Count; i++)
        {
            optionObjects[i].transform.SetAsFirstSibling();
        }

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }

    }

    protected override void OnSuccess()
    {
    }
}
