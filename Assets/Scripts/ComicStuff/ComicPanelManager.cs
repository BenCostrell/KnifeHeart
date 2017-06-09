using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicPanelManager : MonoBehaviour {

    public GameObject comicBackground;
    public GameObject[] scenarios;
    public GameObject continueButton;
    public float panelAppearTime;
    public GameObject calendarCircle;
    public Sprite[] calendarAnimationSprites;
    public float calendarAnimationSpeed;
    public Vector2[] calendarCircleLocations;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TurnOffComicPanels()
    {
        comicBackground.SetActive(false);
        continueButton.SetActive(false);
        calendarCircle.SetActive(false);

        for (int i = 0; i < scenarios.Length; i++)
        {
            foreach (Transform t in scenarios[i].GetComponentsInChildren<Transform>())
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    public void PositionAndActivateCalendarCircle()
    {
        calendarCircle.SetActive(true);
        calendarCircle.GetComponent<RectTransform>().anchoredPosition = 
            calendarCircleLocations[Services.VisualNovelScene.currentRoundNum - 1];
    }
}
