using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScreenShake : Task
{
    private float duration;
    private float timeElapsed;
    private Transform cameraTransform;
    private float speed;
    private float magnitude;
    private Vector3 initialPos;

    public ScreenShake(float spd, float mag, float dur)
    {
        speed = spd;
        magnitude = mag;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        cameraTransform = Camera.main.transform;
        Services.CameraController.shaking = true;
        initialPos = cameraTransform.position;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        cameraTransform.position = initialPos + magnitude * new Vector3(
            Mathf.PerlinNoise(Time.time * speed, 0), 
            Mathf.PerlinNoise(Time.time * speed, 1000));
        if (timeElapsed >= duration) SetStatus(TaskStatus.Success);
    }

    protected override void CleanUp()
    {
        Services.CameraController.shaking = false;
    }
}
