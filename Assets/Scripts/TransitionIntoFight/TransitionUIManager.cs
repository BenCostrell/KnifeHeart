using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUIManager : MonoBehaviour {

	public GameObject fightBackground;
	public GameObject wordsContainer;
	public GameObject[] fightWords;

	public GameObject transitionUI;
	public GameObject readyPrompt_P1;
	public GameObject readyPrompt_P2;
	public GameObject ready_P1;
	public GameObject ready_P2;

	public float fightBackgroundSlideInTime;
	public float fightWordGrowthTime;
	public float fightWordStaggerTime;
	public float readyPromptGrowTime;
	public float readyPromptShrinkTime;
	public float uiScaleOutTime;
	public float transitionEndTime;


	// Use this for initialization
	void Start () {
		fightBackground.SetActive (false);
		SetFightWordsStatus (false);
		transitionUI.SetActive (false);
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
