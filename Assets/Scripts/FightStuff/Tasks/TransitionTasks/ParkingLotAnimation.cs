using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParkingLotAnimation : Task
{
    private float timeElapsed;
    private int nextIndex;
    private SpriteRenderer parkingLotSr;

    protected override void Init()
    {
        timeElapsed = 0;
        nextIndex = 1;
        parkingLotSr = Services.FightScene.GetActiveArena().GetComponent<SpriteRenderer>();
        parkingLotSr.sprite = Services.FightUIManager.parkingLotSprites[0];
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= Services.FightUIManager.parkingLotAnimationStaggerTimes[nextIndex-1])
        {
            if (nextIndex < Services.FightUIManager.parkingLotSprites.Length)
            {
                parkingLotSr.sprite = Services.FightUIManager.parkingLotSprites[nextIndex];
                Services.TaskManager.AddTask(new ScreenShake(
                    Services.FightUIManager.parkingLotAnimationScreenshakeSpeeds[nextIndex - 1],
                    Services.FightUIManager.parkingLotAnimationScreenshakeMagnitudes[nextIndex - 1],
                    Services.FightUIManager.parkingLotAnimationScreenshakeDurations[nextIndex - 1]));
                nextIndex += 1;
                timeElapsed = 0;
            }
            else
            {
                SetStatus(TaskStatus.Success);
            }
        }
    }
}

