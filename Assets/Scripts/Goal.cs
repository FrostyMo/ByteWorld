using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public EndLevel endLevelScript;

    private void OnTriggerEnter(Collider other)
    {
        
        AudioManager.instance.PlayWin();
            
        endLevelScript.Win();
    }
}
