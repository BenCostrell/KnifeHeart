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
		for (int i = 0; i < cooldownUI_P1.Length; i++) {
			GameObject obj_P1 = cooldownUI_P1 [i];
			GameObject obj_P2 = cooldownUI_P2 [i];
			GameObject bar_P1 = obj_P1.transform.GetChild (0).gameObject;
			GameObject bar_P2 = obj_P2.transform.GetChild (0).gameObject;
			Ability.Type ability_P1 = Services.GameInfo.player1Abilities [i];
			Ability.Type ability_P2 = Services.GameInfo.player2Abilities [i];

			obj_P1.GetComponent<SpriteRenderer> ().sprite = spriteDict [ability_P1];
			obj_P2.GetComponent<SpriteRenderer> ().sprite = spriteDict [ability_P2];

			bar_P1.GetComponent<SpriteRenderer> ().color = Color.green;
			bar_P2.GetComponent<SpriteRenderer> ().color = Color.green;

			cooldownBarDict.Add (ability_P1, bar_P1);
			cooldownBarDict.Add (ability_P2, bar_P2);
		}
	}

	public void UpdateCooldownUI(Ability.Type ability, float fractionRemaining){
		GameObject bar = cooldownBarDict [ability];
		bar.transform.localScale = Vector3.Lerp (new Vector3 (0, bar.transform.localScale.y, bar.transform.localScale.z),
			new Vector3 (0.4f, bar.transform.localScale.y, bar.transform.localScale.z), 1 - fractionRemaining);
		if (fractionRemaining > 0) {
			bar.GetComponent<SpriteRenderer> ().color = Color.red;
		} else {
			bar.GetComponent<SpriteRenderer> ().color = Color.green;
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
