using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sing : Attack {

	public float stunDuration;
	public float lifeDuration;
	private float timeElapsed;
    private bool activated;
    public float maxSize;
    private ParticleSystem ps;
    public float particleFadeTimeAfterHit;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
        if (activated)
        {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(0.5f * Vector3.one, maxSize * Vector3.one, timeElapsed / lifeDuration);
        }
	}

	public override void Init(Player player){
		base.Init (player);
		timeElapsed = 0;
        GetComponent<SpriteRenderer>().enabled = false;
        activated = false;
        ps = GetComponentInChildren<ParticleSystem>();
    }

    protected override void HitPlayer(Player player){
		player.Stun (stunDuration);
        TurnOffHitbox();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticles = ps.GetParticles(particles);
        
        for (int i = 0; i < numParticles; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            if (particle.remainingLifetime > particleFadeTimeAfterHit) particle.remainingLifetime = particleFadeTimeAfterHit;
            particles[i] = particle;
        }
        ps.SetParticles(particles, numParticles);
	}

    public override void SetActive()
    {
        base.SetActive();
		transform.position = parentPlayer.transform.position;
        GetComponent<SpriteRenderer>().enabled = true;
        Destroy(gameObject, lifeDuration);
        activated = true;
        ParticleSystem.MainModule main = ps.main;
        main.startLifetime = lifeDuration;

        ps.Play();
    }

    public override void OnCastFinish ()
	{
	}
}
