using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager_mail : MonoBehaviour
{
    public GameObject objectPrefab1;
    public GameObject objectPrefab2;
    public Transform spawnPoint;
    public Transform endPoint;
    public Transform perfectScorePoint;
    public float objectSpeed = 5f;
    public float clickRange = 1f;

    //For score
    public static int score;
    public TMP_Text scoreText;

    //For dragging
    private bool isDragging = false; // Flag to indicate if dragging is in progress
    private GameObject draggedObject; // Reference to the currently dragged object
    private Vector3 initialMousePosition; // Initial mouse position when dragging starts

    private List<GameObject> activeObjects = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

            // Randomly choose which object to spawn
            GameObject selectedPrefab = Random.Range(0, 2) == 0 ? objectPrefab1 : objectPrefab2;

            // Instantiate the selected object at the spawn point
            GameObject newObject = Instantiate(
                selectedPrefab,
                spawnPoint.position,
                Quaternion.identity
            );
            activeObjects.Add(newObject);
        }
    }

    private void Update()
    {
        int clickedObjectIndex = -1; // Store the index of the clicked object

        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeObjects[i];
            if (obj != null)
            {
                Vector3 endPosition = endPoint.position;
                endPosition.z = obj.transform.position.z;

                // Move the object towards the target point
                obj.transform.position = Vector3.MoveTowards(
                    obj.transform.position,
                    endPosition,
                    objectSpeed * Time.deltaTime
                );

                // Check if the object has reached the target point
                if (obj.transform.position == endPosition)
                {
                    // Object reached the target point, remove it from the active objects list
                    activeObjects.RemoveAt(i);
                    Destroy(obj);
                }

                // Check for mouse click on the object
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Calculate the distance between the mouse click position and the selected point
                    float clickDistance = Mathf.Abs(mousePosition.x - endPosition.x);

                    // Store the index of the clicked object
                    clickedObjectIndex = i;

                    // Check for mouse dragging input
                    if (!isDragging && Input.GetMouseButtonDown(0))
                    {
                        isDragging = true;
                        draggedObject = obj;
                        initialMousePosition = Input.mousePosition;
                    }
                }
            }
            else
            {
                // Object has been destroyed, remove it from the active objects list
                activeObjects.RemoveAt(i);
            }
        }

        // Process the clicked object
        if (clickedObjectIndex != -1)
        {
            GameObject clickedObject = activeObjects[clickedObjectIndex];
            activeObjects.RemoveAt(clickedObjectIndex);
            Destroy(clickedObject);

            // Call the scoring method with the accuracy value
            Vector3 clickedPerfectScorePoint = perfectScorePoint.position;
            float clickDistance = Mathf.Abs(
                clickedPerfectScorePoint.x - clickedObject.transform.position.x
            );
            Score(clickDistance);
        }
    }

    public void Score(float cilckDistance)
    {
        Debug.Log("cilckDistance" + cilckDistance);

        // GameManager_mail.score += (int)scoreadd;
        GameManager_mail.score += 1;
        scoreText.text = "Score: " + GameManager_mail.score;
    }
}
