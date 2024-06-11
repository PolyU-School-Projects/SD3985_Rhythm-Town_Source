using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    TMP_Text textbox;

    void Awake()
    {
        textbox = GetComponent<TMP_Text>();
        textbox.maxVisibleCharacters = 0;
    }

    public void SetText(string textContent)
    {
        textbox.maxVisibleCharacters = 0;
        textbox.text = textContent;
    }
}
