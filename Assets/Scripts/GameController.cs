using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    // Variables to store the settings selected in the Random Game popup
    public bool includeSpecialTiles;
    public bool includeSecureTiles;
    public bool includeDetours;
    public bool limitedLives;
    public int numberOfTiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add any additional methods to manage game-wide state if necessary
}