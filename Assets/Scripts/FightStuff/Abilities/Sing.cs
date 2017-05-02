using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sing : Attack {

	public float stunDuration;
	public float lifeDuration;
	private float timeElapsed;
    private bool activated;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
        if (activated)
        {
            timeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(0.5f * Vector3.one, Vector3.one, timeElapsed / lifeDuration);
        }
	}

	public override void Init(GameObject player){
		base.Init (player);
		timeElapsed = 0;
        GetComponent<SpriteRenderer>().enabled = false;
        activated = false;
    }

    protected override void HitPlayer(GameObject player){
		player.GetComponent<Player> ().Stun (stunDuration);
		Destroy (gameObject);
	}

    public override void SetActive()
    {
        base.SetActive();
        GetComponent<SpriteRenderer>().enabled = true;
        Destroy(gameObject, lifeDuration);
        activated = true;
    }

    public override void OnCastFinish ()
	{
	}
}
