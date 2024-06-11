using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoanDisplayManager_Cloud : MonoBehaviour
{
    public GameObject OnLoan;
    public GameObject miss_position;
    public GameObject coin_position;
    public GameObject range_position;
    public GameObject energy_position;

    private BookManager_Cloud Cloud_bookManager;

    private void Start()
    {
        Cloud_bookManager = FindObjectOfType<BookManager_Cloud>();
        if (Cloud_bookManager == null)
        {
            Debug.LogError("Cloud_bookManager not found in the scene.");
        }
        OnLoan.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Cloud_bookManager != null
        // && Cloud_bookManager.loanDisplayStatus_Cloud.ContainsKey(Cloud_bookManager.missObject)
        )
        {
            bool missObjectValue = Cloud_bookManager
                .objectData_Cloud[Cloud_bookManager.missObject]
                .IsEquipped;
            bool coinObjectValue = Cloud_bookManager
                .objectData_Cloud[Cloud_bookManager.coinObject]
                .IsEquipped;
            bool rangeObjectValue = Cloud_bookManager
                .objectData_Cloud[Cloud_bookManager.rangeObject]
                .IsEquipped;
            bool energyObjectValue = Cloud_bookManager
                .objectData_Cloud[Cloud_bookManager.energyObject]
                .IsEquipped;

            if (missObjectValue)
            {
                OnLoan.gameObject.SetActive(true);
                OnLoan.transform.position = miss_position.transform.position;

                Cloud_bookManager.objectData_Cloud[Cloud_bookManager.missObject].IsEquipped = false;
            }
            else if (coinObjectValue)
            {
                OnLoan.gameObject.SetActive(true);
                OnLoan.transform.position = coin_position.transform.position;
                Cloud_bookManager.objectData_Cloud[Cloud_bookManager.coinObject].IsEquipped = false;
            }
            else if (rangeObjectValue)
            {
                OnLoan.gameObject.SetActive(true);
                OnLoan.transform.position = range_position.transform.position;
                Cloud_bookManager.objectData_Cloud[Cloud_bookManager.rangeObject].IsEquipped =
                    false;
            }
            else if (energyObjectValue)
            {
                OnLoan.gameObject.SetActive(true);
                OnLoan.transform.position = energy_position.transform.position;
                Cloud_bookManager.objectData_Cloud[Cloud_bookManager.energyObject].IsEquipped =
                    false;
            }
        }
    }
}
