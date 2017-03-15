using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FightSceneManager : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public Sprite player1Sprite;
	public Sprite player2Sprite;
	public GameObject playerPrefab;
	public Vector3 player1Spawn;
	public Vector3 player2Spawn;
	public RuntimeAnimatorController player1Anim;
	public RuntimeAnimatorController player2Anim;
	private GameObject gameInfo;
	public GameObject cooldownUIP1A1;
	public GameObject cooldownUIP1A2;
	public GameObject cooldownUIP2A1;
	public GameObject cooldownUIP2A2;
	public GameObject damageUIP1;
	public GameObject damageUIP2;
	public Sprite fireballUI;
	public Sprite lungeUI;
	public Sprite shieldUI;
	public Sprite singUI;
	private Dictionary<Ability.Type, Sprite> spriteDict;

	// Use this for initialization
	void Awake () {
		gameInfo = GameObject.FindGameObjectWithTag ("GameInfo");
		InitializeSpriteDict ();
		InitializePlayers ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Reset")) {
			if (Input.GetButton("HardReset")){
				Destroy(gameInfo);
				SceneManager.LoadScene("toyRoom");
			} else {
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}


	void InitializeSpriteDict(){
		spriteDict = new Dictionary<Ability.Type, Sprite> ();
		spriteDict.Add (Ability.Type.Fireball, fireballUI);
		spriteDict.Add (Ability.Type.Lunge, lungeUI);
		spriteDict.Add (Ability.Type.Shield, shieldUI);
		spriteDict.Add (Ability.Type.Sing, singUI);
	}

	void InitializePlayers(){
		player1 = Instantiate (playerPrefab, player1Spawn, Quaternion.identity) as GameObject;
		player2 = Instantiate (playerPrefab, player2Spawn, Quaternion.identity) as GameObject;

		InitializePlayer (player1, 1);
		InitializePlayer (player2, 2);
	}

	void InitializePlayer(GameObject player, int playerNum){
		Sprite playerSprite;
		RuntimeAnimatorController playerAnim;
		List<Ability.Type> abilityList;
		GameObject cooldownUIA1;
		GameObject cooldownUIA2;

		if (playerNum == 1) {
			playerSprite = player1Sprite;
			playerAnim = player1Anim;
			cooldownUIA1 = cooldownUIP1A1;
			cooldownUIA2 = cooldownUIP1A2;
			if (gameInfo == null) {
				abilityList = new List<Ability.Type> (){ Ability.Type.Fireball, Ability.Type.Lunge };
			} else {
				abilityList = gameInfo.GetComponent<GameInfo> ().player1Abilities;
			}
		} else {
			playerSprite = player2Sprite;
			playerAnim = player2Anim;
			cooldownUIA1 = cooldownUIP2A1;
			cooldownUIA2 = cooldownUIP2A2;
			if (gameInfo == null) {
				abilityList = new List<Ability.Type> (){ Ability.Type.Sing, Ability.Type.Shield };
			} else {
				abilityList = gameInfo.GetComponent<GameInfo> ().player2Abilities;
			}
		}

		Player pc = player.GetComponent<Player> ();
		pc.playerNum = playerNum;
		player.GetComponent<SpriteRenderer> ().sprite = playerSprite;
		player.GetComponent<Animator> ().runtimeAnimatorController = playerAnim;
		pc.abilityList = abilityList;
		Sprite spriteA1;
		spriteDict.TryGetValue (abilityList [0], out spriteA1);
		cooldownUIA1.GetComponent<SpriteRenderer> ().sprite = spriteA1;
		Sprite spriteA2;
		spriteDict.TryGetValue (abilityList [1], out spriteA2);
		cooldownUIA2.GetComponent<SpriteRenderer> ().sprite = spriteA2;
	}

	public void UpdateCooldownBar(int playerNum, int abilityNum, float fractionOfCooldownRemaining){
		Transform bar = null;
		if (playerNum == 1) {
			if (abilityNum == 1) {
				bar = cooldownUIP1A1.transform.GetChild (0);
			} else if (abilityNum == 2) {
				bar = cooldownUIP1A2.transform.GetChild (0);
			}
		}
		else if (playerNum == 2) {
			if (abilityNum == 1) {
				bar = cooldownUIP2A1.transform.GetChild (0);
			} else if (abilityNum == 2) {
				bar = cooldownUIP2A2.transform.GetChild (0);
			}
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
