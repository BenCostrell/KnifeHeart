using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPlayersTask : Task {
    private int roundNum;
    public PositionPlayersTask(int round)
    {
        roundNum = round;
    }

    protected override void Init()
    {
        Services.FightScene.PositionPlayers(roundNum);
        SetStatus(TaskStatus.Success);
    }

}
