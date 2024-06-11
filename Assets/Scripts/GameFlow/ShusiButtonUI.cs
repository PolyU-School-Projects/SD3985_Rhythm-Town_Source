using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShusiButtonUI : MonoBehaviour
{
    [SerializeField]
    private string SushiGameLevel = "Sushi";

    public GameObject dialogBox;

    public void NewGameButton()
    {
        SceneManager.LoadScene(SushiGameLevel);
    }

    public void ClosePage()
    {
        dialogBox.SetActive(false);
    }
}
