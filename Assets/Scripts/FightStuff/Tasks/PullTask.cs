using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullTask : InterruptibleByFallTask {
	private Player player;
	private Pull pull;
	private Rigidbody2D pullRb;
	private bool retract;
	private Player hookedPlayer;

	public PullTask(Player pl, Pull pul){
		player = pl;
		pull = pul;
	}

	protected override void Init ()
	{
        base.Init();
		retract = false;
		Services.EventManager.Register<PlayerHooked> (OnPlayerHooked);
		player.StopListeningForInput ();
		pullRb = pull.GetComponent<Rigidbody2D> ();
		Services.EventManager.Register<GameOver> (OnGameOver);
        Services.EventManager.Register<AbilityEnded>(OnAbilityEnded);
	}

	internal override void Update ()
	{
        if (retract) {
            if (pullRb == null || pull == null || hookedPlayer == null || player == null) SetStatus(TaskStatus.Fail);
            else {
                pullRb.velocity = pull.speed * (player.transform.position - hookedPlayer.transform.position).normalized;
                hookedPlayer.gameObject.GetComponent<Rigidbody2D>().MovePosition(pull.transform.position);
                if (InPosition())
                {
                    SetStatus(TaskStatus.Success);
                }
            }
		} 
	}

	bool InPosition(){
        return (Vector3.Distance(player.transform.position, hookedPlayer.transform.position) <= pull.distanceToPullTo);
	}


	void OnGameOver(GameOver e){
		Abort ();
	}

	void OnPlayerHooked(PlayerHooked e){
		hookedPlayer = e.hookedPlayer;
		retract = true;
		hookedPlayer.StopListeningForInput ();
        //pull.GetComponent<TrailRenderer>().Clear();
	}

    void OnAbilityEnded(AbilityEnded e)
    {
        if (e.ability == pull && hookedPlayer == null) SetStatus(TaskStatus.Aborted);
        if (e.ability == pull && e.ability.parentPlayer.GetComponent<Player>().stunned) SetStatus(TaskStatus.Aborted);
    }

	protected override void OnSuccess ()
	{
		hookedPlayer.Stun (pull.hitstun);
        pull.OnCastFinish();
        player.StartListeningForInput();
        //Debug.Log("pull stun");
    }

    protected override void OnAbort()
    {
        base.OnAbort();
        if (hookedPlayer != null) hookedPlayer.Stun(pull.hitstun);
    }


    protected override void CleanUp ()
	{
        base.CleanUp();
        Services.EventManager.Unregister<PlayerHooked> (OnPlayerHooked);
		Services.EventManager.Unregister<GameOver> (OnGameOver);
        Services.EventManager.Unregister<AbilityEnded>(OnAbilityEnded);
        if (pull != null) pull.DestroyPull();
	}
}
