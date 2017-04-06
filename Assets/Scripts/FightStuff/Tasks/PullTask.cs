using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullTask : InterruptibleByFallTask {
	private Player player;
	private Pull pull;
	private Rigidbody2D pullRb;
	private float timeElapsed;
	private float duration;
	private bool retract;
	private Player hookedPlayer;
	private LineRenderer line;

	public PullTask(Player pl, Pull pul){
		player = pl;
		pull = pul;
		duration = pull.castDuration;
	}

	protected override void Init ()
	{
        base.Init();
        timeElapsed = 0;
		retract = false;
		Services.EventManager.Register<PlayerHooked> (OnPlayerHooked);
		player.StopListeningForInput ();
		line = pull.gameObject.GetComponent<LineRenderer> ();
		pullRb = pull.GetComponent<Rigidbody2D> ();
		Services.EventManager.Unregister<GameOver> (OnGameOver);
	}

	internal override void Update ()
	{
		line.SetPosition (0, player.transform.position);
		line.SetPosition (1, pull.transform.position);
		if (retract) {
			pullRb.velocity = pull.speed * (player.transform.position - hookedPlayer.transform.position).normalized;
			hookedPlayer.gameObject.GetComponent<Rigidbody2D> ().MovePosition (pull.transform.position);
			if (InPosition ()) {
				SetStatus (TaskStatus.Success);
			}
		} else {
			timeElapsed += Time.deltaTime;
			if (timeElapsed >= duration) {
				SetStatus (TaskStatus.Fail);
			}
		}
	}

	bool InPosition(){
		if (Vector3.Distance (player.transform.position, hookedPlayer.transform.position) <= pull.distanceToPullTo) {
			return true;
		} else {
			return false;
		}
	}


	void OnGameOver(GameOver e){
		Abort ();
	}

	void OnPlayerHooked(PlayerHooked e){
		hookedPlayer = e.hookedPlayer;
		retract = true;
		hookedPlayer.StopListeningForInput ();
	}

	protected override void OnSuccess ()
	{
		hookedPlayer.StartListeningForInput ();
		hookedPlayer.Stun (pull.hitstun);
	}

	protected override void CleanUp ()
	{
        base.CleanUp();
        Services.EventManager.Unregister<PlayerHooked> (OnPlayerHooked);
		Services.EventManager.Unregister<GameOver> (OnGameOver);
		pull.OnFinish ();
		player.StartListeningForInput ();
	}
}
