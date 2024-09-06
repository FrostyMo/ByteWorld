using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public EndLevel endLevelScript;

    private void OnTriggerEnter(Collider other)
    {
        
        AudioManager.instance.PlayWin();
        // Add a slight delay to ensure score is updated before ending the game
        StartCoroutine(DelayedWin());
        //endLevelScript.Win();
    }
    private IEnumerator DelayedWin()
    {
        yield return new WaitForSeconds(0.5f); // Add a half-second delay (adjust as needed)
        endLevelScript.Win();
    }
}
