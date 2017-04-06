using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicPanel : MonoBehaviour {

    public Sprite ponytailWinPanel;
    public Sprite pigtailsWinPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPanel(int winningPlayerNum)
    {
        if (winningPlayerNum == 1)
        {
            GetComponent<Image>().sprite = ponytailWinPanel;
        }
        else
        {
            GetComponent<Image>().sprite = pigtailsWinPanel;
        }
    }
}
