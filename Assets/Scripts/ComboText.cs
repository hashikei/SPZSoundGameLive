using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboText : MonoBehaviour
{
    private Text comboText;

    void Awake()
    {
        comboText = GetComponent<Text>();
        comboText.enabled = false;
    }

    public void Draw(int combo) {
        if (combo <= 0) {
            comboText.enabled = false;
            return;
        }

        if (combo < 5)
            return;

        comboText.text = combo.ToString() + " Combo!!";
        comboText.enabled = true;
    }
}
