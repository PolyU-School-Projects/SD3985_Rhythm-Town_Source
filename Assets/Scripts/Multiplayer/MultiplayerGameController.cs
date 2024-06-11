using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameController : GameController
{
    public struct NoteData : INetworkSerializable, IEquatable<NoteData>
    {
        public int noteID;
        public bool isClicked;
        public Vector2 destination;
        public FixedString32Bytes gradeText;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref noteID);
            serializer.SerializeValue(ref isClicked);
            serializer.SerializeValue(ref destination);
            serializer.SerializeValue(ref gradeText);
        }

        public bool Equals(NoteData other) { return other.noteID == noteID; }
    }

    public NetworkList<NoteData> notes;
    public Transform hostDestination, clientDestination, hostCheckPoint, clientCheckPoint;
    public GameObject hostJudgeRange, clientJudgeRange;
    public static Vector2 _hostDestination, _clientDestination; 
    [SerializeField] TMP_Text hostText, clientText;
    [SerializeField] RelayManager relayManager;

    protected override void Start()
    {        
        base.Start();
        isPaused = true;
        notes = new NetworkList<NoteData>();       
        _hostDestination = hostDestination.position;
        _clientDestination = clientDestination.position;
    }

    protected override void Update()
    {
        base.Update();
        checkPosition = IsHost? hostCheckPoint.position: clientCheckPoint.position;
        hostJudgeRange.SetActive(IsHost);
        clientJudgeRange.SetActive(!IsHost);
        hostText.text = IsHost? "1P\n<size=50%>(You)" : "1P";
        clientText.text = IsHost? "2P" : "2P\n<size=50%>(You)";
    }

    public void RecordNote(GameObject noteObject)
    {
        MultiplayerNoteController noteController = noteObject.GetComponent<MultiplayerNoteController>();
        int id = noteController.noteID;
        
        if (id < notes.Count)
        {
            UpdateNoteListServerRpc(new NoteData
            {                
                noteID = id,
                isClicked = true,
                destination = noteController.destination,
                gradeText = noteController.gradeText.text
            });  
        }
    }

    public override void GenerateScore(int seed)
    {
        base.GenerateScore(seed);
        if (IsHost)
        {
            for (int i = 0; i < score.Count; i++)
            {
                notes.Add(new NoteData
                {
                    noteID = i,
                    isClicked = false,
                    gradeText = ""
                });
                if (i == score.Count - 1)
                    Debug.Log("Notelist finalized with length" + i);
            }
        }
    }

    protected override void UploadRecord() { relayManager.UploadPlayerRecordServerRpc(RelayManager.playerNo, accuracy); }

    protected override void GetResult() { relayManager.GetResult(); NetworkManager.Singleton.Shutdown(); }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateNoteListServerRpc(NoteData noteData) { notes[noteData.noteID] = noteData; }
}
