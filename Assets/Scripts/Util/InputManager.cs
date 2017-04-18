using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {

    private int prevAxisStatus_P1;
    private int prevAxisStatus_P2;

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

        int axisStatus_P1;

        if (Mathf.Abs(Input.GetAxis("Vertical_P1")) > 0.5f)
        {
            if (Input.GetAxis("Vertical_P1") > 0)
            {
                axisStatus_P1 = 1;
            }
            else
            {
                axisStatus_P1 = -1;
            }

            if (axisStatus_P1 != prevAxisStatus_P1)
            {

                Services.EventManager.Fire(new AxisPressed(axisStatus_P1, 1));
            }
        }
        else
        {
            axisStatus_P1 = 0;
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

        int axisStatus_P2;

        if (Mathf.Abs(Input.GetAxis("Vertical_P2")) > 0.5f)
        {
            if (Input.GetAxis("Vertical_P2") > 0)
            {
                axisStatus_P2 = 1;
            }
            else
            {
                axisStatus_P2 = -1;
            }

            if (axisStatus_P2 != prevAxisStatus_P2)
            {

                Services.EventManager.Fire(new AxisPressed(axisStatus_P2, 2));
            }
        }
        else
        {
            axisStatus_P2 = 0;
        }

        if (Input.GetButtonDown ("Reset")) {
			Services.EventManager.Fire (new Reset ());
		}

        prevAxisStatus_P1 = axisStatus_P1;
        prevAxisStatus_P2 = axisStatus_P2;
	}
}
