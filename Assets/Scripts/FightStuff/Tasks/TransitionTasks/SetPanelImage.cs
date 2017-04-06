using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanelImage : Task {
    private ComicPanel panel;

    public SetPanelImage(GameObject pnl)
    {
        panel = pnl.GetComponent<ComicPanel>();
    }

    protected override void Init()
    {
        panel.SetPanel(3 - Services.FightScene.fallenPlayer.playerNum);
        SetStatus(TaskStatus.Success);
    }

}
