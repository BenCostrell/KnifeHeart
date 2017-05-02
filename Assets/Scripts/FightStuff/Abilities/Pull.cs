using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Attack {
	public float speed;
	public float distanceToPullTo;
	public float hitstun;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init(GameObject player){
		base.Init (player);
	}

	protected override Vector3 GetDirectionHit(GameObject playerHit){
		return GetComponent<Rigidbody2D> ().velocity.normalized;
	}

	protected override void HitPlayer(GameObject player){
		base.HitPlayer (player);
		Services.EventManager.Fire (new PlayerHooked (player.GetComponent<Player>()));
	}

    public override void SetActive()
    {
        base.SetActive();

        float angle = parentPlayer.GetComponent<Player>().effectiveRotation;
        Vector3 direction = new Vector3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad));

        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localPosition += 0.5f * direction;
        GetComponent<Rigidbody2D>().velocity = speed * direction;

        PullTask pullTask = new PullTask(parentPlayer.GetComponent<Player>(), this);
        parentPlayer.GetComponent<Player>().taskManager.AddTask(pullTask);
    }

    public override void OnCastFinish ()
	{
	}

	public void OnFinish(){
		Destroy (gameObject);
	}
}
