using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTransitionMusic : Task {

    private float timeElapsed;
    private float duration;
    private AudioSource source;

    public PlayTransitionMusic(AudioSource src)
    {
        source = src;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        Services.MusicManager.GenerateSourceAndPlay(Services.MusicManager.fightTransition);
        duration = Services.MusicManager.fightTransition.length - Services.MusicManager.jankyHeadstart;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        source.Play();
    }

}
