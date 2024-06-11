using UnityEngine;
using UnityEngine.UI;

public class ButtonStyle : MonoBehaviour
{
    [SerializeField] Sprite enabledStyle, disabledStyle;

    void Update()
    {
        GetComponent<Image>().sprite = transform.parent.GetComponent<Button>().interactable? enabledStyle: disabledStyle;
    }
}
