using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public GameObject randomGameOptionsPopup;

    // Method to load a scene by name
    public void sceneLoader(string sceneName)
    {
        if (sceneName.Equals("RandomGame"))
        {
            TileGenerator.levelNumber = Level.Random;
            randomGameOptionsPopup.SetActive(true);
        }
        else
        {
            TileGenerator.levelNumber = Level.Level11;
            UnityEngine.Debug.Log("Button clicked, loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName); // Correct method name
        }
        
    }

}