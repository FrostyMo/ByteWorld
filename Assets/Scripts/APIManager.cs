using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;


[Serializable]
public class dataToSave
{
    public string userName;
    //public List<> totalCoins;
    public int crrLevel;
    public int highScore;//and many more

}

[Serializable]
public class LevelData
{
    public int allowedMoves;
    public int attempts;
    public bool cleared;
    public int highestScore;
    public int leastMoves;
    public int requiredMoves;
}

[Serializable]
public class GameData
{
    public Dictionary<string, LevelData> normalgame = new Dictionary<string, LevelData>();
}

[Serializable]
public class UserData
{
    public string userName;
    public GameData games = new GameData();
}

public class APIManager : MonoBehaviour
{
    public static APIManager API;
    public string userName;
    private double time;
    private DatabaseReference dbReference; // Firebase Database reference

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
        time = Time.realtimeSinceStartup;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference; // Initialize the database reference
    }

    #region signup 
    public void SignUp()
    {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
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
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            SignupEmail.text = "";
            SignupPassword.text = "";
            SignupPasswordConfirm.text = "";

            InitializeUserData(result.User.UserId); // Initialize user data after sign up
            showLogMsg("Sign up Successful");
            
            

        });
    }
    private void InitializeUserData(string userId)
    {
        UserData newUserData = new UserData();
        newUserData.userName = userId; // or use a custom username if required

        // Initialize some data if needed
        LevelData level1 = new LevelData { allowedMoves = 2, attempts = 0, cleared = false, highestScore = 0, leastMoves = 0, requiredMoves = 1 };
        newUserData.games.normalgame.Add("level1", level1);

        // Save the initial data to Firebase
        string json = JsonUtility.ToJson(newUserData);
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    #endregion

    #region Login
    public void Login()
    {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = LoginEmail.text;
        string password = loginPassword.text;

        Credential credential =
        EmailAuthProvider.GetCredential(email, password);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }
            loadingScreen.SetActive(false);
            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            if (!result.User.IsEmailVerified)
            {
                showLogMsg("Successful");

                loginUi.SetActive(false);
                //SuccessUi.SetActive(true);
                //SuccessUi.transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "Id: " + result.User.UserId;
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

}
