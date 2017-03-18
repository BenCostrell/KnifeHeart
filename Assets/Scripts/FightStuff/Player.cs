using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public int playerNum;
	public List<Ability.Type> abilityList;

	private Rigidbody2D rb;
	public Animator anim;
	private SpriteRenderer sr;

	public float maxSpeed;
	public float accel;
	public float hitstunFactor;
	public float knockbackDamageGrowthFactor;

	public int damage;
	public bool actionable;
	private float timeUntilActionable;
	public bool isInvulnerable;
	private bool inHitstun;
	private bool actionInProcess;

	private float basicAttackCooldownCounter;
	private float ability1BaseCooldown;
	private float ability2BaseCooldown;
	private float ability1CooldownCounter;
	private float ability2CooldownCounter;
	private bool ability1OnCooldown;
	private bool ability2OnCooldown;

    AudioSource audioSource;
    AudioClip audioClip;

    public AudioClip player1Died;
    public AudioClip player2Died;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
		timeUntilActionable = 0;
		ability1CooldownCounter = 0;
		ability2CooldownCounter = 0;
		inHitstun = false;
		actionInProcess = false;

        audioSource = Camera.main.GetComponent<AudioSource>();
        audioClip = Camera.main.GetComponent<AudioClip>();

		ability1BaseCooldown = 1;
		ability2BaseCooldown = 1;

		StartListeningForInput ();
	}
	
	// Update is called once per frame
	void Update () {
		//ProcessAbilityCooldowns ();
		if (timeUntilActionable > 0) {
			timeUntilActionable -= Time.deltaTime;
		} else {
			if (actionInProcess) {
				ResetToNeutral ();
			}
			if (actionable) {
				Move ();
			}
			//DetectActionInput ();
		}
	}

	void Move(){
		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal_P" + playerNum), Input.GetAxis ("Vertical_P" + playerNum));

		rb.AddForce (accel * direction);

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = maxSpeed * direction.normalized;
		}

		if (direction.x > 0.01f) {
			sr.flipX = true;
		} else if (direction.x < -0.01f) {
			sr.flipX = false;
		}

		//Debug.Log (rb.velocity);
	}

	public void StartListeningForInput(){
		Services.EventManager.Register<ButtonPressed> (AbilityActivated);
		actionable = true;
		Debug.Log ("listening");
	}

	public void StopListeningForInput(){
		Services.EventManager.Unregister<ButtonPressed> (AbilityActivated);
		actionable = false;
		Debug.Log ("stopped listening");
	}

	void ProcessAbilityCooldowns(){
		if (abilityList != null) {
			if (ability1CooldownCounter > 0) {
				ability1CooldownCounter -= Time.deltaTime;
			} else {
				ability1OnCooldown = false;
			}

			Services.FightUIManager.UpdateCooldownBar (playerNum, 1, ability1CooldownCounter / ability1BaseCooldown);

			if (ability2CooldownCounter > 0) {
				ability2CooldownCounter -= Time.deltaTime;
			} else {
				ability2OnCooldown = false;
			}

			Services.FightUIManager.UpdateCooldownBar (playerNum, 2, ability2CooldownCounter / ability2BaseCooldown);

		}
	}

	void DetectActionInput(){
		if (basicAttackCooldownCounter > 0) {
			basicAttackCooldownCounter -= Time.deltaTime;
		}
		else if (Input.GetButtonDown("BasicAttack_P" + playerNum)){
			//basicAttackCooldownCounter = DoAbility (Ability.Type.BasicAttack);
		}

		if (Input.GetButtonDown ("Ability1_P" + playerNum) && !ability1OnCooldown) {
			//ability1CooldownCounter = DoAbility (abilityList [0]);
			ability1OnCooldown = true;
			ability1BaseCooldown = ability1CooldownCounter;
		} else if (Input.GetButtonDown ("Ability2_P" + playerNum) && !ability2OnCooldown) {
			//ability2CooldownCounter = DoAbility (abilityList [1]);
			ability2OnCooldown = true;
			ability2BaseCooldown = ability2CooldownCounter;
		}

	}

	void OnTriggerEnter2D(Collider2D other) 
	{ 

	} 

	void OnTriggerExit2D(Collider2D collider){
		if (collider.tag == "Arena"){
			Die ();
		}
	}

	void ResetToNeutral(){
		actionInProcess = false;
		anim.SetTrigger ("ResetToNeutral");
		isInvulnerable = false;
	}

	void AbilityActivated(ButtonPressed e){
		if (e.playerNum == playerNum) {
			switch (e.buttonTitle){
			case "A":	
				DoAbility (Ability.Type.BasicAttack);
				break;
			case "X":
				DoAbility (abilityList [0]);
				break;
			case "Y":
				DoAbility (abilityList [1]);
				break;
			case "B":
				DoAbility (abilityList [2]);
				break;
			default:
				break;
			}
		}
	}

	void DoAbility(Ability.Type type){
		GameObject abilityObj = Instantiate (Services.PrefabDB.GetPrefabFromAbilityType (type), transform.position, Quaternion.identity) as GameObject;
		abilityObj.GetComponent<Ability> ().Init (gameObject);
	}


	void Die(){
		Destroy (gameObject);
        if (gameObject == Services.FightSceneManager.player1)
        {
            Debug.Log("player 1 died");
            audioSource.clip = player1Died;
            audioSource.Play();
        }
        else if (gameObject == Services.FightSceneManager.player2)
        {
            Debug.Log("player 2 died");
            audioSource.clip = player2Died;
            audioSource.Play();
        }

    }

	public void TakeHit(int damageTaken, float baseKnockback, float knockbackGrowth, Vector3 knockbackVector){
		damage += damageTaken;
		float knockbackMagnitude = baseKnockback + (knockbackGrowth * damage * knockbackDamageGrowthFactor);
		Stun(knockbackMagnitude * hitstunFactor);
		rb.velocity = knockbackMagnitude * knockbackVector;
		Services.FightUIManager.UpdateDamageUI (gameObject);
	}

	public void Stun(float hitstun){
		rb.velocity = Vector3.zero;
		HitstunTask startHitstun = new HitstunTask (hitstun, this);
		Services.TaskManager.AddTask (startHitstun);
	}

	public void InitiateAction(float actionDuration){
		timeUntilActionable = actionDuration;
		rb.velocity = Vector3.zero;
		actionInProcess = true;
	}
}
