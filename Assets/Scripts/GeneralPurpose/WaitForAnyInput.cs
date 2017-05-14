using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForAnyInput : Task {

	protected override void Init ()
	{
		Services.EventManager.Register<ButtonPressed> (Continue);
	}

	protected virtual void Continue (ButtonPressed e){
        Services.MusicManager.GenerateSourceAndPlay(Services.MusicManager.vnSelect);
        SetStatus (TaskStatus.Success);
	}

	protected override void OnSuccess ()
	{
		Services.EventManager.Unregister<ButtonPressed> (Continue);
	}
}
