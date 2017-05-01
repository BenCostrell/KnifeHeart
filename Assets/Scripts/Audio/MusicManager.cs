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

	// Use this for initialization
	void Awake () {
        Services.EventManager.Register<SceneTransition>(OnSceneTransition);
	}
	
    void Start()
    {
        titleSource = InitializeAudio(titleTrack);
        baseSource = InitializeAudio(baseTrack);
        foreach(AudioClip clip in fightTracks)
        {

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
                break;
            case "VisualNovelScene":
                break;
            case "FightScene":
                break;
            default:
                break;
        }
    }

    AudioSource InitializeAudio(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = false;
        return source;
    }

    void StartPlayingTrack()
    {

    }
}
