using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAudio : Task {

    private float timeElapsed;
    private float duration;
    private float targetVolume;
    private AudioSource source;

    public FadeInAudio(AudioSource src, float dur, float targetVol)
    {
        duration = dur;
        source = src;
        targetVolume = targetVol;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        source.volume = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        source.volume = Mathf.Lerp(0, targetVolume, Easing.QuadEaseIn(timeElapsed / duration));

        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

}
