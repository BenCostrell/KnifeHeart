using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallop : Attack {

	public float delay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init(GameObject player){
		animTrigger = "Wallop";
		cooldown = 2f;
		castDuration = 1f;
		baseKnockback = 30;
		knockbackGrowth = 5;
		damage = 5;
		isProjectile = false;
		isMelee = true;
		onCastAudio = Resources.Load ("Sounds/Abilities/Sword") as AudioClip;
		delay = 0.5f;

		base.Init (player);
		WallopTask delayHitbox = new WallopTask (player.GetComponent<Player> (), this);
		Services.TaskManager.AddTask (delayHitbox);
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return (playerHit.transform.position - parentPlayer.transform.position).normalized;
	}
}
