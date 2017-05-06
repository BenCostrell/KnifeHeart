using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallAnimation : Task {
    private float timeElapsed;
    private float duration;
    private Transform playerTransform;
    private Vector3 initialScale;
    private Vector3 initialDirection;
    private Vector3 initialPos;

    protected override void Init()
    {
        playerTransform = Services.FightScene.fallenPlayer.transform;
        timeElapsed = 0;
        duration = Services.FightScene.fallAnimationTime;
        initialScale = playerTransform.localScale;
        initialDirection = Services.FightScene.fallenPlayer.velocityAtDeath.normalized;
        Debug.Log(initialDirection);
        Debug.Log(Services.FightScene.fallenPlayer.stageEdgeBoundaryCollider.enabled);
        initialPos = playerTransform.position;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        playerTransform.position = Vector3.Lerp(initialPos, initialPos + Services.FightScene.additionalFallDistance * initialDirection,
            Easing.QuadEaseOut(timeElapsed / duration));
        playerTransform.localScale = Vector3.Lerp(initialScale, Vector3.zero, Easing.QuadEaseIn(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        playerTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        playerTransform.localScale = initialScale;
    }

}
