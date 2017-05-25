using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Blink : Ability
{
    public float distance;
    public float stopShortDistance;
    public LayerMask stageBoundary;

    public override void Init(Player player)
    {
        base.Init(player);
        parentPlayer.isInvulnerable = true;
    }

    void Teleport()
    {
        parentPlayer.isInvulnerable = false;
        parentPlayer.rb.velocity = Vector3.zero;
        Feet feet = parentPlayer.GetComponentInChildren<Feet>();
        float angle = parentPlayer.effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(feet.transform.position, direction, distance, stageBoundary);
        if (hit)
        {
            float distanceToBlink = Mathf.Max(hit.distance - stopShortDistance, 0f);
            Vector3 targetPosition = parentPlayer.transform.position + distanceToBlink * direction;
            parentPlayer.rb.MovePosition(targetPosition);
        }
        else
        {
            parentPlayer.rb.MovePosition(parentPlayer.transform.position + distance * direction);
        }
    }

    public override void SetActive()
    {
        Teleport();
    }

    public override void OnCastFinish()
    {
        parentPlayer.isInvulnerable = false;
        base.OnCastFinish();
    }
}
