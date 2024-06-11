using TMPro;
using UnityEngine;

public class ShowPassword : MonoBehaviour
{
    [SerializeField] TMP_InputField passwordInputField;

    public void TogglePasswordDisplay()
    {
        passwordInputField.contentType = (passwordInputField.contentType == TMP_InputField.ContentType.Password) ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInputField.Select();
    }
}
