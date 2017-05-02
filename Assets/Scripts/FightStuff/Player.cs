using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public int playerNum;
	public List<Ability.Type> abilityList;

	private Rigidbody2D rb;
	public Animator anim;
	private SpriteRenderer sr;
    [HideInInspector]
    public Collider2D stageEdgeBoundaryCollider;
    public LayerMask stageEdgeBoundaryLayer;

	public float maxSpeed;
	public float accel;
    public float dashSpeed;
	public float hitstunFactor;
	public float knockbackDamageGrowthFactor;

	public int damage;
	public bool actionable;
	public bool isInvulnerable;
	public float effectiveRotation;

    private Ability currentActiveAbility;
    [HideInInspector]
	public List <Ability.Type> abilitiesOnCooldown;
    [HideInInspector]
    public TaskManager taskManager;

    [HideInInspector]
    public AudioSource castAudioSource;
    [HideInInspector]
    public AudioSource impactAudioSource;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
        taskManager = new TaskManager();
        foreach(Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject.tag == "stageEdgeBoundaryCollider")
            {
                stageEdgeBoundaryCollider = col;
                break;
            }
        }

		abilitiesOnCooldown = new List<Ability.Type> ();

		StartListeningForInput ();
		Services.EventManager.Register<GameOver> (OnGameOver);
        Services.EventManager.Register<PlayerFall>(OnPlayerFall);
        castAudioSource = gameObject.AddComponent<AudioSource>();
        impactAudioSource = gameObject.AddComponent<AudioSource>();

		anim.SetBool ("sideFacing", true);
	}
	
	// Update is called once per frame
	void Update () {
        taskManager.Update();
        if (actionable) {
			anim.SetBool ("neutral", true);
			Move ();
		} else {
		}

		anim.SetBool ("Actionable", actionable); 
	}

	void Move(){
		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal_P" + playerNum), Input.GetAxis ("Vertical_P" + playerNum));

        if(Vector2.Dot(direction, rb.velocity) <= 0)
        {
            rb.velocity = dashSpeed * direction.normalized;
        }

		rb.AddForce (accel * direction);

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = maxSpeed * direction.normalized;
		}
		anim.SetFloat ("Velocity", direction.magnitude);

//		if (direction.magnitude > 0.1f)
//			EndAbility ();

		float angleFacing = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		if (direction.magnitude > 0.01f) {
			if ((angleFacing >= -45) && (angleFacing < 45)) {
				sr.flipX = true;
				effectiveRotation = 180;
				anim.SetBool ("sideFacing", true);
				anim.SetBool ("upFacing", false);
				anim.SetBool ("downFacing", false);
			} else if ((angleFacing >= 45) && (angleFacing < 135)) {
				sr.flipX = false;
				effectiveRotation = -90;
				anim.SetBool ("sideFacing", false);
				anim.SetBool ("upFacing", true);
				anim.SetBool ("downFacing", false);
			} else if ((angleFacing >= 135) || ((angleFacing >= -180) && (angleFacing <= -135))) {
				sr.flipX = false;
				effectiveRotation = 0;
				anim.SetBool ("sideFacing", true);
				anim.SetBool ("upFacing", false);
				anim.SetBool ("downFacing", false);
			} else {
				sr.flipX = false;
				effectiveRotation = 90;
				anim.SetBool ("sideFacing", false);
				anim.SetBool ("upFacing", false);
				anim.SetBool ("downFacing", true);
			}
		}
		//Debug.Log (rb.velocity);
	}

	public void StartListeningForInput(){
		Services.EventManager.Register<ButtonPressed> (AbilityActivated);
		actionable = true;
	}

	public void StopListeningForInput(){
		Services.EventManager.Unregister<ButtonPressed> (AbilityActivated);
		actionable = false;
	}

    public void ResetCooldowns()
    {
        foreach (Ability.Type ability in abilitiesOnCooldown)
        {
            Services.FightUIManager.UpdateCooldownUI(ability, 0, playerNum);
        }
        abilitiesOnCooldown.Clear();
    }
 
	void OnGameOver(GameOver e){
		StopListeningForInput ();
        if (e.losingPlayer == this)
        {
            sr.enabled = false;
        }
	}

	public void ResetToNeutral(){
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
                if (abilityList.Count > 1)
                {
                    DoAbility(abilityList[1]);
                }
				break;
			case "B":
                if (abilityList.Count > 2)
                {
                    DoAbility(abilityList[2]);
                }
				break;
			default:
				break;
			}
		}
	}

	void DoAbility(Ability.Type type){
		if (!abilitiesOnCooldown.Contains (type)) {
			GameObject abilityObj = Instantiate (Services.PrefabDB.GetPrefabFromAbilityType (type), 
				transform.position, Quaternion.identity) as GameObject;
			EndAbility ();
			currentActiveAbility = abilityObj.GetComponent<Ability> ();
			currentActiveAbility.Init (gameObject);

			CastAbilityTask castTimeLockout = new CastAbilityTask (currentActiveAbility.castDuration, this, currentActiveAbility);
			AbilityCooldownTask abilityCooldown = new AbilityCooldownTask (type, currentActiveAbility.cooldown, this);
            HighlightAbilityOffCooldown highlightCooldownEnd = new HighlightAbilityOffCooldown(type, this, 
                Services.FightUIManager.abCDHighlightTime);
            abilityCooldown.Then(highlightCooldownEnd);


			StopListeningForInput ();
//			taskManager.AddTask (castTimeLockout);
			taskManager.AddTask (abilityCooldown);
		}
	}


	public void Fall(){
        rb.velocity = Vector2.zero;
        Services.EventManager.Fire (new PlayerFall (this));
        stageEdgeBoundaryCollider.enabled = false;
    }

    void OnPlayerFall(PlayerFall e)
    {
        StopListeningForInput();
    }

	public void TakeHit(int damageTaken, float baseKnockback, float knockbackGrowth, Vector3 knockbackDirection){
		damage += damageTaken;
		float knockbackMagnitude = baseKnockback + (knockbackGrowth * damage * knockbackDamageGrowthFactor);
		float hitstunDuration = knockbackMagnitude * hitstunFactor;
		Vector3 knockbackVector = knockbackMagnitude * knockbackDirection;
        HitLag hitLag = new HitLag(knockbackMagnitude);
        if (knockbackMagnitude > 0)
        {
            KnockbackTask startKnockback = new KnockbackTask(hitstunDuration, this, knockbackVector);
            hitLag
            .Then(startKnockback);
        }

		taskManager.AddTask (hitLag);
	}

	public void Stun(float hitstun){
		HitstunTask startHitstun = new HitstunTask (hitstun, this);
		taskManager.AddTask (startHitstun);
	}

    public void SetAbilityActive()
    {
		if (currentActiveAbility != null)
			currentActiveAbility.SetActive ();
    }

    public void TurnOffHitbox()
    {
        if (currentActiveAbility.GetType() == typeof(Attack))
        {
            Attack currentActiveAttack = currentActiveAbility as Attack;
            currentActiveAttack.TurnOffHitbox();
        }
    }

	public void EndAbility(){
		if (currentActiveAbility != null)
			currentActiveAbility.OnCastFinish ();
	}
		
}
