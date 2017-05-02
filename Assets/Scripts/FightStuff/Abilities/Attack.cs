using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Ability {

	public float baseKnockback;
	public float knockbackGrowth;
	public int damage;
	public bool isProjectile;
    public AudioClip onImpactAudio;

    protected bool hitPlayer1;
    protected bool hitPlayer2;

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
        PlayImpactSound();
        PlayHitParticleEffect(player);
    }

    void PlayHitParticleEffect(GameObject player)
    {
        Vector3 collisionPoint = player.transform.position + (transform.position - player.transform.position) / 2;
        GameObject hitParticle = Instantiate(Services.PrefabDB.HitParticle,
                collisionPoint, Quaternion.identity, player.transform) as GameObject;
        Destroy(hitParticle, hitParticle.GetComponent<ParticleSystem>().main.duration);
    }

    void PlayImpactSound()
    {
        impactAudioSource.clip = onImpactAudio;
        impactAudioSource.Play();
    }

	protected virtual Vector3 GetDirectionHit (GameObject playerHit){
		return Vector3.zero;
	}

	void OnTriggerStay2D(Collider2D collider){
		GameObject collidedObject = collider.gameObject;
		if (collidedObject.tag == "Player") {
			Player pc = collidedObject.GetComponent<Player> ();
			if (pc.playerNum != parentPlayer.GetComponent<Player>().playerNum){
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

    public override void Init(GameObject player)
    {
        base.Init(player);
        GetComponent<Collider2D>().enabled = false;
    }

    public override void SetActive()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void TurnOffHitbox()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
