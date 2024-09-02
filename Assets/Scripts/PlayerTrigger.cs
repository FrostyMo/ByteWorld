using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public EndLevel endLevelScript;

    // Update is called once per frame
    void Update()
    {
        
        if (transform.position.y < -10.0f)
        {
            Debug.Log("Is less than 10" + transform.position.y);
            endLevelScript.Lose();
        }
    }
}
