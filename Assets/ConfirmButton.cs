using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmButton : MonoBehaviour
{

    public void OnButtonClick()
    {
        SceneManager.LoadScene("Sushi");
    }
}