using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour {
    private Player parentPlayer;

    void Start()
    {
        parentPlayer = transform.parent.gameObject.GetComponent<Player>();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Arena")
        {
            parentPlayer.Fall();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "DeathZone")
        {
            parentPlayer.Fall();
        }
    }
}
