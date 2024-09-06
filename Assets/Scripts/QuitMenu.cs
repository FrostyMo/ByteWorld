using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour
{
    public GameObject quitMenuUI;  // Reference to the quit menu UI in the scene
    private bool isPaused = false; // Track if the game is paused
    public Button ResumeButton;
    public Button QuitButton;

    private void Start()
    {
        ResumeButton.onClick.AddListener(Resume);
        QuitButton.onClick.AddListener(QuitToMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    // Resume the game
    public void Resume()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        quitMenuUI.SetActive(false);   // Hide the quit menu
        
        isPaused = false;              // Set pause flag to false
    }

    // Pause the game
    void Pause()
    {
        GameStateManager.Instance.SetGameState(GameState.Paused);
        quitMenuUI.SetActive(true);    // Show the quit menu
        
        isPaused = true;               // Set pause flag to true
    }

    // Quit to main menu
    public void QuitToMainMenu()
    {
        GetComponent<EndLevel>().Quit();
        //Destroy(GetComponent<GameStateManager>());
        SceneManager.LoadScene("MainMenu");  // Load the main menu scene
        
    }
}