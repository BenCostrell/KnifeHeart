using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForFall : Task {
    private Player fallenPlayer;

    protected override void Init()
    {
        Services.EventManager.Register<PlayerFall>(OnPlayerFall);
    }

    void OnPlayerFall(PlayerFall e)
    {
        fallenPlayer = e.fallenPlayer;
        SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        Services.FightSceneManager.fallenPlayer = fallenPlayer;
        Services.EventManager.Unregister<PlayerFall>(OnPlayerFall);
    }
}
