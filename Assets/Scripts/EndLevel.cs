using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private bool levelCompleted;
    public GameObject player;
    PlayerMovement playerMovement;

    public void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void Win()
    {
        if (!levelCompleted)
        {
            Debug.Log("Win");
            levelCompleted = true;
            //StartCoroutine(LoadNextScene());
            SaveGameData(true); // true means the player won
            GameStateManager.Instance.LevelComplete();
            // Capture and save game data for normal or random game
            
        }
        
    }
    public void Lose()
    {
        if (!levelCompleted)
        {
            Debug.Log("Lose");
            levelCompleted = true;
            // Capture and save game data for normal or random game
            SaveGameData(false); // false means the player lost

            GameStateManager.Instance.ResetGame();

            
        }
    }

    private void SaveGameData(bool isWin)
    {
        Debug.Log("Allowed: " + playerMovement.GetAllowedMoves() + "\n attempts" + GameStateManager.Instance.GetRetryCount());
        Debug.Log("Required: "+ playerMovement.GetRequiredMoves());
        GameData gameData = new()
        {
            allowedMoves = playerMovement.GetAllowedMoves(),
            attempts = GameStateManager.Instance.GetRetryCount(),
            cleared = isWin,
            score = playerMovement.GetTotalScore(),
            leastMoves = playerMovement.GetMovesTaken(),
            requiredMoves = playerMovement.GetRequiredMoves(),
        };

        // Check if it’s a random game or a normal level
        if (TileGenerator.levelNumber == Level.Random)
        {
            // Handle random game session
            string sessionId = APIManager.API.GetCurrentSessionId();
            APIManager.API.UpdateRandomGameSession(sessionId, gameData, isWin);
        }
        else
        {
            // Handle normal game level
            string levelName = TileGenerator.levelNumber.ToString();
            APIManager.API.EndLevel(levelName, gameData);
        }
    }

}
