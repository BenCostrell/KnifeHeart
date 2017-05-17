using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitForRpsDialogueSelection : Task {

    private float timeElapsed;
    private float duration;
    private VisualNovelScene.RpsOption choice_P1;
    private VisualNovelScene.RpsOption choice_P2;
    private float choiceTime_P1;
    private float choiceTime_P2;
    private Image timerBackground;
    private Image timer;

    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(OnInputReceived);
        duration = Services.DialogueUIManager.rpsWaitTime;
        timeElapsed = 0;
        choice_P1 = VisualNovelScene.RpsOption.None;
        choice_P2 = VisualNovelScene.RpsOption.None;
        timerBackground = Services.DialogueUIManager.rpsTimerBackground.GetComponent<Image>();
        timer = Services.DialogueUIManager.rpsTimer.GetComponent<Image>();
        timer.gameObject.SetActive(true);
        timerBackground.gameObject.SetActive(true);
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
        VisualNovelScene.RpsOption choice = VisualNovelScene.RpsOption.None;
        switch (e.buttonTitle)
        {
            case "Y":
                choice = VisualNovelScene.RpsOption.Rock;
                break;
            case "X":
                choice = VisualNovelScene.RpsOption.Paper;
                break;
            case "B":
                choice = VisualNovelScene.RpsOption.Scissors;
                break;
            default:
                break;
        }
        if (e.playerNum == 1)
        {
            choice_P1 = choice;
            if (choice != VisualNovelScene.RpsOption.None)
            {
                Services.MusicManager.GenerateSourceAndPlay(Services.MusicManager.rpsReadySoundP1);
                choiceTime_P1 = timeElapsed;
                Services.DialogueUIManager.rpsReady_P1.SetActive(true);
            }
        }
        else if (e.playerNum == 2)
        {
            choice_P2 = choice;
            if (choice != VisualNovelScene.RpsOption.None)
            {
                Services.MusicManager.GenerateSourceAndPlay(Services.MusicManager.rpsReadySoundP2);
                choiceTime_P2 = timeElapsed;
                Services.DialogueUIManager.rpsReady_P2.SetActive(true);
            }
        }
        if ((choice_P1 != VisualNovelScene.RpsOption.None) && (choice_P2 != VisualNovelScene.RpsOption.None))
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
