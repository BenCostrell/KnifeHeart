using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionComicManager : MonoBehaviour {

    public GameObject comicBackground;
    public GameObject transitionToParkingLot;
    public GameObject transitionToHell;
    public GameObject winComic;
    public GameObject continueButton;
    public Sprite[] transitionToParkingLotImagesPigtailsWins;
    public Sprite[] transitionToParkingLotImagesPonytailWins;
    public Sprite[] transitionToHellImagesPigtailsWins;
    public Sprite[] transitionToHellImagesPonytailWins;
    public List<Sprite> winComicPigtailsWinsPage1;
    public List<Sprite> winComicPigtailsWinsPage2;
    public List<Sprite> winComicPonytailWinsPage1;
    public List<Sprite> winComicPonytailWinsPage2;

    public List<List<Sprite>> winComicPigtailsWins;
    public List<List<Sprite>> winComicPonytailWins;

    public float panelAppearTime;
    public float continuePromptGrowTime;
    public float continuePromptShrinkTime;

    // Use this for initialization
    void Start()
    {
        winComicPigtailsWins = new List<List<Sprite>>();
        winComicPigtailsWins.Add(winComicPigtailsWinsPage1);
        winComicPigtailsWins.Add(winComicPigtailsWinsPage2);
        winComicPonytailWins = new List<List<Sprite>>();
        winComicPonytailWins.Add(winComicPonytailWinsPage1);
        winComicPonytailWins.Add(winComicPonytailWinsPage2);
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
        foreach (Transform t in winComic.GetComponentsInChildren<Transform>())
        {
            t.gameObject.SetActive(false);
        }
    }
}
