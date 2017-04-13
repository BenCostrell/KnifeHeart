using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUIManager : MonoBehaviour {

	public GameObject UI_P1;
	public GameObject UI_P2;

	public GameObject[] cooldownUI_P1;
	public GameObject[] cooldownUI_P2;

	public GameObject damageUIP1;
	public GameObject damageUIP2;

	public Sprite fireballUI;
	public Sprite lungeUI;
	public Sprite shieldUI;
	public Sprite singUI;
	public Sprite wallopUI;
	public Sprite pullUI;
    public Sprite blinkUI;
	private Dictionary<Ability.Type, Sprite> spriteDict;
	private Dictionary<Ability.Type, GameObject> cooldownBarDict;

    public float abCDHighlightScale;
    public float abCDHighlightTime;


    // Use this for initialization
    void Start () {
		InitializeSpriteDict ();
		InitializeUI ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void InitializeSpriteDict(){
		spriteDict = new Dictionary<Ability.Type, Sprite> ();
		spriteDict.Add (Ability.Type.Fireball, fireballUI);
		spriteDict.Add (Ability.Type.Lunge, lungeUI);
		spriteDict.Add (Ability.Type.Shield, shieldUI);
		spriteDict.Add (Ability.Type.Sing, singUI);
		spriteDict.Add (Ability.Type.Wallop, wallopUI);
		spriteDict.Add (Ability.Type.Pull, pullUI);
        spriteDict.Add(Ability.Type.Blink, blinkUI);
	}

	void InitializeUI(){
		cooldownBarDict = new Dictionary<Ability.Type, GameObject> ();
        for (int i = 0; i < cooldownUI_P1.Length - 1; i++) {
            GameObject obj_P1 = cooldownUI_P1[i];
            GameObject obj_P2 = cooldownUI_P2[i];
            if (i < Services.FightScene.roundNum)
            {
                
                GameObject timer_P1 = obj_P1.transform.GetChild(0).gameObject;
                GameObject timer_P2 = obj_P2.transform.GetChild(0).gameObject;
                GameObject icon_P1 = obj_P1.transform.GetChild(1).gameObject;
                GameObject icon_P2 = obj_P2.transform.GetChild(1).gameObject;
                Ability.Type ability_P1 = Services.GameInfo.player1Abilities[i];
                Ability.Type ability_P2 = Services.GameInfo.player2Abilities[i];

                icon_P1.GetComponent<Image>().sprite = spriteDict[ability_P1];
                icon_P2.GetComponent<Image>().sprite = spriteDict[ability_P2];

                cooldownBarDict.Add(ability_P1, timer_P1);
                cooldownBarDict.Add(ability_P2, timer_P2);
            }
            else
            {
                obj_P1.SetActive(false);
                obj_P2.SetActive(false);
            }
		}
	}

    GameObject GetTimer(Ability.Type ability, int playerNum)
    {
        GameObject timer;
        if (ability != Ability.Type.BasicAttack)
        {
            timer = cooldownBarDict[ability];
        }
        else
        {
            if (playerNum == 1)
            {
                timer = cooldownUI_P1[3].transform.GetChild(0).gameObject;
            }
            else
            {
                timer = cooldownUI_P2[3].transform.GetChild(0).gameObject;
            }
        }

        return timer;
    }

	public void UpdateCooldownUI(Ability.Type ability, float fractionRemaining, int playerNum){
        Image timerImage = GetTimer(ability, playerNum).GetComponent<Image>();
		float alpha;
        Color iconColor;
		timerImage.fillAmount = 1 - fractionRemaining;
		if (fractionRemaining > 0) {
			alpha = 0.5f;
            iconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		} else {
			alpha = 1;
            iconColor = Color.black;
		}
		timerImage.color = new Color (timerImage.color.r, timerImage.color.g, timerImage.color.b, alpha);
        timerImage.transform.parent.GetChild(1).GetComponent<Image>().color = iconColor;
    }

    public void ScaleCooldownUI(Ability.Type ability, int playerNum, float scale)
    {
        Transform timerTransform = GetTimer(ability, playerNum).transform;
        timerTransform.localScale = scale * Vector3.one;
        if (scale == 1)
        {
            timerTransform.parent.GetChild(1).GetComponent<Image>().color = Color.black;
        }
        else
        {
            timerTransform.parent.GetChild(1).GetComponent<Image>().color = Color.white;
        }
    }

	public void UpdateDamageUI(GameObject player){
		Player pc = player.GetComponent<Player> ();
		GameObject damageUI = null;
		if (pc.playerNum == 1) {
			damageUI = damageUIP1;
		} else if (pc.playerNum == 2) {
			damageUI = damageUIP2;
		}
		if (damageUI != null) {
			damageUI.GetComponentInChildren<Text> ().text = pc.damage.ToString();
		}
	}
}
