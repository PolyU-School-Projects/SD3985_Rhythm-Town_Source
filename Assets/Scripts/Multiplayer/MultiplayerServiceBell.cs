using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class MultiplayerServiceBell : ServiceBell
{
    NetworkObject networkObject;
    NetworkAnimator networkAnimator;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MultiplayerGameController multiplayerGameController = GameObject.Find("MultiplayerGameController").GetComponent<MultiplayerGameController>();
        networkObject = transform.parent.gameObject.GetComponent<NetworkObject>();
        networkAnimator = GetComponent<NetworkAnimator>();
        if ((!multiplayerGameController.IsHost && networkObject.IsOwner) ||
            (multiplayerGameController.IsHost && !networkObject.IsOwner))
        {
            transform.position = new Vector2(-transform.position.x, transform.position.y);            
        }
    }

    void Update()
    {
        GetComponent<CircleCollider2D>().enabled = networkObject.IsOwner && GameObject.Find("RelayManager").GetComponent<RelayManager>().hasStarted;
    }

    protected override void OnMouseOver()
    {
        if (!networkObject.IsOwner) return;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            networkAnimator.SetTrigger("Clicked");
            audioSource.PlayOneShot(soundFX);
        }
    }
}
