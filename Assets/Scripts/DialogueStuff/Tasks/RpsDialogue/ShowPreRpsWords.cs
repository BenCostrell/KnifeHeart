using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShowPreRpsWords : Task
{
    private float durationPerWord;
    private float staggerTime;
    private float timeElapsed;
    private float totalDuration;
    private List<RectTransform> words;
    
    public ShowPreRpsWords(float durPerWord, float stagTime)
    {
        durationPerWord = durPerWord;
        staggerTime = stagTime;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        totalDuration = durationPerWord - staggerTime;
        words = new List<RectTransform>();
        foreach(GameObject word in Services.DialogueUIManager.rpsWords)
        {
            word.SetActive(true);
            totalDuration += staggerTime;
            RectTransform rectTrans = word.GetComponent<RectTransform>();
            words.Add(rectTrans);
            rectTrans.localScale = Vector3.zero;
        }
        words[0].transform.parent.gameObject.SetActive(true);
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(totalDuration, timeElapsed + Time.deltaTime);
        for (int i = 0; i < words.Count; i++)
        {
            float stagger = i * staggerTime;
            if ((timeElapsed >= stagger) && (timeElapsed <= stagger + durationPerWord))
            {
                words[i].localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one,
                    Easing.BackEaseOut((timeElapsed - stagger) / durationPerWord));
            }
            if (timeElapsed > stagger + durationPerWord)
            {
                words[i].localScale = Vector3.one;
            }
        }

        if (timeElapsed == totalDuration) SetStatus(TaskStatus.Success);
    }
}
