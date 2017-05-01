using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip titleTrack;
    public AudioClip baseTrack;
    public AudioClip[] fightTracks;

    private AudioSource titleSource;
    private AudioSource baseSource;
    private AudioSource[] fightSources;

    public float fadeInTime;
    public float fadeOutTime;

    private List<AudioSource> currentActiveSources;

	// Use this for initialization
	void Awake () {
        Services.EventManager.Register<SceneTransition>(OnSceneTransition);
        Services.EventManager.Register<FightAdvance>(OnFightAdvance);
	}
	
    void Start()
    {
        currentActiveSources = new List<AudioSource>();
        titleSource = InitializeAudio(titleTrack);
        baseSource = InitializeAudio(baseTrack);
        fightSources = new AudioSource[fightTracks.Length];
        for (int i = 0; i < fightTracks.Length; i++)
        {
            fightSources[i] = InitializeAudio(fightTracks[i]);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}

    void OnSceneTransition(SceneTransition e)
    {
        switch (e.sceneName)
        {
            case "TitleScreen":
                StartPlayingTrack(titleSource);
                break;
            case "VisualNovelScene":
                StartPlayingTrack(baseSource);
                break;
            case "FightScene":
                if (Services.FightScene.roundNum != 3) StartPlayingTrack(fightSources[Services.FightScene.roundNum - 1]);
                else {
                    StartFinalFightTracks();
                    FadeInTrack(fightSources[2]);
                }
                break;
            default:
                break;
        }
    }

    void OnFightAdvance(FightAdvance e)
    {
        FadeInTrack(fightSources[e.roundNum - 1]);
    }

    AudioSource InitializeAudio(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = false;
        source.loop = true;
        return source;
    }

    void StartPlayingTrack(AudioSource source)
    {
        ActionTask playTrack = new ActionTask(source.Play);
        FadeInAudio fadeIn = new FadeInAudio(source, fadeInTime);
        playTrack.Then(fadeIn);

        if (currentActiveSources.Count > 0)
        {
            FadeOutAudio fadeOut = null;
            for (int i = currentActiveSources.Count - 1; i >= 0; i--)
            {
                fadeOut = new FadeOutAudio(currentActiveSources[i], fadeOutTime);
                Services.TaskManager.AddTask(fadeOut);
                currentActiveSources.Remove(currentActiveSources[i]);
            }
            fadeOut.Then(playTrack);
        }
        else
        {
            Services.TaskManager.AddTask(playTrack);
        }
        currentActiveSources.Add(source);
    }

    void FadeInTrack(AudioSource source)
    {
        Services.TaskManager.AddTask(new FadeInAudio(source, fadeInTime));
    }

    void StartFinalFightTracks()
    {
        for (int i = 2; i < 4; i++)
        {
            fightSources[i].volume = 0;
            fightSources[i].Play();
            currentActiveSources.Add(fightSources[i]);
        }
    }
}
