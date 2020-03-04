using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public static bool isGameOver = false;

    public GameObject GameOverPanel;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }
    public void gameOver()
    {
        if(!isGameOver)
        {
            isGameOver = true;
            Time.timeScale = 0f;
            GameOverPanel.SetActive(true);

        }
    }

}
