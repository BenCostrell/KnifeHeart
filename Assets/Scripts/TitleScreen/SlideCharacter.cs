using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SlideCharacter : Task
{
    private float timeElapsed;
    private float duration;
    private GameObject character;
    private Vector3 initialPos;
    private Vector3 targetPos;
    private Easing.Function easeType; 

    public SlideCharacter(GameObject chara, Vector3 initial, Vector3 target, float dur, Easing.FunctionType easingType)
    {
        character = chara;
        initialPos = initial;
        targetPos = target;
        duration = dur;
        easeType = Easing.GetFunctionWithTypeEnum(easingType);
    }

    protected override void Init()
    {
        timeElapsed = 0;
        character.transform.position = initialPos;
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(duration, timeElapsed + Time.deltaTime);
        character.transform.position = Vector3.LerpUnclamped(initialPos, targetPos, easeType(timeElapsed / duration));
        if (timeElapsed == duration) SetStatus(TaskStatus.Success);
    }
}
