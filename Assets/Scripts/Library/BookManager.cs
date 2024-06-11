using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// await UnityServices.InitializeAsync();
// await AuthenticationService.Instance.SignInAnonymouslyAsync();

//Examples for saving data in the cloud.
// var data = new Dictionary<string, object>{ { "MySaveKey", "HelloWorld" } };
// await CloudSaveService.Instance.Data.ForceSaveAsync(data);

public class BookManager : MonoBehaviour
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

    private Dictionary<GameObject, string> objectTextMap;

    /*need further modification
        loan: should remember which loan was last equipped
        level: should remember the level*/
    public Dictionary<GameObject, bool> loanDisplayStatus;
    public Dictionary<GameObject, int> objectLevelRecord;

    private void Start()
    {
        objectTextMap = new Dictionary<GameObject, string>()
        {
            { missObject, "Miss: Help you reduce the number of missed note!" },
            { coinObject, "Coin: Earn more coin for each game!" },
            { rangeObject, "Range: The accuracy detection rage will increase!" },
            { energyObject, "Energy: Less energy needed for each game!" }
        };
        loanDisplayStatus = new Dictionary<GameObject, bool>()
        {
            { missObject, false },
            { coinObject, false },
            { rangeObject, false },
            { energyObject, false }
        };
        objectLevelRecord = new Dictionary<GameObject, int>()
        {
            { missObject, 0 },
            { coinObject, 1 },
            { rangeObject, 2 },
            { energyObject, 3 }
        };
        //Upgrade button
        upgradeButton.onClick.AddListener(OnUpdateButtonClick);
        upgradeButton.gameObject.SetActive(false);
        levelDisplay.gameObject.SetActive(false);
        //Equip button
        equipButton.onClick.AddListener(OnEquipButtonClick);
        equipButton.gameObject.SetActive(false);
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

                if (objectTextMap.ContainsKey(clickedObject))
                {
                    string objectText = objectTextMap[clickedObject];
                    //display of the corresponding dialog from the librarian
                    dialog.gameObject.SetActive(true);
                    dialog.text = objectText;

                    //displat of the corresponding button and info
                    upgradeButton.gameObject.SetActive(true);
                    levelDisplay.gameObject.SetActive(true);
                    levelDisplay.text = "Current level: " + objectLevelRecord[clickedObject];
                    equipButton.gameObject.SetActive(true);
                }
            }
            // else
            // {
            //     dialog.text =
            //         "Welcome to the library! How can I help? blablabla bla for testing purposes";
            //     upgradeButton.gameObject.SetActive(false);
            //     levelDisplay.gameObject.SetActive(false);
            //     equipButton.gameObject.SetActive(false);
            // }
        }
    }

    public void OnUpdateButtonClick()
    {
        Debug.Log("Update button clicked");

        if (objectLevelRecord.ContainsKey(clickedObject))
        {
            objectLevelRecord[clickedObject] += 1;

            levelDisplay.text = "Current level: " + objectLevelRecord[clickedObject];
            Debug.Log("updating " + objectLevelRecord[clickedObject]);
        }
    }

    public void OnEquipButtonClick()
    {
        Debug.Log("Equip button clicked");

        if (loanDisplayStatus.ContainsKey(clickedObject))
        {
            loanDisplayStatus[clickedObject] = true;
        }
    }
}
