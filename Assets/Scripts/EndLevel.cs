using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private bool levelCompleted;

    public void Win()
    {
        if (!levelCompleted)
        {
            Debug.Log("Win");
            levelCompleted = true;
            //StartCoroutine(LoadNextScene());
            GameStateManager.Instance.LevelComplete();
        }
        
    }
    public void Lose()
    {
        if (!levelCompleted)
        {
            Debug.Log("Lose");
            levelCompleted = true;
            GameStateManager.Instance.ResetGame();
        }
    }

    

}
