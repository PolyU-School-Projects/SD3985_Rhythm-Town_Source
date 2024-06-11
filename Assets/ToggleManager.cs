using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ricimi;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public Button confirmButton;

    public static string selectedToggleName;

    private void Start() { }

    public async void HandleConfirm()
    {
        // 获取选中的Toggle
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();

        if (activeToggle != null)
        {
            Debug.Log($"Avatar {activeToggle.name} selected");
            if (activeToggle != null)
            {
                selectedToggleName = activeToggle.name;
                await UpdateRecordAsync(selectedToggleName);
                GetComponent<SceneTransition>().PerformTransition();
            }
        }
        Debug.Log("button pressed");
    }

    public string GetSelectedToggleName()
    {
        return selectedToggleName;
    }

    public async Task UpdateRecordAsync(string selectedToggleName)
    {
        String sendback = GetSelectedToggleName();
        Debug.Log("avatar" + selectedToggleName + " saved to cloud");
        // Save the objectLevelRecord_Cloud to the cloud or perform any other necessary updates
        var data = new Dictionary<string, object> { { "Avatar", selectedToggleName }, };

        // Task CloudSaveService.Instance.Data.Player.SaveAsync(Dictionary<string, object> data)；
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
}
