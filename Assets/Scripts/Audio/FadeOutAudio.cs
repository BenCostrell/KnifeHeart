using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAudio : Task
{

    private float timeElapsed;
    private float duration;
    private AudioSource source;

    public FadeOutAudio(AudioSource src, float dur)
    {
        duration = dur;
        source = src;
    }

    protected override void Init()
    {
        timeElapsed = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        source.volume = Mathf.Lerp(1, 0, Easing.QuadEaseIn(timeElapsed / duration));

        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        source.Stop();
    }

}
