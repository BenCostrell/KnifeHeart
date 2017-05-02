using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIAnimation : Task
{
    private float timeElapsed;
    private float duration;
    private RectTransform dmgTextTransform;
    private int playerNum;
    private int baseFontSize;
    private Vector3 targetRot;
    private Color baseColor;
    public DamageUIAnimation(RectTransform dmgText, float dur, int num)
    {
        dmgTextTransform = dmgText;
        duration = dur;
        playerNum = num;
    }

    protected override void Init()
    {
        Services.EventManager.Fire(new PlayerDamaged(playerNum));
        Services.EventManager.Register<PlayerDamaged>(OnAnotherDamageEvent);
        targetRot = UnityEngine.Random.Range(Services.FightUIManager.dmgRotationMin, Services.FightUIManager.dmgRotationMax) * Vector3.forward;
        int randomDirection = UnityEngine.Random.Range(0, 2);
        if (randomDirection == 1)
        {
            targetRot *= -1;
        }
        Text textComp = dmgTextTransform.gameObject.GetComponentInChildren<Text>();
        baseColor = textComp.color;
        textComp.color = Color.red;
    }


    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed <= duration / 2)
        {
            dmgTextTransform.localScale = Vector3.Lerp(Vector3.one, Services.FightUIManager.dmgTextScaleFactor * Vector3.one,
                Easing.QuadEaseOut(timeElapsed / (duration / 2)));
            dmgTextTransform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, targetRot, 
                Easing.QuadEaseOut(timeElapsed / (duration / 2))));
        }
        else
        {
            dmgTextTransform.localScale = Vector3.Lerp(Services.FightUIManager.dmgTextScaleFactor * Vector3.one, Vector3.one,
                Easing.QuadEaseIn((timeElapsed - (duration / 2)) / (duration / 2)));
            dmgTextTransform.localRotation = Quaternion.Euler(Vector3.Lerp(targetRot, Vector3.zero,
                Easing.QuadEaseIn((timeElapsed - (duration / 2)) / (duration / 2))));
        }

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    void OnAnotherDamageEvent(PlayerDamaged e)
    {
        if (e.playerNum == playerNum)
        {
            SetStatus(TaskStatus.Aborted);
        }
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<PlayerDamaged>(OnAnotherDamageEvent);
        dmgTextTransform.localScale = Vector3.one;
        dmgTextTransform.localRotation = Quaternion.identity;
        dmgTextTransform.gameObject.GetComponentInChildren<Text>().color = baseColor;
    }

}
