using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAudio : Task {

    private float timeElapsed;
    private float duration;
    private AudioSource source;

    public FadeInAudio(AudioSource src, float dur)
    {
        duration = dur;
        source = src;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        source.volume = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        source.volume = Mathf.Lerp(0, 1, Easing.QuadEaseIn(timeElapsed / duration));

        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

}
