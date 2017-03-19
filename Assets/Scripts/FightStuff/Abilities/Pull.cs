using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Attack {
	public float speed;
	public float distanceToPullTo;
	public float hitstun;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init(GameObject player){
		animTrigger = "Pull";
		cooldown = 3f;
		castDuration = 0.5f;
		baseKnockback = 0;
		knockbackGrowth = 0f;
		damage = 2;
		speed = 20;
		distanceToPullTo = 2f;
		hitstun = 0.2f;
		isProjectile = true;
		isMelee = false;
		onCastAudio = Resources.Load ("Sounds/Abilities/Fireball") as AudioClip;

		base.Init (player);

		float angle = player.GetComponent<Player>().effectiveRotation;
		Vector3 direction = new Vector3 (-Mathf.Cos(angle*Mathf.Deg2Rad), -Mathf.Sin(angle*Mathf.Deg2Rad));

		transform.rotation = Quaternion.Euler (0, 0, angle);
		transform.localPosition += 0.5f * direction;
		GetComponent<Rigidbody2D> ().velocity = speed * direction;

		PullTask pullTask = new PullTask (player.GetComponent<Player> (), this);
		Services.TaskManager.AddTask (pullTask);
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return GetComponent<Rigidbody2D> ().velocity.normalized;
	}

	protected override void HitPlayer(GameObject player){
		base.HitPlayer (player);
		Services.EventManager.Fire (new PlayerHooked (player.GetComponent<Player>()));
	}

	public override void OnCastFinish ()
	{
	}

	public void OnFinish(){
		Destroy (gameObject);
	}
}
