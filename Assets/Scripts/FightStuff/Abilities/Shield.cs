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
            if (shrinkTimeElapsed >= shrinkTime) shrink = false;
        }
	}

	public override void Init(Player player){
		base.Init (player);
        baseScale = transform.localScale;
        shrinkTimeElapsed = 0;
        shrink = false;
        GetComponent<Collider2D>().enabled = false;
	}

	void OnTriggerEnter2D(Collider2D collider){
		GameObject collidedObject = collider.gameObject;
		if (collidedObject.tag == "Attack") {
            Attack attack = collidedObject.GetComponent<Attack>();
            if (attack.parentPlayer.playerNum != parentPlayer.playerNum) DeflectAttack(collidedObject.GetComponent<Attack>());
		}
	}

    void DeflectAttack(Attack attack)
    {
        attack.parentPlayer = parentPlayer;
        if (attack.isProjectile)
        {
            attack.gameObject.GetComponent<Rigidbody2D>().velocity *= -1;
            attack.transform.localScale = new Vector3(
                attack.transform.localScale.x * -1,
                attack.transform.localScale.y,
                attack.transform.localScale.z);
        }
        PlayDeflectionEffect();
    }

    void PlayDeflectionEffect()
    {
        GameObject deflectionParticle = 
            Instantiate(Services.PrefabDB.Deflection, transform.position, Quaternion.identity) as GameObject;
        ParticleSystem[] particleSystems = deflectionParticle.GetComponentsInChildren<ParticleSystem>();
        float longestDuration = 0;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem.MainModule main;
            main = particleSystems[i].main;
            float duration = main.duration + main.startLifetime.constant;
            if (duration > longestDuration) longestDuration = duration;
        }
        Destroy(deflectionParticle, longestDuration);

    }

    public override void OnCastFinish ()
	{
        parentPlayer.isInvulnerable = false;
        base.OnCastFinish ();
	}

    public override void SetActive()
    {
        GetComponent<Collider2D>().enabled = true;
        parentPlayer.isInvulnerable = true;
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
