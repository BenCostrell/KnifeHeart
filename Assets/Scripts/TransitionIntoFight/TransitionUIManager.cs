using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUIManager : MonoBehaviour {

	public GameObject fightBackground;
	public GameObject[] fightWords;
	public float fightBackgroundSlideInTime;
	public float fightWordGrowthTime;
	public float fightWordStaggerTime;


	// Use this for initialization
	void Start () {
		fightBackground.SetActive (false);
		SetFightWordsStatus (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetFightWordsStatus(bool active){
		for (int i = 0; i < fightWords.Length; i++) {
			fightWords [i].SetActive (active);
		}
	}
}
