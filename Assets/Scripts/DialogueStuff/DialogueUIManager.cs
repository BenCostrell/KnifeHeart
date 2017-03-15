using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUIManager : MonoBehaviour {

	public GameObject dialogueText;
	public GameObject option1;
	public GameObject option2;
	public GameObject option3;
	public GameObject option4;

	// Use this for initialization
	void Start () {
		SetOptionUIStatus (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetOptionUIStatus(bool active){
		option1.SetActive (active);
		option2.SetActive (active);
		option3.SetActive (active);
		option4.SetActive (active);
	}
}
