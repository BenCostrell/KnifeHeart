using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip titleTrack;
    public AudioClip baseTrack;
    public AudioClip[] fightTracks;

    private AudioSource titleAudioSource;

	// Use this for initialization
	void Awake () {
        Services.EventManager.Register<SceneTransition>(OnSceneTransition);
	}

    void Start()
    {

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

    void StartPlayingTrack(AudioClip track)
    {
    }
}
