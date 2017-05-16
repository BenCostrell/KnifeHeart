using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : Scene<TransitionData> {

    public GameObject[] characters;
    public Sprite[] glowingSprites;
    public Sprite pinkStartPrompt;
    public GameObject startPrompt;
    public GameObject title;
    public float initialDelay;
    public float delayBeforeStarting;
    public float playerBounceScale;
    public float playerBounceTime;
    public float startPromptPeriod;
    public float startPromptBounceScale;
    public float startPromptMinAlpha;
    public float titleBounceNoiseSpd;
    public float titleBounceNoiseMag;
    public float slideInDuration;
    public float titleScaleInTime;
    public float delayBeforeStartPrompt;

	
    // Use this for initialization
	void Start () {
        Services.TitleScreen = this;
        characters[0].transform.position = characters[0].transform.position + 500 * Vector3.left;
        characters[1].transform.position = characters[1].transform.position + 500 * Vector3.right;
        startPrompt.SetActive(false);
        title.SetActive(false);

        TaskQueue titleScreenSequence = new TaskQueue(new List<Task>
        {
            new WaitForTime(initialDelay),
            new SlideCharacter(characters[0],
            characters[0].transform.position,
            characters[0].transform.position + 500 * Vector3.right, slideInDuration, Easing.FunctionType.BackEaseOut),
            new SlideCharacter(characters[1],
            characters[1].transform.position,
            characters[1].transform.position + 500 * Vector3.left, slideInDuration, Easing.FunctionType.BackEaseOut),
            new ScaleTitle(title, Vector3.zero, title.transform.localScale, titleScaleInTime, Easing.FunctionType.BackEaseOut),
            new WaitForTime(delayBeforeStartPrompt),
            new SetObjectStatus(true, startPrompt),
            new WaitToStartGame(),
            new ActionTask(SetStartPromptSprite),
            new WaitForTime(delayBeforeStarting),
            new SetObjectStatus(false, startPrompt),
            new ActionTask(SlideOutCharacters),
            new WaitForTime(slideInDuration),
            new ScaleTitle(title, title.transform.localScale, Vector3.zero, titleScaleInTime, Easing.FunctionType.BackEaseIn),
            new ActionTask(StartGame)
        });

        Services.TaskManager.AddTaskQueue(titleScreenSequence);
	}
	
    void SlideOutCharacters()
    {
        Services.TaskManager.AddTask(new SlideCharacter(characters[0],
            characters[0].transform.position,
            characters[0].transform.position + 600 * Vector3.left, slideInDuration, Easing.FunctionType.BackEaseIn));
        Services.TaskManager.AddTask(new SlideCharacter(characters[1],
            characters[1].transform.position,
            characters[1].transform.position + 600 * Vector3.right, slideInDuration, Easing.FunctionType.BackEaseIn));
    }

    void SetStartPromptSprite()
    {
        startPrompt.GetComponent<Image>().sprite = pinkStartPrompt;
    }

	// Update is called once per frame
	void Update () {
		
	}

    internal override void OnEnter(TransitionData data)
    {
        Services.EventManager.Fire(new SceneTransition("TitleScreen"));
    }

    void StartGame()
    {
        Services.SceneStackManager.Swap<VisualNovelScene>();
    }

    
}
