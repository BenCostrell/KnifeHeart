using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {
	public void GetInput(){
		if (Input.GetButtonDown ("A_P1")) {
			Services.EventManager.Fire (new ButtonPressed ("A", 1));
		} else if (Input.GetButtonDown ("B_P1")) {
			Services.EventManager.Fire (new ButtonPressed ("B", 1));
		} else if (Input.GetButtonDown ("X_P1")) {
			Services.EventManager.Fire (new ButtonPressed ("X", 1));
		} else if (Input.GetButtonDown ("Y_P1")) {
			Services.EventManager.Fire (new ButtonPressed ("Y", 1));
		}
		if (Input.GetButtonDown ("A_P2")) {
			Services.EventManager.Fire (new ButtonPressed ("A", 2));
		} else if (Input.GetButtonDown ("B_P2")) {
			Services.EventManager.Fire (new ButtonPressed ("B", 2));
		} else if (Input.GetButtonDown ("X_P2")) {
			Services.EventManager.Fire (new ButtonPressed ("X", 2));
		} else if (Input.GetButtonDown ("Y_P2")) {
			Services.EventManager.Fire (new ButtonPressed ("Y", 2));
		}
		if (Input.GetButtonDown ("Reset")) {
			Services.EventManager.Fire (new Reset ());
		}
	}
}
