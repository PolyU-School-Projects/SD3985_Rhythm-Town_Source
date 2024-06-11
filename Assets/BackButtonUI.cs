using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonUI : MonoBehaviour
{
    [SerializeField]
    private string mapLevel = "Map";


    public void NewGameButton()
    {
        SceneManager.LoadScene(mapLevel);
    }
}
