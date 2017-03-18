using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastAbilityTask : PlayerUnactionableTask {

	private Ability ability;

	public CastAbilityTask(float dur, Player pl, Ability ab) : base(dur, pl){
		ability = ab;	
	}

	protected override void Init ()
	{
		base.Init ();
	}

	protected override void CleanUp ()
	{
		base.CleanUp ();
		ability.OnCastFinish ();
		player.ResetToNeutral ();
	}
}
