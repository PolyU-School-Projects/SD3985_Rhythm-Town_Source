
using Ricimi;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerButtonController : MonoBehaviour
{
    public void ResumeGame()
    {
        GameObject.Find("RelayManager").GetComponent<RelayManager>().ResumeGameServerRpc();
    }

    public void QuitGame()
    {
        GameObject.Find("RelayManager").GetComponent<RelayManager>().hasQuit = true;
        GameObject.Find("RelayManager").GetComponent<RelayManager>().QuitGameServerRpc();
        GetComponent<SceneTransition>().PerformTransition();
    }
}