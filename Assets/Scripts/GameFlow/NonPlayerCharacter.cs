using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogBox;


    void Start()
    {
        dialogBox.SetActive(false);
    }


    public void DisplayDialog()
    {
        dialogBox.SetActive(true);
    }
}