using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnactionableTask : Task {
	protected float duration;
	protected Player player;

	public PlayerUnactionableTask(float dur, Player pl){
		duration = dur;
		player = pl;
	}

	protected override void Init ()
	{
		player.StopListeningForInput ();
	}

	internal override void Update ()
	{
		duration -= Time.deltaTime;
		Debug.Log (duration);
		if (duration <= 0) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		player.StartListeningForInput ();
	}
}
