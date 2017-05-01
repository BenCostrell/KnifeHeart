using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Ability {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init(GameObject player){
		base.Init (player);

        GetComponent<Collider2D>().enabled = false;
	}

	void OnTriggerEnter2D(Collider2D collider){
		GameObject collidedObject = collider.gameObject;
		if (collidedObject.tag == "Attack") {
			Attack attack = collidedObject.GetComponent<Attack> ();
			if (attack.parentPlayer.GetComponent<Player>().playerNum != parentPlayer.GetComponent<Player>().playerNum){
				attack.parentPlayer = parentPlayer;
				if (attack.isProjectile) {
					collidedObject.GetComponent<Rigidbody2D> ().velocity *= -1;
					collidedObject.transform.localScale = new Vector3 (collidedObject.transform.localScale.x * -1, collidedObject.transform.localScale.y,
						collidedObject.transform.localScale.z);
				}

			}
		}
	}

	public override void OnCastFinish ()
	{
		base.OnCastFinish ();
		parentPlayer.GetComponent<Player> ().isInvulnerable = false;
	}

    public override void SetActive()
    {
        GetComponent<Collider2D>().enabled = true;
        parentPlayer.GetComponent<Player>().isInvulnerable = true;
    }
}
