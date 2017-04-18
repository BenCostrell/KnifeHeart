using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunge : Attack {

	private float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init(GameObject player){
		animTrigger = "Lunge";
		cooldown = 1f;
		castDuration = 0.4f;
		baseKnockback = 16;
		knockbackGrowth = 2;
		damage = 2;
		speed = 40;
		isProjectile = false;
		isMelee = true;
		onCastAudio = Resources.Load ("Sounds/Abilities/Lunge") as AudioClip;

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

        parentPlayer.GetComponent<Rigidbody2D>().velocity = speed * direction;
    }
}
