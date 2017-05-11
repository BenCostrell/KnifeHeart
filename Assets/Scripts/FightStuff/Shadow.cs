using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

    private Player parentPlayer;
    private SpriteRenderer playerSr;
    private SpriteRenderer sr;
    private Material mat;
    public float skewFactor;
    public float xOffsetFactor;
    public float yOffsetFactor;
    public float yStretchFactor;
    private float baseYStretch;
    private float baseYOffset;

	// Use this for initialization
	void Start () {
        parentPlayer = GetComponentInParent<Player>();
        sr = GetComponent<SpriteRenderer>();
        playerSr = parentPlayer.GetComponent<SpriteRenderer>();
        mat = sr.material;
        baseYStretch = transform.localScale.y;
        baseYOffset = transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
        sr.sprite = playerSr.sprite;
        sr.flipX = playerSr.flipX;
        float skewAmount = parentPlayer.transform.position.x * skewFactor;
        mat.SetFloat("_HorizontalSkew", skewAmount);
        transform.localPosition = new Vector3(
            skewAmount * xOffsetFactor,
            baseYOffset * (1 + (Mathf.Abs(skewAmount) * yOffsetFactor)),
            transform.localPosition.z);
        transform.localScale = new Vector3(
            transform.localScale.x,
            baseYStretch * (1 + (Mathf.Abs(skewAmount) * yStretchFactor)),
            transform.localScale.z);
	}
}
