using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCooldownTask : Task {
	private Player player;
	private float duration;
	private float timeRemaining;
	private Ability.Type ability;

	public AbilityCooldownTask(Ability.Type ab, float dur, Player pl){
		ability = ab;
		duration = dur;
		timeRemaining = dur;
		player = pl;
	}

	protected override void Init ()
	{
		player.abilitiesOnCooldown.Add (ability);
	}

	internal override void Update ()
	{
		timeRemaining -= Time.deltaTime;
		if (ability != Ability.Type.BasicAttack) {
			Services.FightUIManager.UpdateCooldownUI (ability, timeRemaining / duration);
		}
		if (timeRemaining <= 0) {
			SetStatus (TaskStatus.Success);
		}
	}

	protected override void OnSuccess ()
	{
		player.abilitiesOnCooldown.Remove (ability);
	}
}
