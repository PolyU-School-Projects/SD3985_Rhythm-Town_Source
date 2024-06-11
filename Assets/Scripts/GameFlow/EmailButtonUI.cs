using System.Collections;
using System.Collections.Generic;
using Ricimi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmailButtonUI : MonoBehaviour
{
    [SerializeField]

    public GameObject dialogBox;

    public void NewGameButton()
    {
        GetComponent<SceneTransition>().PerformTransition();
    }

    public void ClosePage()
    {
        dialogBox.SetActive(false);
    }
}
