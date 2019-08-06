using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeText : MonoBehaviour
{
    private static readonly Dictionary<Judge, Color> JUDGE_COLOR = new Dictionary<Judge, Color> {
        { Judge.Perfect, Color.yellow },
        { Judge.Great, Color.green },
        { Judge.Good, Color.blue },
        { Judge.Miss, Color.red },
    };

    private Text judgeText;

    private float elapsedTime;

    void Awake()
    {
        judgeText = GetComponent<Text>();
        judgeText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!judgeText.IsActive())
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime > 0.5f) {
            judgeText.enabled = false;
        }
    }

    public void Draw(Judge judge) {
        judgeText.text = judge.ToString();
        judgeText.color = JUDGE_COLOR[judge];
        judgeText.enabled = true;
        elapsedTime = 0f;
    }
}
