using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNote : NoteController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        noteType = Random.Range(0, 3);
        Sprite[] sprites = Resources.LoadAll<Sprite>("car");
        Debug.Log("sprites: " + sprites);
        targetSource = $"camera";
        destination = new Vector2(-99, -99);
        display.GetComponent<SpriteRenderer>().sprite = sprites[noteType];
        if (!isTarget)
        {
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
        destroyTimer = fullBeat / 3;
    }
}
