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

	public override void Init(Player player){
		base.Init (player);
	}

	protected override Vector3 GetDirectionHit(Player playerHit){
		return (playerHit.transform.position - parentPlayer.transform.position).normalized;
	}
}
