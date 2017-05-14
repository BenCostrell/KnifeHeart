using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {

    [HideInInspector]
    public enum Type { None, BasicAttack, Fireball, Lunge, Sing, Shield, Wallop, Pull, Blink };
    [HideInInspector]
	public GameObject parentPlayer;

    public float cooldown;
	public string animTrigger;
	public AudioClip onCastAudio;
    public bool isMelee;
    public AudioClip specialAudio;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void Init(GameObject player){
		parentPlayer = player;
		if (isMelee) {
			transform.parent = player.transform;
			transform.localRotation = Quaternion.Euler (0, 0, player.GetComponent<Player> ().effectiveRotation);
			FixedJoint2D joint = player.AddComponent<FixedJoint2D> ();
			joint.connectedBody = GetComponent<Rigidbody2D> ();
			joint.enableCollision = true;
		}
		player.GetComponent<Player> ().anim.SetTrigger (animTrigger);
		player.GetComponent<Player> ().anim.SetBool ("neutral", false);
		OnCast ();
	}

	protected virtual void OnCast(){
		if (onCastAudio != null) {
            AudioSource source = parentPlayer.GetComponent<Player>().castAudioSource;
            source.clip = onCastAudio;
			source.Play ();
		}
	}

	public virtual void OnCastFinish(){
        //Debug.Log("ending ability at time " + Time.time);
        if (isMelee) {
			Destroy (parentPlayer.GetComponent<FixedJoint2D> ());
		}
		Destroy (gameObject);
	}

    public virtual void SetActive()
    {

    }

    public virtual void SetInactive()
    {

    }

    public virtual void PlaySpecialAudio()
    {
        if (specialAudio != null)
        {
            Services.MusicManager.GenerateSourceAndPlay(specialAudio);
        }
    }
}
