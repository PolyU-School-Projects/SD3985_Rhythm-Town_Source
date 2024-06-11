using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerNoteController : NoteController
{
    protected MultiplayerGameController multiplayerGameController;
    protected override void Start()
    {
        base.Start();
        multiplayerGameController = GameObject.Find("MultiplayerGameController").GetComponent<MultiplayerGameController>();
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        MultiplayerGameController.NoteData noteData = multiplayerGameController.notes[noteID];
        if (!isClicked && noteData.isClicked)
        {
            destination = noteData.destination;
            MoveToDestination();
            gradeText.text = noteData.gradeText.ToString();
            isClicked = true;
        }
    }

    protected override void Judge()
    {
        base.Judge();
        multiplayerGameController.RecordNote(gameObject);
    }
}
