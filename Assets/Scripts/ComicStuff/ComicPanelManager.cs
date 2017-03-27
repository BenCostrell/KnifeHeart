using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicPanelManager : MonoBehaviour {

    public GameObject comicBackground;
    public GameObject scenario1;
    public GameObject scenario2;
    public GameObject scenario3;
    public GameObject continueButton;
    public float panelAppearTime;


    // Use this for initialization
    void Start () {
        TurnOffComicPanels();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void TurnOffComicPanels()
    {
        comicBackground.SetActive(false);
        continueButton.SetActive(false);

        foreach (Transform t in scenario1.GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in scenario2.GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in scenario3.GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(false);
        }
    }
}
