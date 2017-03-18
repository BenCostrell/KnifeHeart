using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallopTask : Task {
	private float timeUntilHitboxActive;
	private Player player;
	private Wallop wallop;

	public WallopTask(Player pl, Wallop wal){
		player = pl;
		wallop = wal;
	}

	protected override void Init ()
	{
		wallop.gameObject.GetComponent<Collider2D> ().enabled = false;
		timeUntilHitboxActive = wallop.delay;
	}

	internal override void Update ()
	{
		timeUntilHitboxActive -= Time.deltaTime;
		if (timeUntilHitboxActive <= 0) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		wallop.gameObject.GetComponent<Collider2D> ().enabled = true;
		float angle = wallop.transform.localRotation.eulerAngles.z * Mathf.Deg2Rad;
		wallop.transform.localPosition = 3 * new Vector3 (-Mathf.Cos (angle), -Mathf.Sin (angle), 0);
	}
}
