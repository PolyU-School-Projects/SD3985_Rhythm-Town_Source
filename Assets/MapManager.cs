using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using Cinemachine;
using System;

using System.Collections;

using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;



public class MapManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public GameObject virtualCamera;

    public static GameObject playerInstance;
    private int avatarNo;
    GameObject playerPrefab;



    private async void Start()
    {
        if (playerPrefab == null) { await LoadData(); }
        Vector2 spawnPosition = (PlayerMovement.lastPosition == null) ? Vector2.zero : PlayerMovement.lastPosition;
        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = playerInstance.transform;

    }


    public async Task LoadData()
    {
        try
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "Avatar" });
            if (playerData.ContainsKey("Avatar"))
            {
                Item item = playerData["Avatar"];
                if (!int.TryParse(item.Value.GetAsString(), out avatarNo)) { avatarNo = 0; }
                playerPrefab = playerPrefabs[avatarNo];
                Debug.Log("toggleName value: " + avatarNo);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

}