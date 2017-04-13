using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HighlightAbilityOffCooldown : InterruptibleByFallTask
{
    private float duration;
    private float timeElapsed;
    private Ability.Type ability;
    private Player player;

    public HighlightAbilityOffCooldown(Ability.Type ab, Player pl, float dur)
    {
        ability = ab;
        player = pl;
        duration = dur;
    }

    protected override void Init()
    {
        base.Init();
        timeElapsed = 0;
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

        if (timeElapsed <= duration / 2)
        {
            Services.FightUIManager.ScaleCooldownUI(ability, player.playerNum,
                Mathf.Lerp(1, Services.FightUIManager.abCDHighlightScale, 
                Easing.ExpoEaseOut(timeElapsed / (duration / 2))));
        }
        else
        {
            Services.FightUIManager.ScaleCooldownUI(ability, player.playerNum,
                Mathf.Lerp(Services.FightUIManager.abCDHighlightScale, 1,
                Easing.ExpoEaseOut((timeElapsed - (duration / 2)) / (duration / 2))));
        }

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
