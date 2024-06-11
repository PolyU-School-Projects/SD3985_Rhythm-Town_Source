using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerGameEndManager : MonoBehaviour
{
    [SerializeField] TMP_Text hostNameText, clientNameText, hostRecordText, clientRecordText, coinText, expText;
    [SerializeField] Image hostAvatar, clientAvatar;
    [SerializeField] List<GameObject> difficultyIcons;
    public static RelayManager.PlayerData hostData, clientData;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < difficultyIcons.Count; i++)
        {
            difficultyIcons[i].SetActive(i == GameStartManager.lastDifficulty);
        }
        hostAvatar.sprite = Resources.Load<Sprite>($"Avatars/{hostData.avatarID}");
        clientAvatar.sprite = Resources.Load<Sprite>($"Avatars/{clientData.avatarID}");
        hostNameText.text = $"{hostData.nickname}";
        clientNameText.text = $"{clientData.nickname}";
        hostRecordText.text = $"{hostData.accuracy}%";
        clientRecordText.text = $"{clientData.accuracy}%";
    }
}
