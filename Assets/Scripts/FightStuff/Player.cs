using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    [HideInInspector]
    public int playerNum;
    [HideInInspector]
	public List<Ability.Type> abilityList;
    [HideInInspector]
	public Rigidbody2D rb;
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
    public float wallBounceFactor;
    public float expectedHighKnockback;

    [HideInInspector]
    public int damage;
    [HideInInspector]
	public bool actionable;
    [HideInInspector]
	public bool isInvulnerable;
    [HideInInspector]
	public float effectiveRotation;
    [HideInInspector]
    public Vector3 previousVelocity;
    [HideInInspector]
    public Vector3 velocityAtDeath;

    private Ability currentActiveAbility;
    [HideInInspector]
	public List <Ability.Type> abilitiesOnCooldown;
    [HideInInspector]
    public TaskManager taskManager;

    [HideInInspector]
    public AudioSource castAudioSource;
    [HideInInspector]
    public AudioSource impactAudioSource;

    private FSM<Player> stateMachine;
    private ParticleSystem movementDust;
    private float baseMovementDustRotation;
    public int dustEmissionCount;
    public float animationVelocityFactor;
    public float hitParticleScalingFactor;
    [HideInInspector]
    public ParticleSystem knockbackTrail;
    private float baseKnockbackTrailRotation;
    public int knockbackTrailEmissionCount;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
        taskManager = new TaskManager();
        stateMachine = new FSM<Player>(this);
        movementDust = GetComponentsInChildren<ParticleSystem>()[0];
        knockbackTrail = GetComponentsInChildren<ParticleSystem>()[1];
        baseMovementDustRotation = movementDust.transform.localEulerAngles.z;
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
	}

    void LateUpdate()
    {
        previousVelocity = rb.velocity;
    }

    public void UpdateRotation(Vector2 direction)
    {
        float angleFacing = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (direction.magnitude > 0.01f)
        {
            if ((angleFacing >= -45) && (angleFacing < 45))
            {
                sr.flipX = true;
                effectiveRotation = 180;
                anim.SetBool("sideFacing", true);
                anim.SetBool("upFacing", false);
                anim.SetBool("downFacing", false);
            }
            else if ((angleFacing >= 45) && (angleFacing < 135))
            {
                sr.flipX = false;
                effectiveRotation = -90;
                anim.SetBool("sideFacing", false);
                anim.SetBool("upFacing", true);
                anim.SetBool("downFacing", false);
            }
            else if ((angleFacing >= 135) || ((angleFacing >= -180) && (angleFacing <= -135)))
            {
                sr.flipX = false;
                effectiveRotation = 0;
                anim.SetBool("sideFacing", true);
                anim.SetBool("upFacing", false);
                anim.SetBool("downFacing", false);
            }
            else {
                sr.flipX = false;
                effectiveRotation = 90;
                anim.SetBool("sideFacing", false);
                anim.SetBool("upFacing", false);
                anim.SetBool("downFacing", true);
            }
        }

        movementDust.transform.localRotation = Quaternion.Euler(0, 0, baseMovementDustRotation + effectiveRotation);
    }

    void Move()
    {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal_P" + playerNum), Input.GetAxis("Vertical_P" + playerNum));

        if (Vector2.Dot(direction, rb.velocity) <= 0)
        {
            rb.velocity = dashSpeed * direction.normalized;
        }

        rb.AddForce(accel * direction);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = maxSpeed * direction.normalized;
        }
        anim.SetFloat("InputMagnitude", direction.magnitude);
        anim.SetFloat("Velocity", Easing.QuadEaseOut(rb.velocity.magnitude/maxSpeed) * animationVelocityFactor);
        UpdateRotation(direction);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && !stageEdgeBoundaryCollider.enabled)
        {
            rb.velocity = Vector2.Reflect(previousVelocity, collision.contacts[0].normal);
        }
    }

	public void StartListeningForInput(){
		Services.EventManager.Register<ButtonPressed> (AbilityActivated);
		actionable = true;
        anim.SetBool("Actionable", true);
        //Debug.Log("started listening at time: " + Time.time);
	}

	public void StopListeningForInput(){
		Services.EventManager.Unregister<ButtonPressed> (AbilityActivated);
		actionable = false;
        anim.SetBool("Actionable", false);
        //Debug.Log("stopped listening at time: " + Time.time);
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
		//anim.SetTrigger ("ResetToNeutral");
		//isInvulnerable = false;
  //      Debug.Log("resetting to neutral at time: " + Time.time);
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
            Debug.Assert(currentActiveAbility != null);
			currentActiveAbility.Init (gameObject);

			//CastAbilityTask castTimeLockout = new CastAbilityTask (currentActiveAbility.castDuration, this, currentActiveAbility);
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
        velocityAtDeath = previousVelocity;
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
        EmitKnockbackTrail(knockbackDirection);
	}

	public void Stun(float hitstun){
		HitstunTask startHitstun = new HitstunTask (hitstun, this);
		taskManager.AddTask (startHitstun);
	}

    public void SetAbilityActive()
    {
        if (currentActiveAbility != null)
        {
            currentActiveAbility.SetActive();
        }
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

    public void DebugAnimationState(string state)
    {
        //Debug.Log("entered state: " + state + " at time " + Time.time);
    }

    public void CreateDustCloud()
    {
        movementDust.Emit(dustEmissionCount);
    }

    public void EmitKnockbackTrail(Vector2 knockbackDirection)
    {
        knockbackTrail.transform.parent.localRotation =
           Quaternion.Euler(0, 0, Mathf.Atan2(knockbackDirection.y, knockbackDirection.x) * Mathf.Rad2Deg);
        knockbackTrail.Emit(knockbackTrailEmissionCount);
        //Debug.Log("emitting at time: " + Time.time);
    }

    // STATES //
    //private class PlayerState : FSM<Player>.State { }

    //private class Actionable : PlayerState
    //{
    //    public override void OnEnter()
    //    {
    //        Context.StartListeningForInput();
    //    }

    //    public override void Update()
    //    {
    //        Context.anim.SetBool("neutral", true);
    //        Context.anim.SetBool("Actionable", true);
    //        Context.Move();
    //    }
    //}

    //private class Unactionable : PlayerState
    //{

    //    public override void OnEnter()
    //    {
    //        Context.StopListeningForInput();
    //    }

    //    public override void Update()
    //    {
    //        base.Update();
    //    }
    //}

}
