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
    public float musicVolume;

    private List<AudioSource> currentActiveSources;
    public AudioClip scrollSound;
    public AudioClip selectSound;
    public AudioClip readyP1;
    public AudioClip readyP2;
    public AudioClip rpsReadySoundP1;
    public AudioClip rpsReadySoundP2;
    public AudioClip vnSelect;
    public AudioClip fightinWordsSwirl;


    // Use this for initialization
    public void Init () {
        currentActiveSources = new List<AudioSource>();
        titleSource = InitializeAudio(titleTrack);
        baseSource = InitializeAudio(baseTrack);
        fightSources = new AudioSource[fightTracks.Length];
        for (int i = 0; i < fightTracks.Length; i++)
        {
            fightSources[i] = InitializeAudio(fightTracks[i]);
        }
        Services.EventManager.Register<SceneTransition>(OnSceneTransition);
        Services.EventManager.Register<FightAdvance>(OnFightAdvance);
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
                StartPlayingTrack(titleSource);
                break;
            case "VisualNovelScene":
                StartPlayingTrack(baseSource);
                break;
            case "FightScene":
                StartPlayingTrack(fightSources[Services.FightScene.roundNum - 1]);
                //if(Services.FightScene.roundNum == 3)
                //{
                //    StartRooftop();
                //}
                break;
            default:
                break;
        }
    }

    void OnFightAdvance(FightAdvance e)
    {
        //FadeInTrack(fightSources[e.roundNum - 1]);
        StartPlayingTrack(fightSources[e.roundNum - 1]);
        //if(fightSources[e.roundNum = 4])
        //{
        //    StartParkingLot();
        //}
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
        PlayTrack playTrack = new PlayTrack(source);
        FadeInAudio fadeIn = new FadeInAudio(source, fadeInTime, musicVolume);
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
        Services.TaskManager.AddTask(new FadeInAudio(source, fadeInTime, musicVolume));
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

    public void GenerateSourceAndPlay(AudioClip clip)
    {
        GameObject specialAudioSource = Instantiate(Services.PrefabDB.GenericAudioSource);
        AudioSource source = specialAudioSource.GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(specialAudioSource, clip.length);
    }

    //public void StartRooftop()
    //{
    //    this.GetComponent<StudioEventEmitter>().enabled = true;   
    //}

    //public void StartParkingLot()
    //{
    //    FMOD.Studio.EventInstance parkingLot = FMODUnity.RuntimeManager.CreateInstance("Final Battle");
    //    parkingLot.setParameterValue("Level 2", 1.0f);
    //    parkingLot.start();
    //}

}
