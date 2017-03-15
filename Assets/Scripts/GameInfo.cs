using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo {

	public GameInfo(){
		player1Abilities = new List<Ability.Type> ();
		player2Abilities = new List<Ability.Type> ();
	}

	public List<Ability.Type> player1Abilities;
	public List<Ability.Type> player2Abilities;
}
