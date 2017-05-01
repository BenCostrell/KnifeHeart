using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayTrack : Task
{
    private AudioSource source;
    public PlayTrack(AudioSource src)
    {
        source = src;
    }

    protected override void Init()
    {
        source.Play();
        SetStatus(TaskStatus.Success);
    }
}
