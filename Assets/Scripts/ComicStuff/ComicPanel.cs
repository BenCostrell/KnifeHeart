using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicPanel : MonoBehaviour {

    public Sprite ponytailWinPanel;
    public Sprite pigtailsWinPanel;
    public bool switchOnPersonality;
    public Sprite ponytailBlinkPanel;
    public Sprite ponytailFireballPanel;
    public Sprite ponytailLungePanel;
    public Sprite ponytailPullPanel;
    public Sprite ponytailShieldPanel;
    public Sprite ponytailSingPanel;
    public Sprite ponytailWallopPanel;
    public Sprite pigtailsBlinkPanel;
    public Sprite pigtailsFireballPanel;
    public Sprite pigtailsLungePanel;
    public Sprite pigtailsPullPanel;
    public Sprite pigtailsShieldPanel;
    public Sprite pigtailsSingPanel;
    public Sprite pigtailsWallopPanel;

    private Image image;

    // Use this for initialization
    void Awake () {
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPanel(int winningPlayerNum)
    {
        if (switchOnPersonality)
        {
            if (winningPlayerNum == 1)
            {
                Ability.Type ability = Services.GameInfo.player1Abilities[Services.GameInfo.player1Abilities.Count - 1];
                switch (ability)
                {
                    case Ability.Type.Fireball:
                        image.sprite = ponytailFireballPanel;
                        break;
                    case Ability.Type.Lunge:
                        image.sprite = ponytailLungePanel;
                        break;
                    case Ability.Type.Sing:
                        image.sprite = ponytailSingPanel;
                        break;
                    case Ability.Type.Shield:
                        image.sprite = ponytailShieldPanel;
                        break;
                    case Ability.Type.Wallop:
                        image.sprite = ponytailWallopPanel;
                        break;
                    case Ability.Type.Pull:
                        image.sprite = ponytailPullPanel;
                        break;
                    case Ability.Type.Blink:
                        image.sprite = ponytailBlinkPanel;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Ability.Type ability = Services.GameInfo.player2Abilities[Services.GameInfo.player2Abilities.Count - 1];
                switch (ability)
                {
                    case Ability.Type.Fireball:
                        image.sprite = pigtailsFireballPanel;
                        break;
                    case Ability.Type.Lunge:
                        image.sprite = pigtailsLungePanel;
                        break;
                    case Ability.Type.Sing:
                        image.sprite = pigtailsSingPanel;
                        break;
                    case Ability.Type.Shield:
                        image.sprite = pigtailsShieldPanel;
                        break;
                    case Ability.Type.Wallop:
                        image.sprite = pigtailsWallopPanel;
                        break;
                    case Ability.Type.Pull:
                        image.sprite = pigtailsPullPanel;
                        break;
                    case Ability.Type.Blink:
                        image.sprite = pigtailsBlinkPanel;
                        break;
                    default:
                        break;
                }
            }
        }
        else {
            if (winningPlayerNum == 1)
            {
                image.sprite = ponytailWinPanel;
            }
            else
            {
                image.sprite = pigtailsWinPanel;
            }
        }
    }
}
