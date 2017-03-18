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
		Services.EventManager.Fire (new PlayerInputPaused (player));
		Services.EventManager.Register<PlayerInputPaused> (AnotherInputPauseTaskWasStarted);
		player.StopListeningForInput ();
	}

	protected void AnotherInputPauseTaskWasStarted(PlayerInputPaused e){
		if (e.player == player) {
			Abort ();
		}
	}

	internal override void Update ()
	{
		duration -= Time.deltaTime;
		if (duration <= 0) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void CleanUp ()
	{
		Services.EventManager.Unregister<PlayerInputPaused> (AnotherInputPauseTaskWasStarted);
		player.StartListeningForInput ();
	}
}
