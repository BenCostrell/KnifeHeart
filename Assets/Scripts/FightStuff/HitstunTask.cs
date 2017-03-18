using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstunTask : PlayerUnactionableTask {

	public HitstunTask(float dur, Player pl) : base(dur, pl){}

	protected override void Init ()
	{
		base.Init ();
		player.gameObject.GetComponent<SpriteRenderer> ().color = Color.red;
	}

	protected override void OnSuccess ()
	{
		base.OnSuccess ();
		player.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		player.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
	}
}
