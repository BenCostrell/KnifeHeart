using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FightSceneManager : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public Sprite player1Sprite;
	public Sprite player2Sprite;
	public Vector3 player1Spawn;
	public Vector3 player2Spawn;
	public RuntimeAnimatorController player1Anim;
	public RuntimeAnimatorController player2Anim;



	// Use this for initialization
	void Start () {
		InitializePlayers ();
	}
	
	// Update is called once per frame
	void Update () {
	}



	void InitializePlayers(){
		player1 = Instantiate (Services.PrefabDB.Player, player1Spawn, Quaternion.identity) as GameObject;
		player2 = Instantiate (Services.PrefabDB.Player, player2Spawn, Quaternion.identity) as GameObject;

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
			abilityList = Services.GameInfo.player1Abilities;

		} else {
			playerSprite = player2Sprite;
			playerAnim = player2Anim;
			abilityList = Services.GameInfo.player2Abilities;
		}

		Player pc = player.GetComponent<Player> ();
		pc.playerNum = playerNum;
		player.GetComponent<SpriteRenderer> ().sprite = playerSprite;
		player.GetComponent<Animator> ().runtimeAnimatorController = playerAnim;
		pc.abilityList = abilityList;
	}
}
