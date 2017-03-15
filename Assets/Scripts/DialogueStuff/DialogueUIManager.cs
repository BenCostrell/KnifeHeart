using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour {

	public GameObject dialogueText;
	public GameObject option1Obj;
	public GameObject option2Obj;
	public GameObject option3Obj;
	public GameObject option4Obj;

	private Dialogue option1Dialogue;
	private Dialogue option2Dialogue;
	private Dialogue option3Dialogue;
	private Dialogue option4Dialogue;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUpUI(){
		SetOptionUIStatus (false);
	}

	void SetOptionUIStatus(bool active){
		option1Obj.SetActive (active);
		option2Obj.SetActive (active);
		option3Obj.SetActive (active);
		option4Obj.SetActive (active);
	}

	public void SetDialogueOptions(Dialogue option1, Dialogue option2, Dialogue option3, Dialogue option4){
		option1Dialogue = option1;
		option2Dialogue = option2;
		option3Dialogue = option3;
		option4Dialogue = option4;
		SetOptionUIStatus (true);
		SetBlurbText ();
	}

	void SetBlurbText(){
		option1Obj.GetComponentInChildren<Text> ().text = option1Dialogue.blurb;
		option2Obj.GetComponentInChildren<Text> ().text = option2Dialogue.blurb;
		option3Obj.GetComponentInChildren<Text> ().text = option3Dialogue.blurb;
		option4Obj.GetComponentInChildren<Text> ().text = option4Dialogue.blurb;
	}
}
