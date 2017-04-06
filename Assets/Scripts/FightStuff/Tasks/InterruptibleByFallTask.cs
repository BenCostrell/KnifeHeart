using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptibleByFallTask : Task {

    protected override void Init()
    {
        Services.EventManager.Register<PlayerFall>(OnPlayerFall);
    }

    void OnPlayerFall(PlayerFall e)
    {
        SetStatus(TaskStatus.Aborted);
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<PlayerFall>(OnPlayerFall);
    }
}
