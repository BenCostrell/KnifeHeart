using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInPanel : Task {
    private GameObject panel;
    private bool slide;
    private float duration;
    private float timeElapsed;
    private Vector2 initialPos;
    private RectTransform rectTransform;

    public SlideInPanel(GameObject _panel, bool _slide)
    {
        panel = _panel;
        slide = _slide;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.DialogueUIManager.panelAppearTime;
        rectTransform = panel.GetComponent<RectTransform>();
        initialPos = rectTransform.anchoredPosition;
        panel.SetActive(true);
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        if (slide)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(initialPos + 900 * Vector2.left, initialPos, 
                Easing.ExpoEaseOut(timeElapsed / duration));
        }

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

}
