using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sing : Attack {

	public float stunDuration;
	public float lifeDuration;
	private float timeElapsed;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		transform.localScale = Vector3.Lerp (0.5f * Vector3.one, Vector3.one, timeElapsed / lifeDuration);
	}

	public override void Init(GameObject player){
		animTrigger = "Sing";
		cooldown = 3;
		castDuration = 0.2f;
		baseKnockback = 0;
		knockbackGrowth = 0;
		damage = 0;
		isProjectile = true;

		base.Init (player);

		stunDuration = 2;
		lifeDuration = 3;
		timeElapsed = 0;
		Destroy (gameObject, lifeDuration);
	}

	protected override void HitPlayer(GameObject player){
		player.GetComponent<Player> ().Stun (stunDuration);
		Destroy (gameObject);
	}

	public override void OnCastFinish ()
	{
	}
}
