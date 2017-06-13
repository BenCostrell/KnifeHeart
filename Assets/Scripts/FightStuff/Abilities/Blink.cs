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
        PlayTeleportEffect(true);
    }

    void Teleport()
    {
        parentPlayer.isInvulnerable = false;
        parentPlayer.rb.velocity = Vector3.zero;
        Feet feet = parentPlayer.GetComponentInChildren<Feet>();
        float angle = parentPlayer.effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        Vector3 targetPosition;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(feet.transform.position, direction, distance, stageBoundary);
        if (hit)
        {
            float distanceToBlink = Mathf.Max(hit.distance - stopShortDistance, 0f);
            targetPosition = parentPlayer.transform.position + distanceToBlink * direction;
        }
        else
        {
            targetPosition = parentPlayer.transform.position + distance * direction;
        }
        parentPlayer.rb.MovePosition(targetPosition);
        transform.position = targetPosition;
        PlayTeleportEffect(false);
    }

    void PlayTeleportEffect(bool goingOut)
    {
        GameObject effectPrefab;
        if (goingOut) effectPrefab = Services.PrefabDB.TeleportOut;
        else effectPrefab = Services.PrefabDB.TeleportIn;
        GameObject teleportParticle =
            Instantiate(effectPrefab, transform.position, Quaternion.identity) as GameObject;
        ParticleSystem[] particleSystems = teleportParticle.GetComponentsInChildren<ParticleSystem>();
        float longestDuration = 0;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem.MainModule main;
            main = particleSystems[i].main;
            float duration = main.duration + main.startLifetime.constant;
            if (duration > longestDuration) longestDuration = duration;
        }
        Destroy(teleportParticle, longestDuration);
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
