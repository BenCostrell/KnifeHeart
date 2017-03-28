using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Attack {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public override void Init(GameObject player){
		animTrigger = "basicAttack";
		cooldown = 0.6f;
		castDuration = 0.2f;
		baseKnockback = 16;
		knockbackGrowth = 2;
		damage = 2;
		isProjectile = false;
		isMelee = true;
		onCastAudio = Resources.Load ("Sounds/Abilities/Sword") as AudioClip;

		base.Init (player);
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return (playerHit.transform.position - parentPlayer.transform.position).normalized;
	}
}
