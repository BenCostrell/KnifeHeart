using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAudio : Task
{

    private float timeElapsed;
    private float duration;
    private float initialVolume;
    private AudioSource source;

    public FadeOutAudio(AudioSource src, float dur)
    {
        duration = dur;
        source = src;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        initialVolume = source.volume;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        source.volume = Mathf.Lerp(initialVolume, 0, Easing.QuadEaseIn(timeElapsed / duration));

        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        source.Stop();
        source.volume = initialVolume;
    }

}
