using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicPanelManager : MonoBehaviour {

    public GameObject comicBackground;
    public GameObject[] scenarios;
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

        for (int i = 0; i < scenarios.Length; i++)
        {
            foreach (Transform t in scenarios[i].GetComponentsInChildren<Transform>())
            {
                t.gameObject.SetActive(false);
            }
        }
    }
}
