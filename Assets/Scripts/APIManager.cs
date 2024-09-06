using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using Newtonsoft.Json;

[Serializable]
public class GameData
{
    public int allowedMoves;
    public int attempts;
    public bool cleared;
    public int score;  // Individual playthrough score
    public int leastMoves;
    public int requiredMoves;
    public int timeSpent; // in seconds
}

[Serializable]
public class LevelData
{
    public List<GameData> plays = new List<GameData>();  // List of all playthroughs
    public int highestScore;  // Highest score across all attempts
    public int totalTimeForLevel; // Total time spent on this level across all playthroughs
}

[Serializable]
public class NormalGameData
{
    public Dictionary<string, LevelData> levels = new Dictionary<string, LevelData>();
}

[Serializable]
public class RandomGameOptionsSet
{
    public bool specialTile;
    public bool secureTile;
    public bool detours;
    public bool limitedLives;
}

[Serializable]
public class RandomGameSession
{
    public RandomGameOptionsSet optionsUsed = new RandomGameOptionsSet();
    public int numOfTiles;
    public int timeSpent; // in seconds
    public int levelsWon;
    public int levelsAttempted;
    public Dictionary<string, GameData> games = new Dictionary<string, GameData>();
}

[Serializable]
public class UserData
{
    public string email;
    public NormalGameData normalgame = new NormalGameData();
    public Dictionary<string, RandomGameSession> randomgameSessions = new Dictionary<string, RandomGameSession>();
    public int totalGameplayTime; // in seconds
}

public class APIManager : MonoBehaviour
{
    public static APIManager API;
    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    private DatabaseReference mpHighestScoreRef;
    private EventHandler<ValueChangedEventArgs> mpHighestScoreListener;


    private UserData userData;
    private string userId;
    private string userName;

    private float levelStartTime; // To track when the level starts
    private float sessionStartTime; // To track when the random game session starts
   
    private string currentSessionId;
    private int mpHighScore;
    private int yourHighScore;

    [Header("Login")]
    public TMP_InputField LoginEmail;
    public TMP_InputField loginPassword;

    [Header("Sign up")]
    public TMP_InputField SignupEmail;
    public TMP_InputField SignupPassword;
    public TMP_InputField SignupPasswordConfirm;

    [Header("Extra")]
    public GameObject loadingScreen;
    public TextMeshProUGUI logTxt;
    public GameObject loginUi, signupUi, SuccessUi;

    // Make sure not to destory this class's instance
    // It will be used to call APIs later on
    private void Awake()
    {
        if (API != null)
            Destroy(API);
        else
            API = this;
        DontDestroyOnLoad(API);
    }

    // This time will be used to record in
    // all api calls to save any data
    void Start()
    {
        mpHighScore = 0;
        yourHighScore = 0;
        InitializeFirebase();
    }

    private void InitializeUserData()
    {
        userData = new UserData();
        SaveUserData();
        //SaveProgressPeriodically(); // Start periodic saving of user data
    }

    private void InitializeFirebase()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false); // Enable offline data persistence
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    #region ScoresFetching

    public void GetHighScoresForLevel(string levelName, Action<int, int> callback)
    {
        //int mpHighScore = 0;
        //int personalHighScore = 0;

        // Fetch MP Highest Score
        dbReference.Child("normalgame").Child(levelName).Child("MPHighestScore").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    mpHighScore = Convert.ToInt32(snapshot.Value);
                }
            }

            // Fetch Personal Highest Score
            dbReference.Child("normalgame").Child(levelName).Child("users").Child(userId).Child("highestScore").GetValueAsync().ContinueWithOnMainThread(task2 =>
            {
                if (task2.IsCompleted && !task2.IsCanceled && !task2.IsFaulted)
                {
                    DataSnapshot snapshot2 = task2.Result;
                    if (snapshot2.Exists)
                    {
                        yourHighScore = Convert.ToInt32(snapshot2.Value);
                    }
                }

                // Pass both high scores back to PlayerMovement via callback
                callback.Invoke(mpHighScore, yourHighScore);
            });
        });
    }



    public void UpdateHighScoresAfterLevel(string levelName, int currentScore)
    {
        // Check if the current score exceeds MP highest score and update if necessary
        if (currentScore > mpHighScore)
        {
            dbReference.Child("normalgame").Child(levelName).Child("MPHighestScore").SetValueAsync(currentScore).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log("MP Highest Score updated successfully");
                }
                else
                {
                    Debug.LogError("Failed to update MP Highest Score.");
                }
            });
        }

        // Check if the current score exceeds personal highest score and update if necessary
        if (currentScore >= yourHighScore)
        {
            dbReference.Child("normalgame").Child(levelName).Child("users").Child(userId).Child("highestScore").SetValueAsync(currentScore).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log("Personal Highest Score updated successfully");
                }
                else
                {
                    Debug.LogError("Failed to update personal Highest Score.");
                }
            });
        }
    }

    //public void ListenForMPHighestScoreChanges(string levelName, Action<int> onMPHighScoreUpdated)
    //{
    //    // Listen for changes in the MPHighestScore
    //    dbReference.Child("normalgame").Child(levelName).Child("MPHighestScore").ValueChanged += (object sender, ValueChangedEventArgs e) =>
    //    {
    //        if (e.DatabaseError != null)
    //        {
    //            Debug.LogError($"Failed to listen for MP highest score updates: {e.DatabaseError.Message}");
    //            return;
    //        }

    //        if (e.Snapshot.Exists)
    //        {
    //            mpHighScore = Convert.ToInt32(e.Snapshot.Value);
    //            Debug.Log("MP Highest Score updated to: " + mpHighScore);
    //            // Call the callback to update the score in UI
    //            onMPHighScoreUpdated?.Invoke(mpHighScore);
    //        }
    //    };
    //}
    // Listen for changes in MP Highest Score
    public void ListenForMPHighestScoreChanges(string levelName, Action<int> onMPHighestScoreChanged)
    {
        mpHighestScoreRef = dbReference.Child("normalgame").Child(levelName).Child("MPHighestScore");

        // Define the event listener
        mpHighestScoreListener = (sender, e) =>
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError($"Failed to listen for MP highest score updates: {e.DatabaseError.Message}");
                return;
            }

            if (e.Snapshot.Exists)
            {
                mpHighScore = Convert.ToInt32(e.Snapshot.Value);
                Debug.Log("MP Highest Score updated to: " + mpHighScore);

                // Invoke the callback with the new MPHighestScore value
                onMPHighestScoreChanged?.Invoke(mpHighScore);
            }
        };

        // Add the listener
        mpHighestScoreRef.ValueChanged += mpHighestScoreListener;
    }

    // Stop listening for MP Highest Score changes
    public void StopListeningForMPHighestScoreChanges()
    {
        if (mpHighestScoreRef != null && mpHighestScoreListener != null)
        {
            mpHighestScoreRef.ValueChanged -= mpHighestScoreListener;
            Debug.Log("Stopped listening for MP Highest Score changes.");
        }
    }

    public void UpdatePersonalHighScore(int score)
    {
        if (score > yourHighScore)
            yourHighScore = score;
    }

    //public void StopListeningForMPHighestScore(string levelName)
    //{
    //    dbReference.Child("normalgame").Child(levelName).Child("MPHighestScore").ValueChanged -= HandleMPHighestScoreChanged;
    //}

    private void HandleMPHighestScoreChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError($"Failed to listen for MP highest score updates: {e.DatabaseError.Message}");
            return;
        }

        if (e.Snapshot.Exists)
        {
            int newMPHighScore = Convert.ToInt32(e.Snapshot.Value);
            Debug.Log("MP Highest Score updated to: " + newMPHighScore);
            mpHighScore = newMPHighScore;
            // Update the high score text in the UI
            //mpHighestScoreText.text = $"MP Highest Score: {newMPHighScore}";
        }
    }
    #endregion

    public void SaveProgressPeriodically()
    {
        InvokeRepeating("SaveUserData", 30.0f, 30.0f); // Save data every 30 seconds
    }

    #region signup 
    public void SignUp()
    {
        loadingScreen.SetActive(true);

        
        string email = SignupEmail.text;
        string password = SignupPassword.text;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                loadingScreen.SetActive(false);
                showLogMsg("Try again");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                loadingScreen.SetActive(false);
                showLogMsg("Try again");
                return;
            }
            // Firebase user has been created.

            loadingScreen.SetActive(false);
            

            SignupEmail.text = "";
            SignupPassword.text = "";
            SignupPasswordConfirm.text = "";

            AuthResult newUser = task.Result;
            userId = newUser.User.UserId;
            userName = newUser.User.Email;
            //Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            //    newUser.DisplayName, userId);
            InitializeUserData(); // Initialize user data after sign up
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            showLogMsg("Sign up Successful");

        });
    }
    
    #endregion

    #region Login
    public void Login()
    {
        loadingScreen.SetActive(true);

        
        string email = LoginEmail.text;
        string password = loginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                loadingScreen.SetActive(false);
                AuthResult user = task.Result;
                userId = user.User.UserId;
                userName = user.User.Email;
                LoadUserData();
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        });




    }
    #endregion

    #region extra
    void showLogMsg(string msg)
    {
        logTxt.text = msg;
        logTxt.GetComponent<Animation>().Play("textFadeout");
    }
    #endregion


    private void LoadUserData()
    {
        dbReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    userData = JsonConvert.DeserializeObject<UserData>(snapshot.GetRawJsonValue());
                }
                else
                {
                    InitializeUserData();
                }
            }
        });
    }

    public void SaveUserData()
    {
        string json = JsonConvert.SerializeObject(userData);
        Debug.Log("Saving User Data: " + json); // Log the data being saved
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Retry after a delay if the save fails
                StartCoroutine(RetrySaveUserData(json, 2.0f)); // Retry after 2 seconds
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Data saved successfully.");
            }
        });
    }

    private IEnumerator RetrySaveUserData(string json, float delay)
    {
        yield return new WaitForSeconds(delay);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Retry save failed again. Consider further actions.");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Data saved successfully on retry.");
            }
        });
    }

    public void StartLevel()
    {
        levelStartTime = Time.time; // Capture the start time of the level
        userData.email = userName;
    }

    public void EndLevel(string levelName, GameData gameData)
    {
        float levelEndTime = Time.time;
        gameData.timeSpent = (int)(levelEndTime - levelStartTime);

        RecordNormalGameLevel(levelName, gameData);

        // Add to total gameplay time
        userData.totalGameplayTime += gameData.timeSpent;
        SaveUserData();
    }

    public void StartRandomGameSession(RandomGameOptionsSet optionsUsed, int numOfTiles)
    {
        userData.email = userName;
        // Generate a new session ID only if there isn't an ongoing session
        if (string.IsNullOrEmpty(currentSessionId))
        {
            sessionStartTime = Time.time; // Capture the start time of the random game session
            currentSessionId = Guid.NewGuid().ToString(); // Generate a unique session ID

            // Initialize a new RandomGameSession with the given options
            RandomGameSession newSession = new RandomGameSession
            {
                optionsUsed = optionsUsed,
                numOfTiles = numOfTiles,
                levelsWon = 0,
                levelsAttempted = 0,
                timeSpent = 0
            };

            // Add this session to the user's data
            userData.randomgameSessions.Add(currentSessionId, newSession);
            SaveUserData();
        }
    }


    public void EndRandomGameSession(GameData gameData)
    {
        if (!string.IsNullOrEmpty(currentSessionId))
        {
            UpdateRandomGameSession(currentSessionId, gameData, false);
            ClearCurrentSessionId();
        }
    }

    public void UpdateRandomGameSession(string sessionId, GameData gameData, bool isWin)
    {
        RandomGameSession session;

        // particular level time becomes current time minus previous start time
        gameData.timeSpent = (int)(Time.time - sessionStartTime);
        sessionStartTime = Time.time;
        if (!userData.randomgameSessions.ContainsKey(sessionId))
        {
            Debug.LogWarning("Session ID not found. Creating a new session.");
            session = new RandomGameSession();
            session.levelsAttempted = 1;
            session.levelsWon = isWin ? 1 : 0;
            session.timeSpent = gameData.timeSpent;
            session.games.Add("Level" + session.levelsAttempted, gameData);

            userData.randomgameSessions[sessionId] = session;
        }
        else
        {
            session = userData.randomgameSessions[sessionId];
            session.levelsAttempted++;
            if (isWin) session.levelsWon++;
            session.timeSpent += gameData.timeSpent;
            session.games.Add("Level" + session.levelsAttempted, gameData);
            
        }
        userData.totalGameplayTime += session.timeSpent;
        SaveUserData();
    }


    public void RecordNormalGameLevel(string level, GameData newPlayData)
    {
        // Ensure the level exists
        if (!userData.normalgame.levels.ContainsKey(level))
        {
            userData.normalgame.levels.Add(level, new LevelData());
        }

        LevelData levelData = userData.normalgame.levels[level];

        // Add the new playthrough to the list of plays for this level
        levelData.plays.Add(newPlayData);

        // Update the highest score if the new play's score is higher
        if (newPlayData.score > levelData.highestScore)
        {
            levelData.highestScore = newPlayData.score;
        }

        // Add the time spent in this playthrough to the total time for the level
        levelData.totalTimeForLevel += newPlayData.timeSpent;

    }

    public void UpdateTotalGameplayTime(int time)
    {
        userData.totalGameplayTime += time;
        SaveUserData();
    }


    public string GetCurrentSessionId()
    {
        return currentSessionId;
    }
    public void ClearCurrentSessionId()
    {
        currentSessionId = null;
    }
}
