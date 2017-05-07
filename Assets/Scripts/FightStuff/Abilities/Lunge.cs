using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunge : Attack {

	public float speed;
    public float upDownTrailWidth;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void FixedUpdate()
    {
        CheckIfGoingOffStage();
    }

	public override void Init(GameObject player){
		base.Init (player);

        Dash();
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return (playerHit.transform.position - parentPlayer.transform.position).normalized;
	}

    void Dash()
    {
        float angle = parentPlayer.GetComponent<Player>().effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad));
        if (angle == 90 || angle == -90)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            trail.widthMultiplier = upDownTrailWidth;
        }

        parentPlayer.GetComponent<Rigidbody2D>().velocity = speed * direction;
    }

    void CheckIfGoingOffStage()
    {
        Vector3 positionDelta = parentPlayer.GetComponent<Rigidbody2D>().velocity * Time.fixedDeltaTime;
        Bounds previousFeetBounds = parentPlayer.GetComponentInChildren<Feet>().gameObject.GetComponent<BoxCollider2D>().bounds;
        Bounds nextFeetBounds = new Bounds(previousFeetBounds.center + positionDelta, previousFeetBounds.size);

        Collider2D[] lungeBoundaries = Services.FightScene.GetActiveArena().GetComponentsInChildren<BoxCollider2D>();
        bool stop = true;
        foreach (Collider2D col in lungeBoundaries)
        {
            if (col.bounds.Intersects(nextFeetBounds) && (col.gameObject.tag == "Arena" || col.gameObject.tag == "LungeBlinkBoundary"))
            {
                stop = false;
                break;
            }
        }

        if (stop) parentPlayer.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        
    }
}
