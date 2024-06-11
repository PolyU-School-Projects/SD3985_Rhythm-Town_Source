using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteControllerBackUp : MonoBehaviour
{
    protected Vector2 position,
        endPosition,
        checkPosition;
    public Vector2 destination { get; protected set; }
    public int noteID;
    public TMP_Text gradeText;
    public GameObject display;
    float speed,
        displaySpeed,
        beat,
        fullBeat,
        distanceThreshold,
        destroyTimer;
    float perfectThreshold = 5f;
    protected int noteType;
    protected bool isDetected;
    public bool isClicked = false,
        isTarget;
    public static bool isPaused;
    public string clickSource,
        targetSource;
    protected Rigidbody2D rb,
        displayRb;

    protected virtual void Start()
    {
        Debug.Log("testing music note");
        fullBeat = 60 / GameController.BPM / GameController.generateSpeed;
        beat = fullBeat;
        position = GameController.spawnPosition;
        endPosition = GameController.endPosition;
        checkPosition = GameController.checkPosition;
        displaySpeed = GameController.noteSpeed;
        speed = GameController.noteSpeed * Time.fixedDeltaTime / fullBeat;
        perfectThreshold *= Time.fixedDeltaTime;
        distanceThreshold = displaySpeed;
        destroyTimer = -1f;
        isPaused = false;

        rb = GetComponent<Rigidbody2D>();
        displayRb = display.GetComponent<Rigidbody2D>();
        rb.MovePosition(position);
    }

    protected virtual void Update()
    {
        if (!isPaused)
        {
            beat -= Time.deltaTime;
            if (destroyTimer < 0)
            {
                if (beat <= 0)
                {
                    beat = fullBeat;
                    //displayRb.MovePosition(Vector2.MoveTowards(displayRb.position, endPosition, displaySpeed));
                }
                displayRb.MovePosition(Vector2.MoveTowards(displayRb.position, endPosition, speed));
                rb.MovePosition(Vector2.MoveTowards(rb.position, endPosition, speed));
            }
            else
            {
                destroyTimer -= Time.deltaTime;
                if (destroyTimer <= 0)
                    Destroy(gameObject);
            }

            if (isDetected && isClicked)
            {
                Judge();
                isDetected = false;
            }

            if ((Vector2)transform.position == endPosition)
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void MoveToDestination(string clickSource)
    {
        displayRb.MovePosition(destination);
        rb.MovePosition(destination);
        destroyTimer = fullBeat / 2;
    }

    protected virtual void Judge()
    {
        float timing = beat;
        if (destination == new Vector2(-99, -99))
        {
            destination = GameObject.Find(clickSource).transform.position;
        }
        MoveToDestination(clickSource);
        GameController.noteCount += 1;
        if (clickSource == targetSource)
        {
            bool isPerfect =
                getDistance(checkPosition) < distanceThreshold
                && (timing >= fullBeat - perfectThreshold || timing <= perfectThreshold);
            GameController.grade += isPerfect ? 1f : 0.9f;
            gradeText.text = isPerfect ? "Perfect!" : "Good!";
            GameController.comboCount += 1;
        }
        else
        {
            gradeText.text = "Wrong!";
            GameController.comboCount = 0;
        }
    }

    public float getDistance(Vector2 position)
    {
        return Vector2.Distance(rb.position, position);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        isDetected = true;
        GameController.noteObjects.Add(this.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isDetected = false;

        if (targetSource != "None" && !isClicked)
        {
            gradeText.text = "Miss!";
            GameController.noteCount += 1;
            GameController.comboCount = 0;
        }

        GameController.noteObjects.Remove(this.gameObject);
    }
}
