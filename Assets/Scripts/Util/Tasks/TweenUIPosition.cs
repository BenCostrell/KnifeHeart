using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenUIPosition : Task {

    private float duration;
    private float timeElapsed;
    private RectTransform rectTransform;
    private Vector2 startPos;
    private Vector2 endPos;
    private Easing.Function easingFunction;

    public TweenUIPosition(GameObject obj, Vector2 start, Vector2 end, float time, Easing.FunctionType easeType)
    {
        rectTransform = obj.GetComponent<RectTransform>();
        startPos = start;
        endPos = end;
        duration = time;
        easingFunction = Easing.GetFunctionWithTypeEnum(easeType);
    }

    protected override void Init()
    {
        timeElapsed = 0;
        rectTransform.gameObject.SetActive(true);
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

        rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, easingFunction(timeElapsed / duration));

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
