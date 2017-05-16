using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WaitToStartGame : Task
{
    private bool[] playersReady;
    private float timeElapsed;
    private Vector3 initialScale;
    private Image startPromptImage;
    private Vector3 titleBasePosition;

    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(OnButtonPressed);
        playersReady = new bool[2] { false, false };
        timeElapsed = 0;
        initialScale = Services.TitleScreen.startPrompt.transform.localScale;
        startPromptImage = Services.TitleScreen.startPrompt.GetComponent<Image>();
        titleBasePosition = Services.TitleScreen.title.transform.position;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        //Services.TitleScreen.title.transform.position = titleBasePosition + new Vector3(
        //     Services.TitleScreen.titleBounceNoiseMag / 2 +
        //     Services.TitleScreen.titleBounceNoiseMag *
        //     Mathf.PerlinNoise(Services.TitleScreen.titleBounceNoiseSpd * Time.time, 0),
        //     Services.TitleScreen.titleBounceNoiseMag / 2 +
        //     Services.TitleScreen.titleBounceNoiseMag *
        //     Mathf.PerlinNoise(Services.TitleScreen.titleBounceNoiseSpd * Time.time, 1000));
        if (timeElapsed%Services.TitleScreen.startPromptPeriod <= Services.TitleScreen.startPromptPeriod / 2)
        {
            Services.TitleScreen.startPrompt.transform.localScale = Vector3.Lerp(
                initialScale,
                Services.TitleScreen.startPromptBounceScale * initialScale,
                Easing.QuadEaseOut(timeElapsed % Services.TitleScreen.startPromptPeriod / (Services.TitleScreen.startPromptPeriod / 2)));
            float newAlpha = Mathf.Lerp(Services.TitleScreen.startPromptMinAlpha, 1, 
                Easing.QuadEaseOut(timeElapsed % Services.TitleScreen.startPromptPeriod / (Services.TitleScreen.startPromptPeriod / 2)));
            startPromptImage.color = new Color(startPromptImage.color.r, startPromptImage.color.g, startPromptImage.color.b, newAlpha);
        }
        else
        {
            Services.TitleScreen.startPrompt.transform.localScale = Vector3.Lerp(
                Services.TitleScreen.startPromptBounceScale * initialScale,
                initialScale,
                Easing.QuadEaseIn((timeElapsed % Services.TitleScreen.startPromptPeriod - 
                (Services.TitleScreen.startPromptPeriod / 2)) /
                (Services.TitleScreen.startPromptPeriod / 2)));
            float newAlpha = Mathf.Lerp(1, Services.TitleScreen.startPromptMinAlpha,
                Easing.QuadEaseIn((timeElapsed % Services.TitleScreen.startPromptPeriod - 
                (Services.TitleScreen.startPromptPeriod / 2)) / 
                (Services.TitleScreen.startPromptPeriod / 2)));
            startPromptImage.color = new Color(startPromptImage.color.r, startPromptImage.color.g, startPromptImage.color.b, newAlpha);
        }
    }

    void OnButtonPressed(ButtonPressed e)
    {
        bool readyToStart = true;
        int playerIndex = e.playerNum - 1;
        if (!playersReady[playerIndex])
        {
            playersReady[playerIndex] = true;
            Services.TitleScreen.characters[playerIndex].GetComponent<Image>().sprite =
                Services.TitleScreen.glowingSprites[playerIndex];
            Services.TaskManager.AddTask(new HighlightPlayerReady(
                Services.TitleScreen.characters[playerIndex], 
                Services.TitleScreen.playerBounceTime));
        }
        foreach(bool playerReady in playersReady)
        {
            if(!playerReady)
            {
                readyToStart = false;
                break;
            }
        }
        if (readyToStart) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        Services.TitleScreen.startPrompt.transform.localScale = Services.TitleScreen.startPromptBounceScale * initialScale;
        startPromptImage.color = new Color(startPromptImage.color.r, startPromptImage.color.g, startPromptImage.color.b, 1);
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<ButtonPressed>(OnButtonPressed);
    }
}
