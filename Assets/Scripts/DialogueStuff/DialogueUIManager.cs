using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour {

	public GameObject startScreen;
	public GameObject crowdImage;
	public GameObject dialogueContainer;
	public GameObject dialogueText;
	public GameObject dialogueTextBox;
	public GameObject[] optionObjects;
	public GameObject continueIndicator;
	public GameObject arrow_P1;
	public GameObject arrow_P2;
	public GameObject ponytail;
	public GameObject pigtails;


	public GameObject selectedOption;

	private Dialogue[] optionDialogues;

	public Dialogue queuedDialogue;

	public Color textBoxColor_P1;
	public Color textBoxColor_P2;
	public Sprite textBox;
	public Sprite textBoxHighlighted;

	public string initialDialogue;

	public float optionAppearanceStaggerTime;
	public float optionAppearanceTime;
	public float indicatorFlashUptime;
	public float unselectedOptionShrinkTime;
	public float selectedOptionHighlightTime;
	public float crowdSlideTime;

	// Use this for initialization
	void Start () {
		Services.EventManager.Register<DialoguePicked> (QueueDialogue);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUpUI(){
		dialogueText.GetComponent<Text> ().text = "";
		optionDialogues = new Dialogue[4];
		continueIndicator.SetActive (false);
		arrow_P1.SetActive (false);
		arrow_P2.SetActive (false);
		arrow_P1.GetComponent<Image> ().color = textBoxColor_P1;
		arrow_P2.GetComponent<Image> ().color = textBoxColor_P2;
		crowdImage.SetActive (false);

		SetOptionUIStatus (false);
	}

	public void SetOptionUIStatus(bool active){
		for(int i = 0; i < optionObjects.Length; i++) {
			if (optionDialogues [i] != null) {
				optionObjects [i].SetActive (active);
			} else {
				optionObjects [i].SetActive (false);
			}
		}
	}

	public void SetDialogueOptions(Dialogue[] dialogueOptions){
		optionDialogues = dialogueOptions;
		SetBlurbText ();
		SetTextBoxColor (Services.VisualNovelSceneManager.currentTurnPlayerNum, true, false);
        SetButtonSide(Services.VisualNovelSceneManager.currentTurnPlayerNum);
	}

	public void ActivateOptionTextBox(int optionNum){
		optionObjects [optionNum - 1].SetActive (true);
	}

	void SetBlurbText(){
		for (int i = 0; i < optionObjects.Length; i++) {
			if (optionDialogues [i] != null) {
				optionObjects [i].GetComponentInChildren<Text> ().text = optionDialogues [i].blurb;
			} else {
				optionObjects [i].GetComponentInChildren<Text> ().text = "";
			}
		}
	}

	public Dialogue GetDialogueFromInput(string buttonName){
		switch (buttonName) {
		case "Y":
			return optionDialogues[0];
		case "X":
			return optionDialogues[1];
		case "B":
			return optionDialogues[2];
		case "A":
			return optionDialogues[3];
		default:
			return null;
		}
	}

	public GameObject GetOptionObjectFromInput(string buttonName){
		switch (buttonName) {
		case "Y":
			return optionObjects[0];
		case "X":
			return optionObjects[1];
		case "B":
			return optionObjects[2];
		case "A":
			return optionObjects[3];
		default:
			return null;
		}
	}

	public void QueueDialogue(DialoguePicked e){
		queuedDialogue = e.dialogue;
		selectedOption = e.optionObject;
	}

    public void SetButtonSide(int playerNum)
    {
        int side = 1;
        if (playerNum == 1)
        {
            side *= -1;
        }

        for (int i = 0; i < optionObjects.Length; i++) {
            RectTransform rectTransform = optionObjects[i].transform.GetChild(0).GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(side * Mathf.Abs(rectTransform.anchoredPosition.x), 
                rectTransform.anchoredPosition.y);
        }
    }
    
	public void SetTextBoxColor(int playerNum, bool options, bool dialogueBox){
		Color textBoxColor = Color.white;
		if (playerNum == 1) {
			textBoxColor = textBoxColor_P1;
		} else if (playerNum == 2) {
			textBoxColor = textBoxColor_P2;
		}
		if (options) {
			for (int i = 0; i < optionObjects.Length; i++) {
				optionObjects [i].GetComponentsInChildren<Image> () [1].color = textBoxColor;
			}
		}
		if (dialogueBox) {
			dialogueTextBox.GetComponent<Image> ().color = textBoxColor;
		}
	}
}
