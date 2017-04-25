using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

public class HitLag : Task
{
    private float timeElapsed;
    private float duration;
    private float knockback;

    public HitLag(float kb)
    {
        knockback = kb;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.FightScene.hitLagRatio * knockback;
        Time.timeScale = 0;
        //Debug.Log(duration);
    }

    internal override void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Time.timeScale = 1;
    }



}
