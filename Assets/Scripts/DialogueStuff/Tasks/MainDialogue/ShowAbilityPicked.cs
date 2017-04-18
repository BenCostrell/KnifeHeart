using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAbilityPicked : Task {
    private int playerNum;
    private GameObject symbol;
    private RectTransform symbolRectTransform;
    private float timeElapsed;
    private float growthTime;
    private float absorbTime;
    private Vector3 targetPosition;
    private float targetScale;

    protected override void Init()
    {
        playerNum = 3 - Services.VisualNovelScene.currentTurnPlayerNum;
        List<Ability.Type> abilityList = Services.VisualNovelScene.abilityLists[playerNum - 1];
        symbol = Services.DialogueUIManager.CreateAbilitySymbol(abilityList[abilityList.Count-1]);
        symbolRectTransform = symbol.GetComponent<RectTransform>();
        timeElapsed = 0;
        symbol.transform.localScale = Vector3.zero;
        symbolRectTransform.anchoredPosition3D = Vector3.zero;
        growthTime = Services.DialogueUIManager.symbolGrowthTime;
        absorbTime = Services.DialogueUIManager.symbolAbsorbTime;
        targetScale = Services.DialogueUIManager.symbolTargetScale;
        if (playerNum == 1)
        {
            targetPosition = Services.DialogueUIManager.ponytail.GetComponent<RectTransform>().anchoredPosition3D;
        }
        else
        {
            targetPosition = Services.DialogueUIManager.pigtails.GetComponent<RectTransform>().anchoredPosition3D;
        }
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed <= growthTime)
        {
            symbol.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale * Vector3.one, 
                Easing.ExpoEaseOut(timeElapsed / growthTime));
        }
        else if (timeElapsed <= (growthTime + absorbTime))
        {
            symbolRectTransform.anchoredPosition3D = Vector3.Lerp(Vector2.zero, targetPosition, 
                Easing.ExpoEaseOut((timeElapsed - growthTime) / absorbTime));
            symbol.transform.localScale = Vector3.Lerp(targetScale * Vector3.one, Vector3.zero,
                Easing.ExpoEaseOut((timeElapsed - growthTime) / absorbTime));
        }

        if (timeElapsed >= (growthTime + absorbTime))
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.DialogueUIManager.DestroySymbol(symbol);
    }

}
