using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public enum GameState
{
    Playing,
    Paused,
    GeneratingPath,
    // Add more states as needed
    Dialog,
    Resetting,
    End,
    ExecutingMovements,
    VariableBinding,
    ConditionalEditing,
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.GeneratingPath;

    public GameObject player;

    public GameObject HUD;

    public GameObject loadingPanel; // For loading screens

    public GameObject dialogPanel; // For dialog box
    public GameObject speedDropdown; // For speed selection during movement

    public TextMeshProUGUI dialogText;
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;
    public Button dontShowAgainButton;
    public TextMeshProUGUI endText;

    private string[] dialogMessages;
    private int currentMessageIndex = 0;
    private string dialogPrefKey;

    private static int retryCount = 0;
    private const int maxRetries = 3; // Example max retries

    private PlayerMovement playerMovementScript;
    //public GameObject variableBindingPanel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            InitializeDialogPanel();
            //HUD.SetActive(false);
        }
    }

    private void Start()
    {
        playerMovementScript = player.GetComponent<PlayerMovement>();
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("In SetGameState with state: " + newState);
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(newState == GameState.GeneratingPath);
        }

        if (dialogPanel != null)
        {
            Debug.Log("Dialog not null");
            dialogPanel.SetActive(newState == GameState.Dialog);
        }

        if (speedDropdown != null)
        {
            speedDropdown.SetActive(newState == GameState.ExecutingMovements);
        }
    }

    public bool IsGamePaused()
    {
        return CurrentState != GameState.Playing;
    }


    private void InitializeDialogPanel()
    {
        if (dialogPanel != null)
        {
            nextButton.onClick.AddListener(NextMessage);
            prevButton.onClick.AddListener(PreviousMessage);
            closeButton.onClick.AddListener(CloseDialog);
            dontShowAgainButton.onClick.AddListener(DontShowAgain);

            dialogPanel.SetActive(false); // Hide initially
        }
    }


    public void ShowDialog(string[] messages, string prefKey, bool showHUD)
    {
        dialogPrefKey = prefKey;
        EnableHUD(showHUD);
        if (PlayerPrefs.GetInt(dialogPrefKey, 0) == 1)
        {
            EnableHUD(true);
            SetGameState(GameState.Playing);
            return;
        }
        //EnableHUD(showHUD);
        dialogMessages = messages;
        currentMessageIndex = 0;
        UpdateDialog();
        SetGameState(GameState.Dialog);
    }

    private void UpdateDialog()
    {
        dialogText.text = dialogMessages[currentMessageIndex];
        prevButton.gameObject.SetActive(currentMessageIndex > 0);
        nextButton.gameObject.SetActive(currentMessageIndex < dialogMessages.Length - 1);
        dontShowAgainButton.interactable = (currentMessageIndex == dialogMessages.Length - 1 || dialogMessages.Length == 1);
        closeButton.interactable = (currentMessageIndex == dialogMessages.Length - 1 || dialogMessages.Length == 1);
    }

    private void NextMessage()
    {
        if (currentMessageIndex < dialogMessages.Length - 1)
        {
            currentMessageIndex++;
            UpdateDialog();
        }
    }

    private void PreviousMessage()
    {
        if (currentMessageIndex > 0)
        {
            currentMessageIndex--;
            UpdateDialog();
        }
    }

    private void CloseDialog()
    {
        Debug.Log("Close Button Pressed");
        SetGameState(GameState.Playing);
        EnableHUD(true);
    }

    private void DontShowAgain()
    {
        PlayerPrefs.SetInt(dialogPrefKey, 1);
        CloseDialog();
    }

    public void EnableHUD(bool setValue)
    {
        HUD.SetActive(setValue);
    }

    public void ResetGame()
    {
        CurrentState = GameState.Resetting;
        if (retryCount < maxRetries)
        {
            retryCount++;
            Debug.Log("Retrying level. Attempt: " + retryCount);
            //StartCoroutine(ResetLevel());
            ResetLevel();
        }
        else
        {
            Debug.Log("Max retries reached. Game Over.");
            retryCount = 0;
            // Handle game over logic, e.g., show game over screen or return to main menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void ResetLevel()
    {
        StartCoroutine(GameEnd(0));
        
    }


    // The function to end the game
    private IEnumerator GameEnd(int isWin)
    {
        // Enable the text to end
        //endText.enabled = true;

        // Start the slow mo effect
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;

        // The seconds on text to show
        for (int i = 3; i >= 0; i--)
        {
            endText.text = $"Ending in {i}...";
            yield return new WaitForSecondsRealtime(1);
        }


        // Disable the text
        endText.text = "";

        // Call the reset according to win or loss
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        if (CurrentState == GameState.Resetting)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        // else no level remaining.
    }

    public void OnSpeedChange(int value)
    {
        string selectedOption = speedDropdown.GetComponent<TMP_Dropdown>().options[value].text;
        int speedMultiplier = int.Parse(selectedOption.Replace("x", ""));

        if (playerMovementScript != null)
        {
            Debug.Log("Playermovement is not null");
            playerMovementScript.SetMovementSpeed(speedMultiplier);
        }
    }

    public void LoadLevel(Level level)
    {
        TileGenerator.levelNumber = level;
        SceneManager.LoadScene("Level1"); // Replace with your scene name
    }

    private IEnumerator LoadNextLevel()
    {
        Level nextLevel = TileGenerator.levelNumber;

        // Increment the current level and load the next level
        if (TileGenerator.levelNumber != Level.Random)
        {
             nextLevel = TileGenerator.levelNumber + 1;
        }
        
        
        

        // Start the slow mo effect
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;

        // The seconds on text to show
        for (int i = 3; i >= 0; i--)
        {
            endText.text = $"Ending in {i}...";
            yield return new WaitForSecondsRealtime(1);
        }


        // Disable the text
        endText.text = "";

        // Call the reset according to win or loss
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;

        // Handle any win animations or screens here
        if (TileGenerator.levelNumber < Level.Level11)
        {
            LoadLevel(nextLevel);
        }

    }

    public void LevelComplete()
    {
        CurrentState = GameState.End;
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator ShowTextInSlowMo(string text, int seconds, bool showSeconds)
    {
        // Start the slow mo effect
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;

        // The seconds on text to show
        for (int i = seconds; i >= 0; i--)
        {
            if (showSeconds)
            {
                endText.text = text + $" {i}...";
            }
            else
            {
                endText.text = text;
            }
            yield return new WaitForSecondsRealtime(1);
        }


        // Disable the text
        endText.text = "";

        // Call the reset according to win or loss
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
    }
}
