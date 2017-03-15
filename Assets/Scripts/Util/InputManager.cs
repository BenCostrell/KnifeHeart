using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {
	public void GetInput(){
		if (Input.GetButtonDown("A_P1")){
			Services.EventManager.Fire (new A_P1 ());
		}
		if (Input.GetButtonDown("B_P1")){
			Services.EventManager.Fire (new B_P1 ());
		}
		if (Input.GetButtonDown("X_P1")){
			Services.EventManager.Fire (new X_P1 ());
		}
		if (Input.GetButtonDown("Y_P1")){
			Services.EventManager.Fire (new Y_P1 ());
		}
		if (Input.GetButtonDown("A_P2")){
			Services.EventManager.Fire (new A_P2 ());
		}
		if (Input.GetButtonDown("B_P2")){
			Services.EventManager.Fire (new B_P2 ());
		}
		if (Input.GetButtonDown("X_P2")){
			Services.EventManager.Fire (new X_P2 ());
		}
		if (Input.GetButtonDown("Y_P2")){
			Services.EventManager.Fire (new Y_P2 ());
		}
		if (Input.GetButtonDown ("Reset")) {
			Services.EventManager.Fire (new Reset ());
		}
	}
}
