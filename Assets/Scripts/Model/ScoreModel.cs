using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ScoreModel 
{
    public static ScoreModel Instance = new ScoreModel();

    public event Action<int> UpdateScoreHandler;
    public int Score{get; private set;}

    public void AddScore(int score)
    {
        Score++;
        UpdateScoreHandler?.Invoke(Score);
    }
    public void ResetScore()
    {
        Score = 0;
    }
}
