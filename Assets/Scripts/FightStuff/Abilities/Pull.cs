using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : Attack {
	public float speed;
	public float distanceToPullTo;
	public float hitstun;
    public float positionOffset;
    private TaskManager pullTaskManager;
    private bool pullFired;

	// Use this for initialization
	void Start () {
        pullTaskManager = new TaskManager();
        pullFired = false;
	}
	
	// Update is called once per frame
	void Update () {
        pullTaskManager.Update();
        if (pullFired && !pullTaskManager.hasActiveTasks()) DestroyPull();
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
        transform.localPosition += positionOffset * direction;
        GetComponent<Rigidbody2D>().velocity = speed * direction;

        PullTask pullTask = new PullTask(parentPlayer.GetComponent<Player>(), this);
        pullTaskManager.AddTask(pullTask);
        pullFired = true;
    }

    public override void OnCastFinish()
    {
        Services.EventManager.Fire(new AbilityEnded(this));
    }

    public void DestroyPull()
    {
        Destroy(gameObject);
    }

}
