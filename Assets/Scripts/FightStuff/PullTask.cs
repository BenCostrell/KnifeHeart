using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullTask : Task {
	private Player player;
	private Pull pull;
	private float timeElapsed;
	private float duration;
	private bool retract;
	private Player hookedPlayer;

	public PullTask(Player pl, Pull pul){
		player = pl;
		pull = pul;
		duration = pull.castDuration;
	}

	protected override void Init ()
	{
		timeElapsed = 0;
		retract = false;
		Services.EventManager.Register<PlayerHooked> (OnPlayerHooked);
		player.StopListeningForInput ();
	}

	internal override void Update ()
	{
		if (retract) {
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

	void OnPlayerHooked(PlayerHooked e){
		pull.GetComponent<Rigidbody2D> ().velocity *= -1;
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
		Services.EventManager.Unregister<PlayerHooked> (OnPlayerHooked);
		pull.OnFinish ();
		player.StartListeningForInput ();
	}
}
