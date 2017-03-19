using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenUIManager : MonoBehaviour {
	public GameObject ponytail;
	public GameObject pigtails;
	public GameObject resetText;

	public Sprite ponytailWinSprite;
	public Sprite ponytailLoseSprite;
	public Sprite pigtailsWinSprite;
	public Sprite pigtailsLoseSprite;

	public float UISlideOffTime;
	public float spriteSlideInTime;
	public float winnerHighlightTime;
	public Vector3 losingPlayerSize;

	public int winningPlayerNum;

	// Use this for initialization
	void Start () {
		Services.EventManager.Register<GameOver> (StartWinScreenSequence);
		ponytail.SetActive (false);
		pigtails.SetActive (false);
		resetText.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void StartWinScreenSequence(GameOver e){
		if (e.losingPlayer.playerNum == 1) {
			winningPlayerNum = 2;
		} else {
			winningPlayerNum = 1;
		}

		SlideOutFightUI slideOutFightUI = new SlideOutFightUI ();
		SlideInWinScreenSprites slideInWinScreenSprites = new SlideInWinScreenSprites ();
		HighlightWinner highlightWinner = new HighlightWinner (winningPlayerNum);

		slideOutFightUI
			.Then (slideInWinScreenSprites)
			.Then (highlightWinner);

		Services.TaskManager.AddTask (slideOutFightUI);
	}
}
