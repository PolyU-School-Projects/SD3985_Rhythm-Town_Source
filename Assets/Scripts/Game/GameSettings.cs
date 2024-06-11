using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider, soundSlider;
    [SerializeField] TMP_Text musicText, soundText, offsetText;
    public static int avatarID, musicVolume = 100, soundVolume = 100, offset;
    public static string nickname;
    static bool hasLoaded = false;
    static string[] settingsLabels = {"Settings_MusicVolume", "Settings_SoundVolume", "Settings_Offset", "Avatar", "inGameDisplayNickName"};

    void Awake()
    {
        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
        Display();
    }

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Anonymous");
        }
        if (!hasLoaded) {await LoadSettings();}
        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
    }

    // Update is called once per frame
    void Update()
    {
        musicVolume = (int) musicSlider.value;
        soundVolume = (int) soundSlider.value;
        Display();
    }

    void Display()
    {
        musicText.text = musicVolume.ToString();
        soundText.text = soundVolume.ToString();
        offsetText.text = offset.ToString();
    }

    public static async Task LoadSettings()
    {
        var settingsData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> (settingsLabels));
        musicVolume = 100;
        soundVolume = 100;
        offset = 0;
        avatarID = 0;

        if (settingsData.TryGetValue(settingsLabels[0], out var data))
            int.TryParse(data.Value.GetAsString(), out musicVolume);
        if (settingsData.TryGetValue(settingsLabels[1], out data))
            int.TryParse(data.Value.GetAsString(), out soundVolume);
        if (settingsData.TryGetValue(settingsLabels[2], out data))
            int.TryParse(data.Value.GetAsString(), out offset);
        if (settingsData.TryGetValue(settingsLabels[3], out data))
            int.TryParse(data.Value.GetAsString(), out avatarID);
        nickname = settingsData.TryGetValue(settingsLabels[4], out data)? data.Value.GetAsString() : "New Citizen";

        hasLoaded = true;
    }

    public async void SaveSettings()
    {
        var settingsData = new Dictionary<string, object> {
            { settingsLabels[0], musicVolume },
            { settingsLabels[1], soundVolume },
            { settingsLabels[2], offset }
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(settingsData);
    }

    public void OffsetAdjust(int amount) { offset += amount; }
}
