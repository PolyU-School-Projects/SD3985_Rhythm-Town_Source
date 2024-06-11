using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    private GameObject player;

    private void Start()
    {

        player = MapManager.playerInstance; // 在 Start 方法中获取 playerInstance

        if (player != null)
        {

        }
        else
        {
            Debug.LogError("PlayerInstance not found!");
        }
    }
}