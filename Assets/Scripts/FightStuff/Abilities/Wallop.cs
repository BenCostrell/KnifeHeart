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
		base.Init (player);
		WallopTask delayHitbox = new WallopTask (player.GetComponent<Player> (), this);
		player.GetComponent<Player>().taskManager.AddTask (delayHitbox);
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return (playerHit.transform.position - parentPlayer.transform.position).normalized;
	}
}
