using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Ability {

	protected float baseKnockback;
	protected float knockbackGrowth;
	protected int damage;
	public bool hitPlayer1;
	public bool hitPlayer2;
	public bool isProjectile;

	// Use this for initialization
	void Start () {
		hitPlayer1 = false;
		hitPlayer2 = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

	protected virtual void HitPlayer(GameObject player){
		player.GetComponent<Player> ().TakeHit (damage, baseKnockback, knockbackGrowth, GetDirectionHit(player));

	}

	protected virtual Vector3 GetDirectionHit (GameObject playerHit){
		return Vector3.zero;
	}

	void OnTriggerStay2D(Collider2D collider){
		GameObject collidedObject = collider.gameObject;
		if (collidedObject.tag == "Player") {
			Player pc = collidedObject.GetComponent<Player> ();
			if (pc.playerNum != parentPlayer.GetComponent<Player>().playerNum){
				Debug.Log (pc.playerNum);
				if ((pc.playerNum == 1) && !hitPlayer1) {
					hitPlayer1 = true;
					if (!pc.isInvulnerable){
						HitPlayer (collidedObject);
					}
				}
				else if ((pc.playerNum == 2) && !hitPlayer2) {
					hitPlayer2 = true;
					if (!pc.isInvulnerable){
						HitPlayer (collidedObject);
					}
				}

			}
		}
	}
}
