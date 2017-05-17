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

    public override void Init(GameObject player)
    {
        base.Init(player);
        parentPlayer.GetComponent<Player>().isInvulnerable = true;
    }

    void Teleport()
    {
        parentPlayer.GetComponent<Player>().isInvulnerable = false;
        parentPlayer.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        Feet feet = parentPlayer.GetComponentInChildren<Feet>();
        float angle = parentPlayer.GetComponent<Player>().effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(feet.transform.position, direction, distance, stageBoundary);
        Debug.DrawRay(feet.transform.position, distance * direction, Color.red, 5f);
        if (hit)
        {
            float distanceToBlink = Mathf.Max(hit.distance - stopShortDistance, 0f);
            Vector3 targetPosition = parentPlayer.transform.position + distanceToBlink * direction;
            Debug.Log("blinking from " + parentPlayer.transform.position + " to " + targetPosition);
            parentPlayer.GetComponent<Player>().rb.MovePosition(targetPosition);
        }
        else
        {
            Debug.Log("blinking max distance");
            parentPlayer.GetComponent<Player>().rb.MovePosition(parentPlayer.transform.position + distance * direction);
        }
        //Collider2D[] blinkBoundaries = Services.FightScene.GetActiveArena().GetComponentsInChildren<BoxCollider2D>();
        //List<Collider2D> walls = new List<Collider2D>();
        //foreach (Collider2D col in blinkBoundaries) if (col.gameObject.tag == "Wall") walls.Add(col);
        //Vector3 initialPosition = parentPlayer.transform.position;
        //bool inRange = false;
        //for (int i = 0; i < 11; i++)
        //{
        //    inRange = false;
        //    parentPlayer.transform.position = initialPosition + (distance * (1 - i / 10f) * direction);
        //    Bounds feetBounds = parentPlayer.GetComponentInChildren<Feet>().gameObject.GetComponent<BoxCollider2D>().bounds;
        //    foreach (Collider2D col in blinkBoundaries) {
        //        if (feetBounds.Intersects(col.bounds) &&
        //            (col.gameObject.tag == "Arena" || col.gameObject.tag == "LungeBlinkBoundary"))
        //        {
        //            inRange = true;
        //            foreach (Collider2D wall in walls) if (feetBounds.Intersects(wall.bounds)) inRange = false;
        //        }
        //    }
        //    if (inRange)
        //    {
        //        Debug.Log("stopped at " + i);
        //        break;
        //    }
        //}
    }

    public override void SetActive()
    {
        Teleport();
    }

    public override void OnCastFinish()
    {
        parentPlayer.GetComponent<Player>().isInvulnerable = false;
        base.OnCastFinish();
    }
}
