using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjectStatus : Task {
    private bool active;
    private GameObject obj;
    public SetObjectStatus (bool newStatus, GameObject objectToSet)
    {
        active = newStatus;
        obj = objectToSet;
    }

    protected override void Init()
    {
        obj.SetActive(active);
        SetStatus(TaskStatus.Success);
    }
}
