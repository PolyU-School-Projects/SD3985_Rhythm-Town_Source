using UnityEngine;

public class MultiplayerSushiNote : MultiplayerNoteController
{
    public Sprite hostSprite, clientSprite;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();        
        destination = multiplayerGameController.IsHost? multiplayerGameController.hostDestination.position : multiplayerGameController.clientDestination.position;
        //Sprite[] sprites = Resources.LoadAll<Sprite>($"Sushi/{noteType}");
        //noteType = isTarget? 1: 0;
        targetSource = isTarget? "ServiceBell" : "None";
        if (isTarget)
        {
            float randomTarget = Random.value;
            GetComponent<SpriteRenderer>().sprite = (randomTarget > 0.5f)? hostSprite : clientSprite;
            isTarget = multiplayerGameController.IsHost? randomTarget > 0.5f : randomTarget < 0.5f;
            targetSource = isTarget? "ServiceBell" : "None";
        }
        else{
            if (Random.value > 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
