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
	public bool isInvulnerable;
	public float effectiveRotation;

    private Ability currentActiveAbility;

	public List <Ability.Type> abilitiesOnCooldown;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();

		abilitiesOnCooldown = new List<Ability.Type> ();

		StartListeningForInput ();
		Services.EventManager.Register<GameOver> (OnGameOver);
        Services.EventManager.Register<PlayerFall>(OnPlayerFall);
	}
	
	// Update is called once per frame
	void Update () {
		if (actionable) {
			anim.SetBool ("neutral", true);
			Move ();
		} else {
		}
	}

	void Move(){
		Vector2 direction = new Vector2 (Input.GetAxis ("Horizontal_P" + playerNum), Input.GetAxis ("Vertical_P" + playerNum));

		rb.AddForce (accel * direction);

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = maxSpeed * direction.normalized;
		}

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
			currentActiveAbility = abilityObj.GetComponent<Ability> ();
			currentActiveAbility.Init (gameObject);

			CastAbilityTask castTimeLockout = new CastAbilityTask (currentActiveAbility.castDuration, this, currentActiveAbility);
			AbilityCooldownTask abilityCooldown = new AbilityCooldownTask (type, currentActiveAbility.cooldown, this);
            HighlightAbilityOffCooldown highlightCooldownEnd = new HighlightAbilityOffCooldown(type, this, 
                Services.FightUIManager.abCDHighlightTime);
            abilityCooldown.Then(highlightCooldownEnd);

			Services.TaskManager.AddTask (castTimeLockout);
			Services.TaskManager.AddTask (abilityCooldown);
		}
	}


	public void Fall(){
        rb.velocity = Vector2.zero;
        Services.EventManager.Fire (new PlayerFall (this));
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
		KnockbackTask startKnockback = new KnockbackTask (hitstunDuration, this, knockbackVector);
        ActionTask updateDamageUI = new ActionTask(Services.FightUIManager.UpdateDamageUI);

        hitLag
            .Then(startKnockback)
            .Then(updateDamageUI);

		Services.TaskManager.AddTask (hitLag);
	}

	public void Stun(float hitstun){
		HitstunTask startHitstun = new HitstunTask (hitstun, this);
		Services.TaskManager.AddTask (startHitstun);
	}

    public void SetAbilityActive()
    {
        currentActiveAbility.SetActive();
    }
}
