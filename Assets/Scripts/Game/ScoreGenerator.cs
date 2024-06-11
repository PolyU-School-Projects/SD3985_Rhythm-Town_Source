using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGenerator
{
    static List<List<bool>> easy = new List<List<bool>>()
    {
        new List<bool>() {true, false},
        new List<bool>() {true, true},
        new List<bool>() {false, true}
    };
    
    static List<List<bool>> normal = new List<List<bool>>()
    {
        new List<bool>() {true, false, true, false},
        new List<bool>() {true, true, false, false},
        new List<bool>() {true, false, false, true}
    };

    static List<List<bool>> hard = new List<List<bool>>()
    {
        new List<bool>() {true, false, true, false},
        new List<bool>() {true, true, false, false},
        new List<bool>() {true, true, true, false},
        new List<bool>() {true, false, false, true},
        new List<bool>() {false, false, false, true},
        new List<bool>() {false, true, false, true},
    };

    static List<List<bool>> expert = new List<List<bool>>()
    {
        new List<bool>() {true, false, true, false},
        new List<bool>() {true, true, false, false},
        new List<bool>() {true, true, true, false},
        new List<bool>() {true, false, false, true},
        new List<bool>() {false, false, false, true},
        new List<bool>() {false, true, false, true},
    };

    static List<List<List<bool>>> beatLists = new List<List<List<bool>>>() { easy, normal, hard, expert };

    public static Queue<bool> GetScore(int difficulty, int musicLength, int barLength, int seed)
    {
        List<List<bool>> beatList = beatLists[difficulty];

        float length = (musicLength - 2) * barLength;
        Queue<bool> score = new Queue<bool>();
        Random.InitState(seed);

        while (length > 0)
        {
            List<bool> choice = beatList[Random.Range(0, beatList.Count - 1)];
            for (int i = 0; i < choice.Count; i++)
                score.Enqueue(choice[i]);
            length -= choice.Count;
        }

        for (int i = 0; i < barLength * 3; i++) { score.Enqueue(false); }

        return score;
    }
}
