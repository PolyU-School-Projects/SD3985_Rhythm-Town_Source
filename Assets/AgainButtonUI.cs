using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgainButtonUI : MonoBehaviour
{
    [SerializeField]
    private string GameLevel = "Shusi";


    public void NewGameButton()
    {
        SceneManager.LoadScene(GameLevel);
    }

}
