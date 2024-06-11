using UnityEngine;

public class CancelButton : MonoBehaviour
{

    public GameObject dialogBox;
    public void OnCancelButtonClick()
    {
        dialogBox.SetActive(false);
    }
}