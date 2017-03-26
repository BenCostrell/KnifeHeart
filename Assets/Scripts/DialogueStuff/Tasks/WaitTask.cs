using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitTask : Task {

    private float timeElapsed;
    private float duration;

    public WaitTask(float dur)
    {
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
