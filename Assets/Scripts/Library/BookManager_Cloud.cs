using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Examples for saving data in the cloud.
// var data = new Dictionary<string, object>{ { "MySaveKey", "HelloWorld" } };
// await CloudSaveService.Instance.Data.ForceSaveAsync(data);

public class BookManager_Cloud : MonoBehaviour
{
    public GameObject missObject;
    public GameObject coinObject;
    public GameObject rangeObject;
    public GameObject energyObject;

    public TMP_Text dialog;
    public Button equipButton;
    public Button upgradeButton;
    public TMP_Text levelDisplay;

    GameObject clickedObject;

    private Dictionary<GameObject, string> objectTextMap_Cloud;
    public Dictionary<GameObject, BookData> objectData_Cloud;
    public Dictionary<GameObject, BookData> objectData_CloudUpload;

    // LoanDisplayManager_Cloud loanDisplayManager = FindObjectOfType<LoanDisplayManager_Cloud>();

    public class BookData
    {
        public int Grade { get; set; }
        public bool IsEquipped { get; set; }
    }

    private void Start()
    {
        StartCoroutine(StartAsync());
    }

    private IEnumerator StartAsync()
    {
        objectTextMap_Cloud = new Dictionary<GameObject, string>()
        {
            { missObject, "Miss: Help you reduce the number of missed note!" },
            { coinObject, "Coin: Earn more coin for each game!" },
            { rangeObject, "Range: The accuracy detection rage will increase!" },
            { energyObject, "Energy: Less energy needed for each game!" }
        };
        objectData_Cloud = new Dictionary<GameObject, BookData>()
        {
            {
                missObject,
                new BookData { Grade = 0, IsEquipped = false }
            },
            {
                coinObject,
                new BookData { Grade = 1, IsEquipped = false }
            },
            {
                rangeObject,
                new BookData { Grade = 2, IsEquipped = false }
            },
            {
                energyObject,
                new BookData { Grade = 3, IsEquipped = false }
            }
        };
        objectData_CloudUpload = new Dictionary<GameObject, BookData>()
        {
            {
                missObject,
                new BookData { Grade = objectData_Cloud[missObject].Grade, IsEquipped = false }
            },
            {
                coinObject,
                new BookData { Grade = objectData_Cloud[coinObject].Grade, IsEquipped = false }
            },
            {
                rangeObject,
                new BookData { Grade = objectData_Cloud[rangeObject].Grade, IsEquipped = false }
            },
            {
                energyObject,
                new BookData { Grade = objectData_Cloud[energyObject].Grade, IsEquipped = false }
            }
        };
        //Upgrade button
        upgradeButton.onClick.AddListener(OnUpdateButtonClick);
        upgradeButton.gameObject.SetActive(false);
        levelDisplay.gameObject.SetActive(false);
        //Equip button
        equipButton.onClick.AddListener(OnEquipButtonClick);
        equipButton.gameObject.SetActive(false);

        // Cloud initiation
        yield return UnityServices.InitializeAsync();
        yield return AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Other initialization code or actions if needed
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("detecting.........");
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                clickedObject = hit.collider.gameObject;
                Debug.Log("clickedObject: " + clickedObject.name);
                if (objectTextMap_Cloud.ContainsKey(clickedObject))
                {
                    string objectText = objectTextMap_Cloud[clickedObject];
                    //display of the corresponding dialog from the librarian
                    dialog.gameObject.SetActive(true);
                    dialog.text = objectText;

                    //displat of the corresponding button and info
                    upgradeButton.gameObject.SetActive(true);
                    levelDisplay.gameObject.SetActive(true);
                    levelDisplay.text = "Current level: " + objectData_Cloud[clickedObject].Grade;
                    equipButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public async void OnUpdateButtonClick()
    {
        Debug.Log("Update button clicked");

        if (objectData_Cloud.ContainsKey(clickedObject))
        {
            objectData_Cloud[clickedObject].Grade += 1;

            levelDisplay.text = "Current level: " + objectData_Cloud[clickedObject].Grade;
            Debug.Log("updating" + objectData_Cloud[clickedObject]);
        }

        await UpdateRecordAsync();
    }

    public async void OnEquipButtonClick()
    {
        Debug.Log("Equip button clicked");
        foreach (var kvp in objectData_CloudUpload)
        {
            kvp.Value.IsEquipped = false;
        }

        if (objectData_Cloud.ContainsKey(clickedObject))
        {
            objectData_Cloud[clickedObject].IsEquipped = true;
            objectData_CloudUpload[clickedObject].IsEquipped = true;
        }

        // foreach (var kvp in objectData_CloudUpload)
        // {
        //     Debug.Log(
        //         "Key: "
        //             + kvp.Key.name
        //             + ", Grade: "
        //             + kvp.Value.Grade
        //             + ", IsEquipped: "
        //             + kvp.Value.IsEquipped
        //     );
        // }
        await UpdateRecordAsync();
    }

    public async Task UpdateRecordAsync()
    {
        Debug.Log("cloud function called");
        // Save the objectLevelRecord_Cloud to the cloud or perform any other necessary updates
        var data = new Dictionary<string, object>
        {
            { "objectLevelRecord_Cloud", objectData_CloudUpload }
        };
        // Task CloudSaveService.Instance.Data.Player.SaveAsync(Dictionary<string, object> data)；
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
}
