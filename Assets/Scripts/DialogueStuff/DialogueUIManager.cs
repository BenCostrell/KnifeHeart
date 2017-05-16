using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour {

    public GameObject background;
    public GameObject crowdImage;
	public GameObject dialogueContainer;
	public GameObject dialogueText;
	public GameObject dialogueTextBox;
    public Sprite[] backgroundImages;
    public Sprite[] dialogueTextBoxImages;
	public GameObject[] optionObjects;
    public GameObject[] rpsOptionObjects;
    public GameObject[] rpsArrows;
    public GameObject[] optionArrows;
    public GameObject rpsTimer;
    public GameObject rpsTimerBackground;
    public GameObject rpsReady_P1;
    public GameObject rpsReady_P2;
    public GameObject[] rpsWords;
	public GameObject continueIndicator;
	public GameObject ponytail;
	public GameObject pigtails;

    public Sprite fireballSymbol;
    public Sprite shieldSymbol;
    public Sprite pullSymbol;
    public Sprite wallopSymbol;
    public Sprite singSymbol;
    public Sprite lungeSymbol;
    public Sprite blinkSymbol;
    [HideInInspector]
    public Dictionary<Ability.Type, Sprite> spriteDict;
    [HideInInspector]
    public Dictionary<Ability.Type, string> poseTriggerDict;

    [HideInInspector]
    public Vector2 defaultPosPonytail;
    [HideInInspector]
    public Vector2 defaultPosPigtails;

    [HideInInspector]
	public GameObject selectedOption;

	private Dialogue[] optionDialogues;

    [HideInInspector]
	public Dialogue queuedDialogue;

    [HideInInspector]
    public bool inRpsStage;

    public float dialogueTextBoxPopUpTime;
	public float optionAppearanceStaggerTime;
	public float optionAppearanceTime;
	public float indicatorFlashUptime;
	public float unselectedOptionShrinkTime;
	public float selectedOptionHighlightTime;
	public float crowdSlideTime;
    public float rpsWordScaleInTime;
    public float rpsWordScaleOutTime;
    public float rpsWordStaggerTime;
    public float rpsWaitTime;
    public float rpsDialogueDelay;
    public float symbolGrowthTime;
    public float symbolAbsorbTime;
    public float symbolTargetScale;
    public float dialogueRotationTime;
    public float optionWheelRadius;
    public float backgroundOptionFadeOutAlpha;
    public float optionArrowBounceTime;
    public float optionArrowBounceDistance;
    public float optObjNoiseMag;
    public float optObjNoiseSpd;
    public float optObjWheelBaseRotation;

	// Use this for initialization
	void Start () {
		
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Init()
    {
        Services.EventManager.Register<DialoguePicked>(QueueDialogue);
        defaultPosPonytail = ponytail.GetComponent<RectTransform>().anchoredPosition;
        defaultPosPigtails = pigtails.GetComponent<RectTransform>().anchoredPosition;
        InitializeDictionaries();
    }

	public void SetUpUI(){
		dialogueText.GetComponent<Text> ().text = "";
		optionDialogues = new Dialogue[4];
		continueIndicator.SetActive (false);
		crowdImage.SetActive (false);
		SetOptionUIStatus (false);
        SetRpsOptionUIStatus(false);
        rpsTimer.SetActive(false);
        rpsTimerBackground.SetActive(false);
        rpsReady_P1.SetActive(false);
        rpsReady_P2.SetActive(false);
        ponytail.GetComponent<RectTransform>().anchoredPosition = defaultPosPonytail;
        pigtails.GetComponent<RectTransform>().anchoredPosition = defaultPosPigtails;
        background.GetComponent<Image>().sprite = backgroundImages[Services.VisualNovelScene.currentRoundNum - 1];
        foreach (GameObject word in rpsWords) word.SetActive(false);
	}

	public void SetOptionUIStatus(bool active){
		for(int i = 0; i < optionObjects.Length; i++) {
			if (optionDialogues [i] != null) {
				optionObjects [i].SetActive (active);
			} else {
				optionObjects [i].SetActive (false);
			}
		}
        for (int i = 0; i < optionArrows.Length; i++)
        {
            optionArrows[i].SetActive(active);
        }
	}

    public void SetRpsOptionUIStatus(bool active)
    {
        for (int i = 0; i < rpsOptionObjects.Length; i++)
        {
            rpsOptionObjects[i].SetActive(active);
            rpsArrows[i].SetActive(active);
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

    public Dialogue GetDialogueFromSelectedOption()
    {
        for (int i = 0; i < optionObjects.Length; i++)
        {
            if (optionObjects[i] == selectedOption)
            {
                return optionDialogues[i];
            }

        }

        return null;
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
	}

    void InitializeDictionaries()
    {
        spriteDict = new Dictionary<Ability.Type, Sprite>()
        {
            { Ability.Type.Fireball, fireballSymbol },
            { Ability.Type.Lunge, lungeSymbol },
            { Ability.Type.Shield, shieldSymbol },
            { Ability.Type.Sing, singSymbol },
            { Ability.Type.Wallop, wallopSymbol },
            { Ability.Type.Pull, pullSymbol },
            { Ability.Type.Blink, blinkSymbol }
    };

        poseTriggerDict = new Dictionary<Ability.Type, string>()
        {
            { Ability.Type.Fireball, "fireballPicked" },
            { Ability.Type.Lunge, "lungePicked" },
            { Ability.Type.Shield, "shieldPicked" },
            { Ability.Type.Sing, "singPicked" },
            { Ability.Type.Wallop, "wallopPicked" },
            { Ability.Type.Pull, "pullPicked" },
            { Ability.Type.Blink, "blinkPicked" }
        };
    }

    public GameObject CreateAbilitySymbol(Ability.Type ability)
    {
        GameObject symbol = Instantiate(Services.PrefabDB.GenericImage, Services.VisualNovelScene.canvas.transform) as GameObject;
        symbol.GetComponent<Image>().sprite = spriteDict[ability];
        symbol.GetComponent<Image>().color = Color.black;
        return symbol;
    }

    public void DestroySymbol(GameObject obj)
    {
        Destroy(obj);
    }

    public void SetPose()
    {
        GameObject characterToPose;
        List<Ability.Type> abilityList = Services.VisualNovelScene.abilityLists[Services.VisualNovelScene.currentTurnPlayerNum - 1];
        Ability.Type abilityPicked = abilityList[abilityList.Count - 1];
        if (Services.VisualNovelScene.currentTurnPlayerNum == 1)
        {
            characterToPose = ponytail;
        }
        else
        {
            characterToPose = pigtails;
        }
        string animTrigger = poseTriggerDict[abilityPicked];

        characterToPose.GetComponent<Animator>().SetTrigger(animTrigger);
    }

    public void InRpsStage()
    {
        inRpsStage = true;
    }

    public void NotInRpsStage()
    {
        inRpsStage = false;
    }
}

