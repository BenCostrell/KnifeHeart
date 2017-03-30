using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanelImage : Task {
    private int panelNum;
    private GameObject panel;
    private GameObject incomingArena;

    public SetPanelImage(GameObject pnl, int num, GameObject newArena)
    {
        panelNum = num;
        panel = pnl;
        incomingArena = newArena;
    }

    protected override void Init()
    {
        Sprite panelSprite;
        if (Services.FightSceneManager.fallenPlayer.playerNum == 1)
        {
            if (incomingArena == Services.FightSceneManager.parkingLotArena)
            {
                panelSprite = Services.TransitionComicManager.transitionToParkingLotImagesPigtailsWins[panelNum];
            }
            else
            {
                panelSprite = Services.TransitionComicManager.transitionToHellImagesPigtailsWins[panelNum];
            }
        }
        else
        {
            if (incomingArena == Services.FightSceneManager.parkingLotArena)
            {
                panelSprite = Services.TransitionComicManager.transitionToParkingLotImagesPonytailWins[panelNum];
            }
            else
            {
                panelSprite = Services.TransitionComicManager.transitionToHellImagesPonytailWins[panelNum];
            }
        }

        panel.GetComponent<Image>().sprite = panelSprite;
        SetStatus(TaskStatus.Success);
    }

}
