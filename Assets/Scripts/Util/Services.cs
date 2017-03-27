﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services {
	public static EventManager EventManager { get; set; }
	public static GameManager GameManager { get; set; }
	public static PrefabDB PrefabDB { get; set; }
	public static TaskManager TaskManager { get; set; }
	public static DialogueDataManager DialogueDataManager { get; set; }
	public static DialogueUIManager DialogueUIManager { get; set; }
	public static GameInfo GameInfo { get; set; }
	public static InputManager InputManager { get; set; }
	public static TransitionUIManager TransitionUIManager { get; set; }
	public static VisualNovelSceneManager VisualNovelSceneManager { get; set; }
	public static FightSceneManager FightSceneManager { get; set; }
	public static FightUIManager FightUIManager { get; set; }
	public static WinScreenUIManager WinScreenUIManager { get; set; }
    public static ComicPanelManager ComicPanelManager { get; set; }
}
