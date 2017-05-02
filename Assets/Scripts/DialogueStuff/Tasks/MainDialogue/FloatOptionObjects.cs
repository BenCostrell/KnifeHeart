using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FloatOptionObjects : Task
{
    private Vector3[] optionBasePositions;

    protected override void Init()
    {
        optionBasePositions = new Vector3[Services.DialogueUIManager.optionObjects.Length];
        for (int i = 0; i < Services.DialogueUIManager.optionObjects.Length; i++)
        {
            optionBasePositions[i] = Services.DialogueUIManager.optionObjects[i].GetComponent<RectTransform>().anchoredPosition3D;
        }

        Services.EventManager.Register<DialoguePicked>(EndFloat);
    }

    internal override void Update()
    {
        for (int i = 0; i < Services.DialogueUIManager.optionObjects.Length; i++)
        {
            Services.DialogueUIManager.optionObjects[i].GetComponent<RectTransform>().anchoredPosition3D =
                optionBasePositions[i] + new Vector3(-Services.DialogueUIManager.optObjNoiseMag / 2 +
                Services.DialogueUIManager.optObjNoiseMag *
                Mathf.PerlinNoise(Services.DialogueUIManager.optObjNoiseSpd * Time.time, i * 1000),
                -Services.DialogueUIManager.optObjNoiseMag / 2 +
                Services.DialogueUIManager.optObjNoiseMag *
                Mathf.PerlinNoise(Services.DialogueUIManager.optObjNoiseSpd * Time.time, i * 10000),
                -Services.DialogueUIManager.optObjNoiseMag / 2 +
                Services.DialogueUIManager.optObjNoiseMag *
                Mathf.PerlinNoise(Services.DialogueUIManager.optObjNoiseSpd * Time.time, i * 100000)
                );
        }
    }

    void EndFloat(DialoguePicked e)
    {
        SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        for (int i = 0; i < Services.DialogueUIManager.optionObjects.Length; i++)
        {
            Services.DialogueUIManager.optionObjects[i].GetComponent<RectTransform>().anchoredPosition3D = optionBasePositions[i];
        }
        Services.EventManager.Unregister<DialoguePicked>(EndFloat);
    }

}
