using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public EndLevel endLevelScript;
    private bool conditionTrue = false;

    // Update is called once per frame
    void Update()
    {
        
        if (transform.position.y < -10.0f && !conditionTrue)
        {
            conditionTrue = true;
            Debug.Log("Is less than 10" + transform.position.y);
            endLevelScript.Lose();
        }
    }
}
