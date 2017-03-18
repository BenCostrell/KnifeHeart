using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackTask : HitstunTask {
	private Vector3 knockback;

	public KnockbackTask(float hitstunDur, Player pl, Vector3 knockbackVector) : base(hitstunDur, pl){
		knockback = knockbackVector;
	}

	protected override void Init ()
	{
		base.Init ();
		player.GetComponent<Rigidbody2D> ().velocity = knockback;
	}
}
