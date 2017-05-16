using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScaleTitle : Task
{
    private float timeElapsed;
    private float duration;
    private GameObject title;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private Easing.Function easeType;

    public ScaleTitle(GameObject titleObj, Vector3 initial, Vector3 target, float dur, Easing.FunctionType easingType)
    {
        title = titleObj;
        initialScale = initial;
        targetScale = target;
        duration = dur;
        easeType = Easing.GetFunctionWithTypeEnum(easingType);
    }

    protected override void Init()
    {
        title.transform.localScale = initialScale;
        title.SetActive(true);
        timeElapsed = 0;
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(duration, timeElapsed + Time.deltaTime);
        title.transform.localScale = Vector3.LerpUnclamped(initialScale, targetScale, easeType(timeElapsed / duration));
        if (timeElapsed == duration) SetStatus(TaskStatus.Success);
    }
}
