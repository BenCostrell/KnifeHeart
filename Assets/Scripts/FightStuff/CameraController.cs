using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float baseSize;
    public float movementSpeed;
    public float minSizeFactor;
    public float maxSizeFactor;
    public float playerDistToCameraSizeRatio;
    public float sizeAdjustSpeed;
    public Vector2 bottomLeftCorner_MinCameraPos;
    public Vector2 topRightCorner_MaxCameraPos;
    [HideInInspector]
    public bool shaking;
    public float screenShakeMagFactor;
    public float screenShakeSpeedFactor;
    public float screenShakeDurFactor;

	void Awake()
    {
    }
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Services.FightScene.fightActive && !shaking)
        {
            FollowPlayers();
        }
	}

    void FollowPlayers()
    {
        Vector3 player1Pos = Services.FightScene.players[0].transform.position;
        Vector3 player2Pos = Services.FightScene.players[1].transform.position;
        Vector3 playerMidpoint = (player1Pos + player2Pos) / 2;
        float playerDistance = Vector3.Distance(player1Pos, player2Pos);
        float targetXPos = Mathf.Clamp(playerMidpoint.x, bottomLeftCorner_MinCameraPos.x, topRightCorner_MaxCameraPos.x);
        float targetYPos = Mathf.Clamp(playerMidpoint.y, bottomLeftCorner_MinCameraPos.y, topRightCorner_MaxCameraPos.y);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetXPos, targetYPos, transform.position.z), 
            movementSpeed * Time.deltaTime);
        float sizeFromPlayerDist = playerDistance * playerDistToCameraSizeRatio;
        float targetSize = Mathf.Clamp(sizeFromPlayerDist, minSizeFactor * baseSize, maxSizeFactor * baseSize);
        float currentSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize += (targetSize - currentSize) * sizeAdjustSpeed;
    }

    public void ResetCamera()
    {
        Services.FightScene.fightActive = false;
        transform.position = new Vector3(0, 0, transform.position.z);
        Camera.main.orthographicSize = baseSize;
    }

    public void ResumeCameraFollow()
    {
        Services.FightScene.fightActive = true;
    }

    public void ShakeScreen(float intensity)
    {
        Services.TaskManager.AddTask(new ScreenShake(
            screenShakeSpeedFactor * intensity, 
            screenShakeMagFactor * intensity, 
            screenShakeDurFactor * intensity));
        Debug.Log("intensity level: " + intensity + "\n" +
            "     speed: " + screenShakeSpeedFactor * intensity + "\n" +
            "     magnitude: " + screenShakeMagFactor * intensity + "\n" +
            "     duration: " + screenShakeDurFactor * intensity);
    }
}
