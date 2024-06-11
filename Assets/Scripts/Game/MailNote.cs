using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailNote : NoteController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        noteType = (Random.value > 0.5f) ? 1: 0;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Mail/Envelope");
        targetSource = $"Cardboard Box_{noteType}";
        destination = new Vector2(-99,-99);
        display.GetComponent<SpriteRenderer>().sprite = sprites[noteType];
        if (!isTarget) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void MoveToDestination()
    {
        base.MoveToDestination();
        displayRb.gameObject.GetComponent<Animator>().SetTrigger("Moved");
    }
}
