using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnactionableTask : InterruptibleByFallTask {
	protected float duration;
	protected Player player;

	public PlayerUnactionableTask(float dur, Player pl){
		duration = dur;
		player = pl;
	}

	protected override void Init ()
	{
        base.Init();
        Services.EventManager.Fire (new PlayerInputPaused (player));
		Services.EventManager.Register<PlayerInputPaused> (AnotherInputPauseTaskWasStarted);
		Services.EventManager.Register<GameOver> (OnGameOver);
		player.StopListeningForInput ();
	}

	protected void AnotherInputPauseTaskWasStarted(PlayerInputPaused e){
		if (e.player == player) {
			Abort ();
		}
	}

	protected void OnGameOver(GameOver e){
		Abort ();
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
        base.CleanUp();
        Services.EventManager.Unregister<GameOver> (OnGameOver);
		Services.EventManager.Unregister<PlayerInputPaused> (AnotherInputPauseTaskWasStarted);
		player.StartListeningForInput ();
	}
}
