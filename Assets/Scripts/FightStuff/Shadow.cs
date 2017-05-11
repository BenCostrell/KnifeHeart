using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

    private Player parentPlayer;
    private SpriteRenderer playerSr;
    private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        parentPlayer = GetComponentInParent<Player>();
        sr = GetComponent<SpriteRenderer>();
        playerSr = parentPlayer.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        sr.sprite = playerSr.sprite;
        sr.flipX = playerSr.flipX;
	}
}
