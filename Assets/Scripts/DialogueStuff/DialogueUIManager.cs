using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour {

	public GameObject dialogueText;
	public GameObject[] optionObjects;

	private Dialogue[] optionDialogues;

	public Dialogue queuedDialogue;

	// Use this for initialization
	void Start () {
		Services.EventManager.Register<DialoguePicked> (QueueDialogue);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUpUI(){
		SetOptionUIStatus (false);
		dialogueText.GetComponent<Text> ().text = "";
	}

	void SetOptionUIStatus(bool active){
		foreach (GameObject optionObj in optionObjects) {
			optionObj.SetActive (active);
		}
	}

	public void SetDialogueOptions(Dialogue[] dialogueOptions){
		optionDialogues = dialogueOptions;
		SetOptionUIStatus (true);
		SetBlurbText ();
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

	public void QueueDialogue(DialoguePicked e){
		queuedDialogue = e.dialogue;
	}
}
