using UnityEngine;
using UnityEngine.UI;

public class LoanDisplayManager : MonoBehaviour
{
    public Image image;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;

    private BookManager bookManager;

    private void Start()
    {
        bookManager = FindObjectOfType<BookManager>();
        if (bookManager == null)
        {
            Debug.LogError("BookManager not found in the scene.");
        }
    }

    private void Update()
    {
        if (bookManager != null
        // && bookManager.loanDisplayStatus.ContainsKey(bookManager.missObject)
        )
        {
            bool missObjectValue = bookManager.loanDisplayStatus[bookManager.missObject];
            bool coinObjectValue = bookManager.loanDisplayStatus[bookManager.coinObject];
            bool rangeObjectValue = bookManager.loanDisplayStatus[bookManager.rangeObject];
            bool energyObjectValue = bookManager.loanDisplayStatus[bookManager.energyObject];

            if (missObjectValue)
            {
                image.sprite = sprite1;
                bookManager.loanDisplayStatus[bookManager.missObject] = false;
            }
            else if (coinObjectValue)
            {
                image.sprite = sprite2;
                bookManager.loanDisplayStatus[bookManager.coinObject] = false;
            }
            else if (rangeObjectValue)
            {
                image.sprite = sprite3;
                bookManager.loanDisplayStatus[bookManager.rangeObject] = false;
            }
            else if (energyObjectValue)
            {
                image.sprite = sprite4;
                bookManager.loanDisplayStatus[bookManager.energyObject] = false;
            }
        }
    }
}
