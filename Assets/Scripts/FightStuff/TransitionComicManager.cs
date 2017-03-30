using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionComicManager : MonoBehaviour {

    public GameObject comicBackground;
    public GameObject transitionToParkingLot;
    public GameObject transitionToHell;
    public GameObject continueButton;
    public Sprite[] transitionToParkingLotImagesPigtailsWins;
    public Sprite[] transitionToParkingLotImagesPonytailWins;
    public Sprite[] transitionToHellImagesPigtailsWins;
    public Sprite[] transitionToHellImagesPonytailWins;

    public float panelAppearTime;
    public float continuePromptGrowTime;
    public float continuePromptShrinkTime;

    // Use this for initialization
    void Start()
    {
        TurnOffComicPanels();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TurnOffComicPanels()
    {
        comicBackground.SetActive(false);
        continueButton.SetActive(false);

        foreach (Transform t in transitionToParkingLot.GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in transitionToHell.GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(false);
        }
    }
}
