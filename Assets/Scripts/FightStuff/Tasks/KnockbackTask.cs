using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackTask : HitstunTask {
	private Vector3 knockback;
    private float distanceSinceLastEmission;
    private Vector3 lastPosition;

	public KnockbackTask(float hitstunDur, Player pl, Vector3 knockbackVector) : base(hitstunDur, pl){
		knockback = knockbackVector;
	}

	protected override void Init ()
	{
		base.Init ();
        distanceSinceLastEmission = 0;
        lastPosition = player.transform.position;
        player.GetComponent<Rigidbody2D> ().velocity = knockback;
        ActionTask updateDamageUI = new ActionTask(Services.FightUIManager.UpdateDamageUI);
        Services.TaskManager.AddTask(updateDamageUI);
        player.stageEdgeBoundaryCollider.enabled = false;
        player.UpdateRotation(-knockback);
        player.anim.SetTrigger("Stunned");
        player.rb.drag = player.knockbackDrag;
        player.EmitKnockbackTrail();
    }

    internal override void Update()
    {
        base.Update();
        player.UpdateRotation(-player.rb.velocity);
        distanceSinceLastEmission += (player.transform.position - lastPosition).magnitude;
        lastPosition = player.transform.position;
        if (distanceSinceLastEmission > player.kbTrailEmissionDistance)
        {
            player.EmitKnockbackTrail();
            distanceSinceLastEmission = 0;
        }

    }

    protected override void CleanUp()
    {
        base.CleanUp();
        player.stageEdgeBoundaryCollider.enabled = true;
        player.rb.drag = player.baseDrag;
        player.knockbackTrail.Stop();
    }
}
