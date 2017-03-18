using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightUIManager : MonoBehaviour {

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
		for (int i = 0; i < cooldownUI_P1.Length; i++) {
			cooldownUI_P1 [i].GetComponent<SpriteRenderer> ().sprite = spriteDict [Services.GameInfo.player1Abilities [i]];
			cooldownUI_P2 [i].GetComponent<SpriteRenderer> ().sprite = spriteDict [Services.GameInfo.player2Abilities [i]];
		}
	}

	public void UpdateCooldownBar(int playerNum, int abilityNum, float fractionOfCooldownRemaining){
		Transform bar = null;
		if (playerNum == 1) {
			bar = cooldownUI_P1 [abilityNum].transform.GetChild (0);
		} else if (playerNum == 2) {
			bar = cooldownUI_P2 [abilityNum].transform.GetChild (0);
		}
		bar.localScale = new Vector3 (0.4f * (1 - fractionOfCooldownRemaining), bar.localScale.y, bar.localScale.z);

		if (fractionOfCooldownRemaining > 0) {
			bar.gameObject.GetComponent<SpriteRenderer> ().color = Color.red;
		} else {
			bar.gameObject.GetComponent<SpriteRenderer> ().color = Color.green;
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
			damageUI.GetComponent<TextMesh> ().text = pc.damage.ToString();
		}
	}
}
