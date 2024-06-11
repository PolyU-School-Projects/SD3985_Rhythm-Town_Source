using Ricimi;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using System.Threading.Tasks;
using Cinemachine;
using System;
public class EnterBuilding : MonoBehaviour
{
    private bool inRange;
    public static bool isPopupOpened;

    [SerializeField] int levelLimit;
    [SerializeField] PopupOpener limitedPopUp;

    int Level;

    async void Start()
    {
        //get the coin data
        GetBalancesOptions options = new GetBalancesOptions { ItemsPerFetch = 4, };

        GetBalancesResult getBalancesResult =
            await EconomyService.Instance.PlayerBalances.GetBalancesAsync(options);

        if (getBalancesResult.Balances.Count > 0)
        {
            //level
            PlayerBalance level = getBalancesResult.Balances[3];
            string levelText = level.Balance.ToString();
            Level = int.Parse(levelText);
        }

        inRange = false;
        isPopupOpened = false;

    }

    void Update()
    {
        if (!inRange) return;
        if (Input.GetKeyDown(KeyCode.E) && !isPopupOpened)
        {
            Debug.Log("Level"+Level);

            if (Level >= levelLimit)
            {
                Enter();

            }
            else
            {
                Limited();
            }
        }
    }

    void Enter()
    {
        PopupOpener popupOpener = GetComponent<PopupOpener>();
        popupOpener.OpenPopup();
    }

    void Limited()
    {
        limitedPopUp.OpenPopup();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") { inRange = true; };
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && inRange) { inRange = false; };
    }

    void OnMouseDown()
    {
        if (!isPopupOpened)
        {
            if (Level >= levelLimit)
            {
                Enter();

            }
            else
            {
                Limited();
            }
        }
    }
}
