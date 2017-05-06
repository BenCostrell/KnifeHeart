using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Blink : Ability
{
    public float distance;

    public override void Init(GameObject player)
    {
        base.Init(player);

    }

    void Teleport()
    {
        parentPlayer.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        float angle = parentPlayer.GetComponent<Player>().effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad));
        Collider2D[] blinkBoundaries = Services.FightScene.GetActiveArena().GetComponentsInChildren<BoxCollider2D>();
        List<Collider2D> walls = new List<Collider2D>();
        foreach(Collider2D col in blinkBoundaries) if (col.gameObject.tag == "Wall") walls.Add(col);
        Vector3 initialPosition = parentPlayer.transform.position;
        for (int i = 0; i < 11; i++)
        {
            bool inRange = false;
            parentPlayer.transform.position = initialPosition + (distance * (1 - i/10f) * direction);
            Bounds feetBounds = parentPlayer.GetComponentInChildren<Feet>().gameObject.GetComponent<BoxCollider2D>().bounds;
            foreach (Collider2D col in blinkBoundaries) {
                if (feetBounds.Intersects(col.bounds) && (col.gameObject.tag == "Arena" || col.gameObject.tag == "LungeBlinkBoundary"))
                {
                    inRange = true;
                    foreach (Collider2D wall in walls) if (feetBounds.Intersects(wall.bounds)) inRange = false;
                }
            }
            Debug.Log(i);
            Debug.Log(direction);
            if (inRange) break;
        }
    }

    public override void SetActive()
    {
        Teleport();
    }
}
