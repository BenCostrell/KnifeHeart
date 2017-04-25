using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services {
    /// UNIVERSAL UTILITY ///
    public static EventManager EventManager { get; set; }
	public static GameManager GameManager { get; set; }
	public static PrefabDB PrefabDB { get; set; }
	public static TaskManager TaskManager { get; set; }
    public static GameInfo GameInfo { get; set; }
    public static InputManager InputManager { get; set; }
    public static SceneStackManager<TransitionData> SceneStackManager { get; set; }

    /// TITLE SCREEN ///
    public static TitleScreen TitleScreen { get; set; }

    /// VISUAL NOVEL SCENE ///
    public static DialogueDataManager DialogueDataManager { get; set; }
    public static DialogueUIManager DialogueUIManager { get; set; }
    public static TransitionUIManager TransitionUIManager { get; set; }
    public static VisualNovelScene VisualNovelScene { get; set; }
    public static WinScreenUIManager WinScreenUIManager { get; set; }
    public static TransitionComicManager TransitionComicManager { get; set; }

    /// FIGHT SCENE ///
    public static FightScene FightScene { get; set; }
	public static FightUIManager FightUIManager { get; set; }
    public static ComicPanelManager ComicPanelManager { get; set; }
    public static CameraController CameraController { get; set; }
}
