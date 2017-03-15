using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue {
	public string mainText;
	public string blurb;
	public Ability.Type abilityGiven;

	public Dialogue(string mainTxt, string blurbTxt, Ability.Type ability){
		mainText = mainTxt;
		blurb = blurbTxt;
		abilityGiven = ability;
	}
}
