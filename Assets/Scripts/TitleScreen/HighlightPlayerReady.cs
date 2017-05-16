using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HighlightPlayerReady : Task
{
    private float duration;
    private float timeElapsed;
    private GameObject playerObj;
    private Vector3 initialScale;

    public HighlightPlayerReady(GameObject player, float dur)
    {
        playerObj = player;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        initialScale = playerObj.transform.localScale;

    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(duration, timeElapsed + Time.deltaTime);
        playerObj.transform.localScale = Vector3.LerpUnclamped(
            initialScale, Services.TitleScreen.playerBounceScale * initialScale, Easing.BackEaseOut(timeElapsed / duration));
        if (timeElapsed == duration) SetStatus(TaskStatus.Success);
    }
}
