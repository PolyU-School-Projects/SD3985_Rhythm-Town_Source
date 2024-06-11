using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ricimi;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : NetworkBehaviour
{
    public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
    {
        public int avatarID;
        public FixedString32Bytes nickname;
        public bool hasJoined, isReady;     
        public float accuracy;   

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref avatarID);
            serializer.SerializeValue(ref nickname);
            serializer.SerializeValue(ref isReady);
            serializer.SerializeValue(ref hasJoined);
            serializer.SerializeValue(ref accuracy);
        }

        public bool Equals(PlayerData other) { throw new NotImplementedException(); }
    }
    
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] Button hostButton, joinButton, startButton, pauseButton;
    [SerializeField] TMP_Text startButtonText, hintText, dialogueText;
    [SerializeField] List<GameObject> difficultyIcons, playerIconBorders, playerIcons, playerNameTexts;
    [SerializeField] GameObject preparationUI;
    [SerializeField] MultiplayerGameController multiplayerGameController;
    [SerializeField] Color readyColor;
    public NetworkList<PlayerData> players = new NetworkList<PlayerData>();
    NetworkVariable<int> difficulty;
    public NetworkVariable<bool> GameStarted, GamePaused;
    public static int playerNo;
    public bool hasQuit, hasStarted;

    void Awake()
    {
        players = new NetworkList<PlayerData>();
        difficulty = new NetworkVariable<int>(0);
        GameStarted = new NetworkVariable<bool>(false);
        GamePaused = new NetworkVariable<bool>(true);
        hasQuit = false;
        hasStarted = false;
    }

    public override void OnNetworkSpawn()
    {
        GameStarted.OnValueChanged += GameStartedState;
        GamePaused.OnValueChanged += GamePausedState;
        FinalizeConnection();
    }

    void GameStartedState(bool previous, bool current)
    {
        if (GameStarted.Value)
        {
            int seed = joinCodeInputField.text.ToUpper().ToIntArray().Sum();
            multiplayerGameController.GenerateScore(seed);
            preparationUI.SetActive(false);
            ResumeGameServerRpc();
            hasStarted = true;
            GameController.Resume();
        }
        else 
        {
            if (IsHost)
                DisconnectHintClientRpc();
        }
    }

    void GamePausedState(bool previous, bool current)
    {
        if (GamePaused.Value)
        {
            pauseButton.GetComponent<PopupOpener>().OpenPopup();
            GameController.Pause();            
        }
        else
        {
            Debug.Log("Pause Change Listened");
            foreach (GameObject pauseObject in GameObject.FindGameObjectsWithTag("Pause"))
                pauseObject.GetComponent<Popup>().Close();
            GameController.Resume();     
        }
    }

    void Update()
    {
        if (startButtonText.text == "Start")
        {
            startButton.interactable = players[0].isReady && players[1].isReady;            
        }
        for (int i = 0; i < players.Count; i++)
        {
            playerIcons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Avatars/{players[i].avatarID}");
            playerNameTexts[i].GetComponent<TMP_Text>().text = $"{i + 1}P" + (i == playerNo? " (You)": "");
            playerIconBorders[i].transform.parent.gameObject.SetActive(players[i].hasJoined);
            playerIconBorders[i].GetComponent<Image>().color = Color.Lerp(playerIconBorders[i].GetComponent<Image>().color, players[i].isReady? readyColor : Color.white, 0.1f);
        }
    }

    public async void StartRelay()
    {
        try
        {
            playerNo = 0;
            DisplayHint();
            string joinCode = await StartHostWithRelay();
            joinCodeInputField.text = joinCode;
        }
        catch (Exception)
        {
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = "Something went wrong! Please check your input or try again later.";
        }
    }
    
    public async void JoinRelay()
    {
        playerNo = 1;
        DisplayHint();
        Debug.Log(joinCodeInputField.text.ToUpper());
        await StartClientWithRelay(joinCodeInputField.text.ToUpper());
    }

    async Task<string> StartHostWithRelay()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Anonymous");
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "wss"));
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return NetworkManager.Singleton.StartHost()? joinCode : null;
    }

    async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Anonymous");
        }

        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    void FinalizeConnection()
    {
        hintText.enabled = false;
        if (IsHost)
        {
            difficulty.Value = GameStartManager.lastDifficulty;
            while (players.Count < 2)
                players.Add(new PlayerData{avatarID = 0, nickname = "", isReady = false, hasJoined = false, accuracy = 100});
        }
        playerNo = IsHost? 0 : 1;
        PlayerData temp = new PlayerData {
            avatarID = GameSettings.avatarID,
            nickname = GameSettings.nickname,
            isReady = false,
            hasJoined = true,
            accuracy = 100,
        };
        SetPlayerDataServerRpc(playerNo, temp);
        for (int i = 0; i < difficultyIcons.Count; i++)
        {
            difficultyIcons[i].SetActive(i == difficulty.Value);
        }
        multiplayerGameController.difficulty = difficulty.Value;
        GameStartManager.lastDifficulty = difficulty.Value;
        joinCodeInputField.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;
        startButton.interactable = true;
    }

    public void SetReady()
    {
        if (startButtonText.text == "Ready")
        {
            PlayerData temp = new PlayerData {
                avatarID = players[playerNo].avatarID,
                nickname = players[playerNo].nickname,
                isReady = true,
                hasJoined = true,
                accuracy = players[playerNo].accuracy
            };
            SetPlayerDataServerRpc(playerNo, temp);
            startButtonText.text = "Start";
        }
        else
            StartGameServerRpc();
    }

    void DisplayHint()
    {
        hintText.maxVisibleCharacters = 0;
        hintText.text = "Loading...";
    }

    public void GetResult()
    {
        MultiplayerGameEndManager.hostData = players[0];
        MultiplayerGameEndManager.clientData = players[1];
    }

    public void Disconnect() { NetworkManager.Singleton.Shutdown(); }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerDataServerRpc(int index, PlayerData playerData) { players[index] = playerData; }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc() { GameStarted.Value = true; }
    [ServerRpc(RequireOwnership = false)]
    public void QuitGameServerRpc() { GameStarted.Value = false; }
    [ServerRpc(RequireOwnership = false)]
    public void PauseGameServerRpc() { GamePaused.Value = true; }
    [ServerRpc(RequireOwnership = false)]
    public void ResumeGameServerRpc() { GamePaused.Value = false; }
    [ServerRpc(RequireOwnership = false)]
    public void UploadPlayerRecordServerRpc(int index, float record)
    {
        PlayerData temp = new PlayerData {
            avatarID = players[index].avatarID,
            nickname = players[index].nickname,
            isReady = players[index].isReady,
            hasJoined = players[index].hasJoined,
            accuracy = record,
        };
        SetPlayerDataServerRpc(index, temp);
    }
    [ClientRpc]
    private void DisconnectHintClientRpc()
    {
        if (hasStarted && !hasQuit)
            GetComponent<PopupOpener>().OpenPopup();
        NetworkManager.Singleton.Shutdown();
    }
}
