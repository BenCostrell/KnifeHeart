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
	private Dictionary<Ability.Type, Sprite> spriteDict;
	private Dictionary<Ability.Type, GameObject> cooldownBarDict;

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
	}

	void InitializeUI(){
		cooldownBarDict = new Dictionary<Ability.Type, GameObject> ();
		for (int i = 0; i < cooldownUI_P1.Length - 1; i++) {
			GameObject obj_P1 = cooldownUI_P1 [i];
			GameObject obj_P2 = cooldownUI_P2 [i];
			GameObject timer_P1 = obj_P1.transform.GetChild (0).gameObject;
			GameObject timer_P2 = obj_P2.transform.GetChild (0).gameObject;
			GameObject icon_P1 = obj_P1.transform.GetChild (1).gameObject;
			GameObject icon_P2 = obj_P2.transform.GetChild (1).gameObject;
			Ability.Type ability_P1 = Services.GameInfo.player1Abilities [i];
			Ability.Type ability_P2 = Services.GameInfo.player2Abilities [i];

			icon_P1.GetComponent<Image> ().sprite = spriteDict [ability_P1];
			icon_P2.GetComponent<Image> ().sprite = spriteDict [ability_P2];

			cooldownBarDict.Add (ability_P1, timer_P1);
			cooldownBarDict.Add (ability_P2, timer_P2);
		}
	}

	public void UpdateCooldownUI(Ability.Type ability, float fractionRemaining){
		GameObject timer = cooldownBarDict [ability];
		Image image = timer.GetComponent<Image> ();
		float alpha;
		image.fillAmount = 1 - fractionRemaining;
		if (fractionRemaining > 0) {
			alpha = 0.5f;
		} else {
			alpha = 1;
		}
		image.color = new Color (image.color.r, image.color.g, image.color.b, alpha);
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
