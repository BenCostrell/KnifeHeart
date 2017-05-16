using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScaleOutRpsWords : Task
{
    private float timeElapsed;
    private float duration;
    private GameObject rpsWordContainer;
    private Vector3 initialScale;

    public ScaleOutRpsWords(GameObject container, float dur)
    {
        rpsWordContainer = container;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        initialScale = rpsWordContainer.transform.localScale;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        rpsWordContainer.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, 
            Easing.QuadEaseIn(timeElapsed / duration));
        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
        rpsWordContainer.transform.localScale = initialScale;
        rpsWordContainer.gameObject.SetActive(false);
    }
}
