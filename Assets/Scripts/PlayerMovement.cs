using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.CompilerServices;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody player;
    private Renderer playerRenderer; // Reference to the player's Renderer component
    public List<Material> armorMaterials; // List of armor materials to set from editor
    private Material currentArmorMaterial; // Current armor material

    public float movementSpeed = 3.7f;
    public Animator animator;
    public GameObject parent;
    private Rigidbody parentBody;

    public static int allowedMoves = 5; // Total allowed moves per level
    public static int requiredMoves = 2; // These are minimum number of moves the level will have (only for non random levels).

    private int movesTaken = 0; // Moves taken by the player
    private int totalSteps = 0; // Moves but loop steps are counted full too
    private int currentTileNumber = 0; // Add this field to track the current tile number
    private int totalScore = 0;
    private int scoreStreak = 1;


    public TextMeshProUGUI movesText; // UI Text to display moves
    public TextMeshProUGUI scoreText; // UI Text to display score
    public TextMeshProUGUI requiredMovesText; // UI Text to display moves
    public TextMeshProUGUI passCodeText; // UI Text to display moves

    //static private List<string> movements = new List<string>();
    private List<Move> movements = new List<Move>();
    private bool isResetting = false; // Flag to ensure reset is called once
    private bool isExecutingMovements = false;
    public float moveDuration = 0.5f; // Duration for each move

    private HashSet<Vector3> visitedPositions = new HashSet<Vector3>();
    private Vector3 currentPos;

    private List<Condition> conditions = new List<Condition>();


    private VariableBinder variableBinder;
    private TileGenerator tileGenerator;

    private Coroutine scoreAnimationCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        currentPos = parent.transform.position;
        visitedPositions.Clear();
        visitedPositions.Add(currentPos);
        movesTaken = 0;
        movements.Clear();
        parentBody = parent.GetComponent<Rigidbody>();
        // Get the VariableBinder component
        variableBinder = GetComponent<VariableBinder>();
        tileGenerator = FindObjectOfType<TileGenerator>();

        UpdateMovesText();
        //CalculateScore(false, false, false, false);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.Instance.IsGamePaused())
        {
            return;
        }

        Move();

    }

    private void Move()
    {
        if (isExecutingMovements)
        {
            return; // Ignore inputs while executing movements
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Return Pressed");
            StartCoroutine(ExecuteMovements());
        }
    }

    private IEnumerator ExecuteMovements()
    {
        isExecutingMovements = true;
        parent.GetComponent<Rigidbody>().useGravity = false; // Disable gravity during movement
        GameStateManager.Instance.SetGameState(GameState.ExecutingMovements);

        Debug.Log("Inside Execute Movements. Move counts: " + movements.Count);
        while (movements.Count > 0)
        {
            //string move = movements[0];
            Move currentMove = movements[0];
            movements.RemoveAt(0);

            //if (move == "W")
            //{
            //    yield return StartCoroutine(MoveForward());
            //}
            //else if (move == "D")
            //{
            //    yield return StartCoroutine(TurnRight());
            //}
            //else if (move == "A")
            //{
            //    yield return StartCoroutine(TurnLeft());
            //}
            //totalSteps++;
            for (int i = 0; i < currentMove.Count; i++)
            {
                if (currentMove.Direction == "W")
                {
                    yield return StartCoroutine(MoveForward());
                }
                else if (currentMove.Direction == "D")
                {
                    yield return StartCoroutine(TurnRight());
                }
                else if (currentMove.Direction == "A")
                {
                    yield return StartCoroutine(TurnLeft());
                }
                totalSteps++;
            }

            // Mark the move as executed
            currentMove.IsExecuted = true;
        }

        parent.GetComponent<Rigidbody>().useGravity = true; // Re-enable gravity after movement
        isExecutingMovements = false;

        if (GameStateManager.Instance.CurrentState != GameState.Resetting)
            GameStateManager.Instance.SetGameState(GameState.Playing);
    }



    public void SetConditions(List<Condition> newConditions)
    {
        conditions = newConditions;
    }


    private IEnumerator MoveForward()
    {
        parentBody.useGravity = false;
        Vector3 startPos = parent.transform.position;
        Vector3 endPos = startPos + parent.transform.forward * 6;
        Debug.Log("Moving to position: " + endPos.ToString());
        if (visitedPositions.Contains(endPos))
        {
            Debug.Log("Cannot move back to a previously visited tile!");
            yield break; // Stop movement if the tile was visited before
        }


        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            parent.transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parent.transform.position = endPos;
        visitedPositions.Add(endPos); // Mark the new position as visited
        currentPos = endPos;
        currentTileNumber++; // Increment the tile number when the player moves forward


        // Determine the most recent applicable condition and apply it
        Condition applicableCondition = GetApplicableCondition(currentTileNumber);

        Tile currentTile = GetTileAtPosition(endPos);
        if (currentTile != null)
        {
            if (applicableCondition != null)
            {
                ApplyAction(applicableCondition);
                bool conditionMet = currentTile.PlayerMeetsCondition(applicableCondition.ActionType, applicableCondition.ActionValue);
                if (conditionMet)
                {
                    if (currentTile.Type != TileType.Normal)
                    {
                        scoreStreak++;
                    }
                }
                else
                {
                    scoreStreak = 1;
                    currentTile.PenaltyApplies = true;
                }
            }
            else
            {
                // there is no condition on the tile - it must not be special
                if (currentTile != null && currentTile.Type != TileType.Normal)
                {
                    currentTile.PenaltyApplies = true;
                }
                RevertToNormalArmor();
            }
        }
        

        

        UpdateMovesText();
        // Call the updated CalculateScore method
 
        CalculateTileScore(currentTile);


        CheckMoveLimit();
        parentBody.useGravity = true;
    }

    private IEnumerator TurnRight()
    {
        Quaternion startRotation = parent.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90, 0);

        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            parent.transform.rotation = Quaternion.Lerp(startRotation, endRotation, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parent.transform.rotation = endRotation;

        

        UpdateMovesText();
        // Call the updated CalculateScore method
        //CalculateScore(isSpecialTile, correctArmor, wrongArmor, noArmor);
        Tile currentTile = GetTileAtPosition(currentPos);
        CalculateTileScore(currentTile);
        CheckMoveLimit();
    }

    private IEnumerator TurnLeft()
    {
        Quaternion startRotation = parent.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, -90, 0);

        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            parent.transform.rotation = Quaternion.Lerp(startRotation, endRotation, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parent.transform.rotation = endRotation;
        
        

        UpdateMovesText();
        // Call the updated CalculateScore method
        //CalculateScore(isSpecialTile, correctArmor, wrongArmor, noArmor);
        Tile currentTile = GetTileAtPosition(currentPos);
        CalculateTileScore(currentTile);
        CheckMoveLimit();
    }

    private void UpdateMovesText()
    {
        if (movesText != null)
        {
            movesText.text = "Moves: " + movesTaken + "/" + allowedMoves;
        }
        if (requiredMovesText != null)
        {
            requiredMovesText.text = "Required moves: " + requiredMoves;
        }
    }

    public void CalculateTileScore(Tile currentTile)
    {
        // Give more score if moves within required moves
        // @movesTaken stores the total moves REGISTERED by player
        // movements.Count reduces after every succesful move MADE 
        int baseScore = (movesTaken - movements.Count <= requiredMoves) ? 50 : 10; // Base score depending on whether the move is within required moves
        int scoreChange = baseScore; // Initialize score change with the base score

        if (currentTile != null ) 
        {
            if (currentTile.PenaltyApplies)
            {
                scoreChange = currentTile.ApplyPenalty(scoreChange); // Apply the penalty if necessary
            }
            else if (currentTile.Type != TileType.Normal)
            {
                scoreChange *= scoreStreak; // Apply score streak multiplier for successful conditions
            }
        }
        else
        {
            scoreChange = 0;
            scoreStreak = 1;
        }

        totalScore += scoreChange; // Update the total score

        // Ensure the total score doesn't go below zero
        totalScore = Mathf.Max(totalScore, 0);

        // Stop any existing animation before starting a new one
        if (scoreAnimationCoroutine != null)
        {
            StopCoroutine(scoreAnimationCoroutine);
        }

        // Start the score animation
        scoreAnimationCoroutine = StartCoroutine(AnimateScore(totalScore));
    }



    private IEnumerator AnimateScore(int targetScore)
    {
        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text.Replace("Score: ", ""));

            // Continue animating while the score has not reached the target
            while (currentScore != targetScore)
            {
                // Calculate the difference between target and current score
                int difference = Mathf.CeilToInt((targetScore - currentScore) / 10.0f);

                // Ensure difference is at least 1 in magnitude to move towards target
                if (difference == 0)
                {
                    difference = targetScore > currentScore ? 1 : -1;
                }

                // Adjust the current score by the calculated difference
                currentScore += difference;

                // Update the score text
                scoreText.text = "Score: " + currentScore;
                yield return new WaitForSeconds(0.05f);

                // Ensure the score does not overshoot
                if ((difference > 0 && currentScore > targetScore) || (difference < 0 && currentScore < targetScore))
                {
                    currentScore = targetScore;
                }
            }

            // Finalize the score text to ensure it's set correctly
            scoreText.text = "Score: " + targetScore;
        }
    }

    public void AddMovement(string move, int count)
    {
        //Debug.Log("Move: " + move + " and count: " + count);
        //for (int i = 0; i < count; i++)
        //{
        //    movements.Add(move);
        //}
        //Debug.Log("Move added to movements: " + movements.Count);

        //movesTaken++; // Each loop counts as one move
        //UpdateMovesText();

        Debug.Log("Move: " + move + " and count: " + count);
        movements.Add(new Move(move, count));
        Debug.Log("Move added to movements: " + movements.Count);

        movesTaken++; // Increment movesTaken only for non-looped moves
        UpdateMovesText();
    }

    private void CheckMoveLimit()
    {
        if (movesTaken > allowedMoves && GameStateManager.Instance.CurrentState != GameState.Resetting)
        {

            GameStateManager.Instance.ResetGame();
        }
    }

    public void SetMovementSpeed(float speedMultiplier)
    {
        Debug.Log("Speed changed to x" + speedMultiplier);
        moveDuration = 0.5f / speedMultiplier;
    }


    private void ApplyAction(Condition applicableCondition)
    {
        TileType tileType = TileType.Normal; // Default to Normal


        switch (applicableCondition.ActionType)
        {
            case ActionType.ApplyArmor:

                if (applicableCondition.ActionValue == "Cold Armor")
                {
                    Debug.Log("Putting Cold Armor");
                    //playerRenderer.material = GetArmorMaterial(TileType.Frozen);
                    tileType = TileType.Frozen;
                }
                else if (applicableCondition.ActionValue == "Fire Armor")
                {
                    Debug.Log("Putting Fire ARmor");
                    //playerRenderer.material = GetArmorMaterial(TileType.Lava);
                    //currentArmor = SpecialTile.TileType.Lava;
                    tileType = TileType.Lava;
                }
                break;
            case ActionType.TakeDetour:
                StartCoroutine(TakeDetour(applicableCondition.ActionValue));
                break;
            case ActionType.VerifyUsing:
                // not much to do here except put a string above - penalty in calculation of score instead
                // if later need to do something with VerifyUsing, can do here
                if (applicableCondition.ActionValue != null)
                {
                    passCodeText.text = "Passcode: " + applicableCondition.ActionValue;
                }
                break;
        }
        // Get the armor material based on the tile type and apply it
        currentArmorMaterial = GetArmorMaterial(tileType);

        if (playerRenderer != null && currentArmorMaterial != null)
        {
            playerRenderer.material = currentArmorMaterial;
        }
    }

    private IEnumerator TakeDetour(string detourType)
    {
        if (detourType.StartsWith("Left"))
        {
            yield return StartCoroutine(TurnLeft());
        }
        else if (detourType.StartsWith("Right"))
        {
            yield return StartCoroutine(TurnRight());
        }

        int steps = 1;
        if (detourType.Length > 5) // Check if it's more than just "Left" or "Right"
        {
            char variable = detourType[5];
            switch (variable)
            {
                case 'X':
                    steps = variableBinder.GetVariableX();
                    break;
                case 'Y':
                    steps = variableBinder.GetVariableY();
                    break;
                case 'Z':
                    steps = variableBinder.GetVariableZ();
                    break;
            }
        }

        for (int i = 0; i < steps; i++)
        {
            yield return StartCoroutine(MoveForward());
        }

        if (detourType.StartsWith("Left"))
        {
            yield return StartCoroutine(TurnRight());
        }
        else if (detourType.StartsWith("Right"))
        {
            yield return StartCoroutine(TurnLeft());
        }
    }


    // Method to get the appropriate armor material based on the tile type
    private Material GetArmorMaterial(TileType tileType)
    {
        int index = (int)tileType;
        if (index >= 0 && index < armorMaterials.Count)
        {
            return armorMaterials[index];
        }
        return null;
    }

    private void RevertToNormalArmor()
    {
        if (playerRenderer != null && GetArmorMaterial(TileType.Normal) != null)
        {
            playerRenderer.material = GetArmorMaterial(TileType.Normal);
        }
        passCodeText.text = "";
    }

    private Condition GetApplicableCondition(int currentTileNumber)
    {
        Condition latestCondition = null;

        foreach (Condition condition in conditions)
        {
            if (condition.Evaluate(currentTileNumber))
            {
                if (latestCondition == null || IsHigherPriority(condition, latestCondition))
                {
                    latestCondition = condition;
                }
            }
        }

        return latestCondition;
    }

    private bool IsHigherPriority(Condition newCondition, Condition existingCondition)
    {
        if (newCondition.ConditionType == ConditionType.MoveEquals)
        {
            return true; // Equals has the highest priority
        }

        if (existingCondition.ConditionType == ConditionType.MoveEquals)
        {
            return false; // Existing Equals condition has higher priority
        }

        // If both are less than or greater than, the most recent one has priority
        return true;
    }


    // Access tilegenerator script to get tile
    private TileType GetTileTypeAtPosition(Vector3 position)
    {
        Tile tile = tileGenerator.GetTileAtPosition(position);
        if (tile != null)
        {
            return tile.Type; // Return the tile type
        }
        return TileType.Normal; // Default to Normal if no tile found
    }

    // Access tilegenerator script to get tile
    private Tile GetTileAtPosition(Vector3 position)
    {
        return tileGenerator.GetTileAtPosition(position);
        
    }

    private (bool, bool, bool, bool) DetermineSpecialFeatures(Vector3 pos)
    {
        bool isSpecialTile = false;
        bool correctArmor = false;
        bool wrongArmor = false;
        bool noArmor = true;
        TileType nextTileType = GetTileTypeAtPosition(pos);
        if (nextTileType == TileType.Frozen || nextTileType == TileType.Lava)
        {
            isSpecialTile = true;

            // Determine if the player has the correct armor
            if (nextTileType == TileType.Frozen && currentArmorMaterial == GetArmorMaterial(TileType.Frozen))
            {
                correctArmor = true;
                noArmor = false;
            }
            else if (nextTileType == TileType.Lava && currentArmorMaterial == GetArmorMaterial(TileType.Lava))
            {
                correctArmor = true;
                noArmor = false;
            }
            
            else if (currentArmorMaterial != null)
            {
                wrongArmor = true;
                noArmor = false;
            }
        }
        return (isSpecialTile, correctArmor, wrongArmor, noArmor);
    }
}
