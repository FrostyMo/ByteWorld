using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;

public class FirebaseMigration : MonoBehaviour
{
    private DatabaseReference dbReference;

    public void Awake()
    {
        StartMigration();
    }
    // Call this function once to start the migration
    public void StartMigration()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        MigrateData();
    }

    private void MigrateData()
    {
        Debug.Log("Entering Migration");
        // Fetch the existing user data
        dbReference.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                Debug.Log("Users data received");
                DataSnapshot usersSnapshot = task.Result;

                if (usersSnapshot.Exists)
                {
                    Dictionary<string, Dictionary<string, int>> levelHighestScores = new Dictionary<string, Dictionary<string, int>>();

                    // Loop through each user
                    foreach (DataSnapshot userSnapshot in usersSnapshot.Children)
                    {
                        string userId = userSnapshot.Key;
                        Debug.Log("Search userID: " + userId);

                        // Loop through each user's levels in normalgame
                        foreach (DataSnapshot levelSnapshot in userSnapshot.Child("normalgame").Child("levels").Children)
                        {
                            string levelID = levelSnapshot.Key;
                            Debug.Log("LevelID: " + levelID);

                            // Get the user's highest score for this level
                            if (levelSnapshot.HasChild("highestScore"))
                            {
                                int userScore = int.Parse(levelSnapshot.Child("highestScore").Value.ToString());
                                Debug.Log("Userscore: " + userScore);

                                // Initialize if the level does not exist
                                if (!levelHighestScores.ContainsKey(levelID))
                                {
                                    levelHighestScores[levelID] = new Dictionary<string, int>();
                                }

                                // Store the user's highest score for the level
                                levelHighestScores[levelID][userId] = Mathf.Max(levelHighestScores[levelID].GetValueOrDefault(userId, 0), userScore);
                            }
                        }
                    }

                    // Now create the parallel normalgame structure
                    foreach (var level in levelHighestScores)
                    {
                        string levelName = level.Key;
                        Dictionary<string, int> userScores = level.Value;

                        // Find the MP highest score
                        int mpHighestScore = 0;
                        foreach (var userScore in userScores)
                        {
                            mpHighestScore = Mathf.Max(mpHighestScore, userScore.Value);
                        }

                        // Save MP highest score and user scores under the new normalgame structure
                        dbReference.Child("normalgame").Child(levelName).Child("MPHighestScore").SetValueAsync(mpHighestScore);

                        foreach (var userScore in userScores)
                        {
                            dbReference.Child("normalgame").Child(levelName).Child("users").Child(userScore.Key).Child("highestScore").SetValueAsync(userScore.Value);
                        }
                    }

                    Debug.Log("Migration completed successfully.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch user data for migration.");
            }
        });
    }
}

//[System.Serializable]
//public class GameData
//{
//    public int highestScore;
//    public int allowedMoves;
//    public int attempts;
//    public bool cleared;
//    public int leastMoves;
//    public int requiredMoves;
//    public int timeSpent; // in seconds
//}