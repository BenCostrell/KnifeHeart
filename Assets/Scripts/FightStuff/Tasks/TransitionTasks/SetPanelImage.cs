using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanelImage : Task {
    private int panelNum;
    private int pageNum;
    private GameObject panel;
    private GameObject incomingArena;

    public SetPanelImage(GameObject pnl, int pnlNum, int pgNum, GameObject newArena)
    {
        panelNum = pnlNum;
        pageNum = pgNum;
        panel = pnl;
        incomingArena = newArena;
    }

    protected override void Init()
    {
        Sprite panelSprite;
        if (Services.FightScene.fallenPlayer.playerNum == 1)
        {
            if (incomingArena == Services.FightScene.parkingLotArena)
            {
                panelSprite = Services.TransitionComicManager.transitionToParkingLotImagesPigtailsWins[panelNum];
            }
            else if (incomingArena == Services.FightScene.hellArena)
            {
                panelSprite = Services.TransitionComicManager.transitionToHellImagesPigtailsWins[panelNum];
            }
            else
            {
                panelSprite = Services.TransitionComicManager.winComicPigtailsWins[pageNum][panelNum];
            }
        }
        else
        {
            if (incomingArena == Services.FightScene.parkingLotArena)
            {
                panelSprite = Services.TransitionComicManager.transitionToParkingLotImagesPonytailWins[panelNum];
            }
            else if (incomingArena == Services.FightScene.hellArena)
            {
                panelSprite = Services.TransitionComicManager.transitionToHellImagesPonytailWins[panelNum];
            }
            else
            {
                panelSprite = Services.TransitionComicManager.winComicPonytailWins[pageNum][panelNum];
            }
        }

        panel.GetComponent<Image>().sprite = panelSprite;
        SetStatus(TaskStatus.Success);
    }

}
