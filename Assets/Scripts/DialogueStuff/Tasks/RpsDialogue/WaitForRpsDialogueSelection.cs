using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitForRpsDialogueSelection : Task {

    private float timeElapsed;
    private float duration;
    private string choice_P1;
    private string choice_P2;
    private float choiceTime_P1;
    private float choiceTime_P2;
    private Image timer;

    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(OnInputReceived);
        duration = Services.DialogueUIManager.rpsWaitTime;
        timeElapsed = 0;
        choice_P1 = "";
        choice_P2 = "";
        timer = Services.DialogueUIManager.rpsTimer.GetComponent<Image>();
        timer.gameObject.SetActive(true);
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        timer.fillAmount = Mathf.Lerp(1, 0, timeElapsed / duration);

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    private void OnInputReceived(ButtonPressed e)
    {
        string choice = "";
        switch (e.buttonTitle)
        {
            case "Y":
                choice = "BE AGGRESSIVE";
                break;
            case "X":
                choice = "BE NICE";
                break;
            case "B":
                choice = "BE PASSIVE AGGRESSIVE";
                break;
            default:
                break;
        }
        if (e.playerNum == 1)
        {
            choice_P1 = choice;
            if (choice != "")
            {
                Services.MusicManager.GenerateSourceAndPlay(Services.MusicManager.selectSound);
                choiceTime_P1 = timeElapsed;
                Services.DialogueUIManager.rpsReady_P1.SetActive(true);
            }
        }
        else if (e.playerNum == 2)
        {
            choice_P2 = choice;
            if (choice != "")
            {
                Services.MusicManager.GenerateSourceAndPlay(Services.MusicManager.selectSound);
                choiceTime_P2 = timeElapsed;
                Services.DialogueUIManager.rpsReady_P2.SetActive(true);
            }
        }
        if ((choice_P1 != "") && (choice_P2 != ""))
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.EventManager.Unregister<ButtonPressed>(OnInputReceived);
        Services.VisualNovelScene.ProcessRpsChoices(choice_P1, choice_P2, choiceTime_P1, choiceTime_P2);
    }
}
