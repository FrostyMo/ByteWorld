using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrowDisplay : MonoBehaviour
{
    public GameObject arrowContainer;
    public GameObject upArrowPrefab;
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;
    public float arrowSize = 50f; // Public variable to adjust arrow size from the editor
    public int maxArrowsPerRow = 10; // Maximum number of arrows per row before wrapping to the next line

    public TMP_InputField loopInputField; // UI Input Field for loop count
    public Button submitButton; // Button to submit the loop count
    public TextMeshProUGUI arrowTextPrefab; // Prefab for loop count text

    private List<GameObject> arrowImages = new List<GameObject>();

    private PlayerMovement playerMovement;

    private VariableBinder variableBinder;

    private bool isLooping = false;
    private int loopCount = 1;

    private void Start()
    {

        playerMovement = FindObjectOfType<PlayerMovement>();

        // Hide the loop input field initially
        loopInputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);

        // Add listener to the submit button
        submitButton.onClick.AddListener(OnSubmitLoopCount);

        // Get the VariableBinder component
        variableBinder = GetComponent<VariableBinder>();
    }

    void Update()
    {
        if (GameStateManager.Instance.IsGamePaused())
        {
            return;
        }

        if (isLooping)
        {
            // If looping, wait for user to input loop count
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                // Show the loop input field and wait for user input
                isLooping = true;
                loopInputField.gameObject.SetActive(true);
                submitButton.gameObject.SetActive(true);
                loopInputField.ActivateInputField();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Forward Pressed");
                AddArrow(upArrowPrefab, 1);
                playerMovement.AddMovement("W", 1);
                
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddArrow(leftArrowPrefab, 1);
                playerMovement.AddMovement("A", 1);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                AddArrow(rightArrowPrefab, 1);
                playerMovement.AddMovement("D", 1);
            }

            // Variable based movement
            if (Input.GetKeyDown(KeyCode.X))
            {
                int xMoves = variableBinder.GetVariableX();
                AddArrow(upArrowPrefab, xMoves);
                playerMovement.AddMovement("W", xMoves);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                int yMoves = variableBinder.GetVariableY();
                AddArrow(upArrowPrefab, yMoves);
                playerMovement.AddMovement("W", yMoves);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                int zMoves = variableBinder.GetVariableZ();
                AddArrow(upArrowPrefab, zMoves);
                playerMovement.AddMovement("W", zMoves);
            }
        }
    }

    void AddArrow(GameObject arrowPrefab, int count)
    {
        GameObject newArrow = Instantiate(arrowPrefab, arrowContainer.transform);
        arrowImages.Add(newArrow);

        // Adjust the size of the new arrow
        RectTransform rectTransform = newArrow.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(arrowSize, arrowSize);

        // Calculate the position of the new arrow
        int row = arrowImages.Count / maxArrowsPerRow;
        int column = arrowImages.Count % maxArrowsPerRow;
        rectTransform.anchoredPosition = new Vector2(column * arrowSize, -row * arrowSize);

        // Add text to show loop count if greater than 1
        if (count > 1)
        {
            TextMeshProUGUI loopText = Instantiate(arrowTextPrefab, arrowContainer.transform);
            loopText.text = "x" + count;
            RectTransform textRectTransform = loopText.GetComponent<RectTransform>();

            // Set the text size to be smaller than the arrow size
            textRectTransform.sizeDelta = new Vector2(arrowSize / 2, arrowSize / 2);
            loopText.fontSize = 24; // Adjust the font size as necessary

            // Position the text at the top right corner of the arrow
            textRectTransform.anchoredPosition = new Vector2(column * arrowSize + arrowSize / 2, -row * arrowSize - arrowSize / 2);
        }
    }

    

    void OnSubmitLoopCount()
    {
        if (int.TryParse(loopInputField.text, out loopCount) && loopCount > 0)
        {
            AddArrow(upArrowPrefab, loopCount);
            playerMovement.AddMovement("W", loopCount);
        }

        // Hide the loop input field after submission
        loopInputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        loopInputField.text = "";
        isLooping = false;
    }
}
