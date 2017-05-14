using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Ability {

    private bool shrink;
    public float shrinkTime;
    private float shrinkTimeElapsed;
    private Vector3 baseScale;
    
	// Use this for initialization
	void Start() { 
	}
	
	// Update is called once per frame
	void Update () {
		if (shrink)
        {
            shrinkTimeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(baseScale, Vector3.zero, Easing.QuadEaseOut(shrinkTimeElapsed / shrinkTime));
        }
	}

	public override void Init(GameObject player){
		base.Init (player);
        baseScale = transform.localScale;
        shrinkTimeElapsed = 0;
        shrink = false;
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
        parentPlayer.GetComponent<Player>().isInvulnerable = false;
        base.OnCastFinish ();
	}

    public override void SetActive()
    {
        GetComponent<Collider2D>().enabled = true;
        parentPlayer.GetComponent<Player>().isInvulnerable = true;
    }

	public void ShieldAnimationStarted(){
		Debug.Log ("shield animation started");
	}

    public override void SetInactive()
    {
        GetComponent<Collider2D>().enabled = false;
        parentPlayer.GetComponent<Player>().isInvulnerable = false;
        shrink = true;
    }
}
