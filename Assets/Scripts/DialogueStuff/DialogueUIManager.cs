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
    public GameObject[] rpsOptionObjects;
    public GameObject rpsTimer;
    public GameObject rpsReady_P1;
    public GameObject rpsReady_P2;
	public GameObject continueIndicator;
	public GameObject arrow_P1;
	public GameObject arrow_P2;
	public GameObject ponytail;
	public GameObject pigtails;


	public GameObject selectedOption;

	private Dialogue[] optionDialogues;

	public Dialogue queuedDialogue;

	public Sprite textBox;
	public Sprite textBoxHighlighted;

	public string initialDialogue;

	public float optionAppearanceStaggerTime;
	public float optionAppearanceTime;
	public float indicatorFlashUptime;
	public float unselectedOptionShrinkTime;
	public float selectedOptionHighlightTime;
	public float crowdSlideTime;
    public float rpsWaitTime;
    public float rpsDialogueDelay;

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
		crowdImage.SetActive (false);
		SetOptionUIStatus (false);
        SetRpsOptionUIStatus(false);
        rpsReady_P1.SetActive(false);
        rpsReady_P2.SetActive(false);
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

    public void SetRpsOptionUIStatus(bool active)
    {
        for (int i = 0; i < rpsOptionObjects.Length; i++)
        {
            rpsOptionObjects[i].SetActive(active);
        }
    }

	public void SetDialogueOptions(Dialogue[] dialogueOptions){
		optionDialogues = dialogueOptions;
		SetBlurbText ();
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
}
