using System;
using System.Collections.Generic;
using Ricimi;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    public float musicBPM, speed = 10f;
    public int musiclength;
    public static float BPM, noteSpeed, generateSpeed, grade, accuracy;
    public static int noteCount, comboCount, life, infiniteLevel;
    public static List<GameObject> noteObjects;
    static float beat, countdownBeat, fullBeat;
    float beatReset, fullBeatReset;
    public static bool isPaused, isOver, isGameEnd;
    public GameObject notePrefab;
    public Transform spawnPoint, endPoint, checkPoint;
    public RectTransform accuracyBar;
    public static Vector2 spawnPosition, endPosition, checkPosition;
    public TMP_Text countdownText, accuracyText, comboText;
    public AudioClip countIn;
    AudioSource audioSource, bgmAudioSource;
    static int countdown, barLength, scoreSeed;
    public int noteID, difficulty = 1;
    int scoreLength;
    protected Queue<bool> score;
    [SerializeField] List<Image> lifeIcons;
    [SerializeField] GameObject lifeIconsGroup;
    [SerializeField] Button pauseButton;
    [SerializeField] AudioClip bgmAudio, bgmLoopAudio;
    List<int> barLengths = new List<int> {2, 4, 4, 8};

    // Start is called before the first frame update
    protected virtual void Start()
    {
        BPM = musicBPM;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameSettings.soundVolume / 100f;
        bgmAudioSource = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
        bgmAudioSource.clip = GameStartManager.isInfinite? bgmLoopAudio : bgmAudio;
        bgmAudioSource.loop = GameStartManager.isInfinite;
        bgmAudioSource.volume = GameSettings.musicVolume / 100f;

        spawnPosition = spawnPoint.position;
        endPosition = endPoint.position;
        checkPosition = checkPoint.position;

        fullBeat = 60 / BPM;
        beat = fullBeat / generateSpeed - GameSettings.offset * Time.deltaTime;
        countdownBeat = fullBeat;
        fullBeatReset = fullBeat * 4;
        beatReset = fullBeat;
        
        difficulty = GameStartManager.lastDifficulty;
        scoreSeed = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        infiniteLevel = 0;
        scoreLength = 0;
        if (GameStartManager.isInfinite)
            GenerateInfiniteScore(infiniteLevel);
        else
            GenerateScore(scoreSeed);
        
        noteSpeed = Vector2.Distance(endPosition, spawnPosition) / speed / 2;
        noteObjects = new List<GameObject>();

        accuracyBar.anchorMax = new Vector2(1, 1);
        accuracyText.text = "100%";

        countdown = 4;
        isPaused = false;
        isOver = false;
        isGameEnd = false;

        grade = 0f;
        noteID = 0;
        noteCount = 0;
        if (GameStartManager.skillType == "Miss")
            noteCount -= GameStartManager.skillLevel * 2;

        comboCount = 0;
        life = 5;      
        foreach (Image icon in lifeIcons)
            icon.sprite = Resources.Load<Sprite>("Life/Heart - Pink");
        lifeIconsGroup.SetActive(GameStartManager.isInfinite);

        accuracyText.text = GameStartManager.isInfinite? "0" : "100%";
        accuracyBar.anchorMax = new Vector2(GameStartManager.isInfinite? 0 : 1, 1); 
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isGameEnd) return;

        if (isOver) {
            UploadRecord();
            beat -= Time.deltaTime;
            if (beat <= 0)
            {
                beat = fullBeat / generateSpeed;
                countdown--;
            }
            if (countdown <= 0)
            {
                isGameEnd = true;                
                GetResult();
                GetComponent<SceneTransition>().PerformTransition();
            }
            return;        
        }

        if (isPaused)
        {
            countdownText.text = "";
            if (bgmAudioSource.isPlaying) { bgmAudioSource.Pause(); }
        }
        else
        {
            if (CountIn())
            {
                pauseButton.interactable = true;
                beat -= Time.deltaTime;
                beatReset -= Time.deltaTime;
                if (beatReset <= 0)
                {
                    beat = 0;
                    beatReset = fullBeatReset;
                }
                if (beat <= 0)
                {
                    beat = fullBeat / generateSpeed;
                    if (noteID < scoreLength)
                        GenerateNote();
                    else
                        score.Dequeue();
                }
                accuracy = AccuracyCalculate(GameStartManager.isInfinite);
                DetectClick();
                if (!GameStartManager.isInfinite && score.Count <= 0)
                {                    
                    isOver = true;
                    countdown = barLength * 2;
                }
                if (GameStartManager.isInfinite)
                {
                    if (life <= 0)
                    {
                        isOver = true;
                        countdown = 1;
                    }
                    if (score.Count <= 0)
                    {
                        if (infiniteLevel < 3)
                            infiniteLevel++;
                        GenerateInfiniteScore(infiniteLevel);
                    }                      
                }
            }
        }
    }

    bool CountIn()
    {
        countdownText.enabled = countdown >= 0;
        if (countdown >= 0)
        {            
            pauseButton.interactable = false;
            countdownBeat -= Time.deltaTime;
            if (countdownBeat <= 0) {
                countdown--;
                if (countdown >= 0)
                {
                    audioSource.PlayOneShot(countIn);
                }     
                countdownBeat = fullBeat;
                countdownText.text = (countdown > 0) ? countdown.ToString(): "GO!";                
            }
        }
        if (countdown < 0)
        {
            if (!bgmAudioSource.isPlaying) { bgmAudioSource.Play(); }
        }
        
        NoteController.isPaused = countdown >= 0;
        return countdown < 0;
    }

    float AccuracyCalculate(bool infiniteMode)
    {
        float accuracy;
        if (infiniteMode)
        {
            accuracy = (int) (grade * 10);
            float progress = Math.Clamp(accuracy / 3000, 0, 1);
            accuracyBar.anchorMax = Vector2.Lerp(accuracyBar.anchorMax, new Vector2(progress, 1), 3 * Time.deltaTime);
            accuracyText.text = $"{ accuracy }";
        }
        else
        {
            accuracy = grade / noteCount;
            if (float.IsNaN(accuracy)) { accuracy = 1f; }
            accuracyBar.anchorMax = Vector2.Lerp(accuracyBar.anchorMax, new Vector2(accuracy, 1), 3 * Time.deltaTime);
            accuracy = Mathf.Round(accuracy * 10000) / 100f;
            accuracyText.text = $"{ accuracy }%";
        }
        comboText.text = (comboCount == 0) ? "" : $"{comboCount}\nCombo";

        return accuracy;
    }

    void DetectClick()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //If something was hit, the RaycastHit2D.collider will not be null.
            if (hit.collider != null && hit.collider.CompareTag("Clickable"))
            {
                if (noteObjects.Count != 0)
                {
                    noteObjects.Sort((o1, o2) => (int)(o1.GetComponent<NoteController>().getDistance(checkPosition) - o2.GetComponent<NoteController>().getDistance(checkPosition)));
                    NoteController nearestNote = noteObjects[0].GetComponent<NoteController>();
                    nearestNote.clickSource = hit.collider.gameObject.name;
                    nearestNote.isClicked = true;
                }
            }
        }
    }

    protected virtual void GenerateNote()
    {
        GameObject newNote = Instantiate(notePrefab, spawnPosition, notePrefab.transform.rotation);
        newNote.GetComponent<NoteController>().isTarget = score.Dequeue();
        newNote.GetComponent<NoteController>().noteID = noteID++;
    }

    public virtual void GenerateScore(int seed)
    {        
        barLength = barLengths[difficulty];
        generateSpeed = barLength / 4f;
        score = ScoreGenerator.GetScore(difficulty, musiclength, barLength, seed);
        scoreLength = score.Count - barLength * 3;
    }

    void GenerateInfiniteScore(int level)
    {
        barLength = barLengths[level];
        generateSpeed = barLength / 4f;
        int seed = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        score = ScoreGenerator.GetScore(level, (level + 1) * 10, barLength, seed);
        scoreLength += score.Count - ((level != 2)? 0: barLength * 2);
    }

    public void DecreaseLife()
    {
        life--;
        for (int i = 0; i < lifeIcons.Count; i++)
            lifeIcons[i].sprite = Resources.Load<Sprite>("Life/Heart - " + (i < life? "Pink" : "Gray"));
    }

    protected virtual void UploadRecord() { return; }
    protected virtual void GetResult() { return; }

    
    public static void Pause()
    {
        isPaused = true;
        NoteController.isPaused = true;
        countdown = 4;
        countdownBeat = fullBeat;
    }

    public static void Resume()
    {
        isPaused = false;        
    }
}