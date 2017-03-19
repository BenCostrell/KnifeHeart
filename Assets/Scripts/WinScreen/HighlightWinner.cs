using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightWinner : Task {

	private float duration;
	private float timeElapsed;
	private Vector3 winnerInitialPos;
	private Vector3 loserInitialPos;
	private int winningPlayerNum;
	private GameObject winner;
	private GameObject loser;
	private Vector3 loserShiftDirection;

	public HighlightWinner(int winningNum){
		winningPlayerNum = winningNum;
	}

	protected override void Init ()
	{
		duration = Services.WinScreenUIManager.winnerHighlightTime;
		timeElapsed = 0;
		if (winningPlayerNum == 1) {
			winner = Services.WinScreenUIManager.ponytail;
			loser = Services.WinScreenUIManager.pigtails;
			loserShiftDirection = Vector3.right;
		} else {
			winner = Services.WinScreenUIManager.pigtails;
			loser = Services.WinScreenUIManager.ponytail;
			loserShiftDirection = Vector3.left;
		}
		winnerInitialPos = winner.transform.position;
		loserInitialPos = loser.transform.position;
	}

	internal override void Update ()
	{
		timeElapsed += Time.deltaTime;

		winner.transform.position = Vector3.Lerp (winnerInitialPos, Vector3.zero, Easing.ExpoEaseOut (timeElapsed / duration));
		loser.transform.position = Vector3.Lerp (loserInitialPos, loserInitialPos + (2f * loserShiftDirection) + (2.8f * Vector3.down), 
			Easing.ExpoEaseOut (timeElapsed / duration));
		loser.transform.localScale = Vector3.Lerp (3f * Vector3.one, 2f * Vector3.one, Easing.ExpoEaseOut (timeElapsed / duration));

		if (timeElapsed >= duration) {
			SetStatus (TaskStatus.Success);
		}
	}

	void SetSprites(){
		if (winningPlayerNum == 1) {
			winner.GetComponent<SpriteRenderer> ().sprite = Services.WinScreenUIManager.ponytailWinSprite;
			loser.GetComponent<SpriteRenderer> ().sprite = Services.WinScreenUIManager.pigtailsLoseSprite;
		} else {
			winner.GetComponent<SpriteRenderer> ().sprite = Services.WinScreenUIManager.pigtailsWinSprite;
			loser.GetComponent<SpriteRenderer> ().sprite = Services.WinScreenUIManager.ponytailLoseSprite;
		}
	}

	protected override void OnSuccess ()
	{
		SetSprites ();
		Services.WinScreenUIManager.resetText.SetActive (true);
		Services.WinScreenUIManager.resetText.transform.position += -10.5f * loserShiftDirection;
	}
}
