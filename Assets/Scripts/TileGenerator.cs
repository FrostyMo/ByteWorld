//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Level
{
    Random = 0,
    Level1 = 1,
    Level2 = 2,
    Level3 = 3,
    Level4 = 4,
    Level5 = 5,
    Level6 = 6,
    Level7 = 7,
    Level8 = 8,
    Level9 = 9,
    Level10 = 10,
    Level11 = 11,
}

public class TileGenerator : MonoBehaviour
{
    public GameObject firstBlock; // Assign the first block prefab in the Inspector
    public GameObject tilePrefab; // Assign the tile prefab in the Inspector
    public GameObject lastBlock; // Assign the last block prefab in the Inspector
    public GameObject goalPrefab; // Assign the goal prefab in the Inspector
    public int numberOfTiles = 10; // Number of tiles to generate
    public EndLevel endLevelScript; // Assign the EndLevel script in the Inspector

    public static bool PathGenerated { get; private set; } = false; // Flag to indicate if the path is generated

    private Vector3 currentPos = Vector3.zero; // Starting position
    private Vector3 startPos = Vector3.zero;
    private HashSet<Vector3> generatedPositions = new HashSet<Vector3>(); // Tracks all occupied positions
    private List<Vector3> pathPositions = new List<Vector3>(); // Stores the sequence of path positions
    private Vector3 lastMoveDirection = Vector3.zero;

    public static Level levelNumber { get; set; } // Use enum for level selection

    private int requiredMoves;
    private int allowedMoves;

    private List<Tile> tiles = new List<Tile>(); // List to store tiles

    public List<Material> tileMaterials; // Assign materials in the Inspector

    void Start()
    {
        PathGenerated = false;
        if (GameController.Instance == null && levelNumber == Level.Random)
        {
            levelNumber = Level.Level11;
        }

        if (levelNumber != Level.Random) {
            StartCoroutine(GenerateHardCodedPath(levelNumber));
        }
        else
        {
            StartCoroutine(GeneratePathWithLoading());
        }
        
    }


    IEnumerator GenerateHardCodedPath(Level level)
    {
        GameStateManager.Instance.SetGameState(GameState.GeneratingPath);

        GenerateTilesForLevel(level);
        yield return null;

        PathGenerated = true;
        if (APIManager.API != null)
            APIManager.API.StartLevel();
        LevelManager.ShowLevelDialog(level);

        //GameStateManager.Instance.SetGameState(GameState.Playing);

        yield return new WaitUntil(() => GameStateManager.Instance.CurrentState == GameState.Playing);
    }

    void GenerateTilesForLevel(Level level)
    {
        List<Vector3> positions = LevelManager.GetLevelPositions(level);

        if (positions.Count > 0)
        {
            ProcessPath(positions);
        }
    }

    

    void ProcessPath(List<Vector3> positions)
    {
        pathPositions = positions;
        tiles.Clear();
        int i = 1;
        foreach (Vector3 pos in positions)
        {
            //TileType tileType = DetermineTileType(pos);
            TileType tileType = LevelManager.DetermineTileType(i-1);
            Material selectedMaterial = GetMaterialForType(tileType);
            if (pos == positions[0])
            {
                Instantiate(firstBlock, pos, Quaternion.identity);
            }
            else if (pos == positions[positions.Count - 1])
            {
                Instantiate(lastBlock, pos, Quaternion.identity);
                Vector3 goalPos = pos + Vector3.up; // Adjust the position if needed
                GameObject goalInstance = Instantiate(goalPrefab, goalPos, Quaternion.identity);
                Goal goalScript = goalInstance.GetComponent<Goal>();
                goalScript.endLevelScript = endLevelScript;
            }
            else
            {
                GameObject tileInstance = Instantiate(tilePrefab, pos, Quaternion.identity);
                Renderer renderer = tileInstance.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = selectedMaterial;
                }
            }
            tiles.Add(new Tile(tileType, selectedMaterial, pos, i++));
            generatedPositions.Add(pos);
        }

        CalculateMovesFromPath(positions);
    }

    void CalculateMovesFromPath(List<Vector3> pathPositions)
    {
        List<string> moveList = new List<string>();
        Vector3 currentDirection = Vector3.forward;

        for (int i = 1; i < pathPositions.Count; i++)
        {
            Vector3 moveDirection = (pathPositions[i] - pathPositions[i - 1]).normalized;

            if (moveDirection != currentDirection)
            {
                string turn = GetTurnDirection(currentDirection, moveDirection);
                if (!string.IsNullOrEmpty(turn))
                {
                    moveList.Add(turn);
                    currentDirection = moveDirection;
                }
            }

            moveList.Add("W");
        }

        CalculateMoves(moveList);
    }

    IEnumerator GeneratePathWithLoading()
    {
        
        Debug.Log("In GeneratePathWithLoading with game state: " + GameStateManager.Instance);
        numberOfTiles = GameController.Instance.numberOfTiles;
        GameStateManager.Instance.SetGameState(GameState.GeneratingPath);
        Debug.Log("In GeneratePathWithLoading with game state: " + GameStateManager.Instance);
        // Calculate the number of special tiles and secure tiles based on the number of tiles and player choices
        int numSpecialTiles = CalculateSpecialTileLimit(numberOfTiles);
        int numSecureTiles = CalculateSecureTileLimit(numberOfTiles);

        // Generate the path
        yield return StartCoroutine(GenerateTilesWithRetry(numberOfTiles, numSpecialTiles, numSecureTiles));

        PathGenerated = true; // Set the flag to true when path is generated

        GameStateManager.Instance.SetGameState(GameState.Playing);
        Debug.Log("Out GeneratePathWithLoading with game state: " + GameStateManager.Instance);
    }

    public IEnumerator GenerateTilesWithRetry(int n, int numSpecialTiles, int numSecureTiles)
    {
        PathGenerated = false; // Reset the flag
        bool pathGenerated = false;

        while (!pathGenerated)
        {
            pathPositions.Clear();
            generatedPositions.Clear();
            tiles.Clear();
            currentPos = startPos; // Reset to starting position
            lastMoveDirection = Vector3.zero; // Reset last move direction

            Debug.Log("Attempting to generate path...");
            pathGenerated = GenerateTiles(n, numSpecialTiles, numSecureTiles);
            if (!pathGenerated)
            {
                Debug.LogWarning("No valid moves available. Regenerating path...");
                yield return null; // Wait for the next frame before retrying
            }
        }

        InstantiateTiles();

        PathGenerated = true; // Set the flag to true when path is generated
        Debug.Log("Path generation complete.");
    }

    bool GenerateTiles(int n, int numSpecialTiles, int numSecureTiles)
    {
        generatedPositions.Add(currentPos); // Add the starting position
        pathPositions.Add(currentPos);

        List<string> moveList = new List<string>(); // Track move list

        Vector3 currentDirection = Vector3.forward; // Assuming the player starts facing forward
        tiles.Add(new Tile(TileType.First, null, currentPos, 1));

        List<int> specialTileIndices = SelectRandomIndices(n, numSpecialTiles);
        List<int> secureTileIndices = SelectRandomIndices(n, numSecureTiles, true, specialTileIndices);


        for (int i = 0; i < n - 1; i++) // Already placed the starting tile
        {
            List<Vector3> validMoves = GetValidMoves();

            if (validMoves.Count == 0)
            {
                Debug.LogWarning("No valid moves available. Path generation stopped early.");
                return false; // Path generation failed
            }

            Vector3 nextPos = validMoves[Random.Range(0, validMoves.Count)];

            Vector3 moveDirection = (nextPos - currentPos).normalized; // Normalize the move direction

            //Debug.LogWarning("Move direction: " + moveDirection + " and curr direction: " + currentDirection);

            // Calculate turn direction if needed
            if (moveDirection != currentDirection)
            {
                string turn = GetTurnDirection(currentDirection, moveDirection);
                if (!string.IsNullOrEmpty(turn))
                {
                    moveList.Add(turn);
                    currentDirection = moveDirection;
                }
            }

            // Add forward move
            moveList.Add("W");

            generatedPositions.Add(nextPos); // Mark position as occupied
            pathPositions.Add(nextPos); // Add position to path

            //moveList.Add(GetDirectionFromMove(lastMoveDirection, nextPos - currentPos));

            // Create and store the tile
            //TileType tileType = DetermineTileTypeForRandomPath(i);
            TileType tileType = TileType.Normal;
            if (specialTileIndices.Contains(i))
            {
                tileType = DetermineSpecialTileType();
            }
            else if (secureTileIndices.Contains(i))
            {
                tileType = DetermineSecureTileType(i, secureTileIndices);
            }
            Material selectedMaterial = GetMaterialForType(tileType);
            tiles.Add(new Tile(tileType, selectedMaterial, nextPos, i+2));

            lastMoveDirection = nextPos - currentPos;
            currentPos = nextPos;
        }

        CalculateMoves(moveList);
        return true; // Path generation successful
    }

    private List<int> SelectRandomIndices(int totalTiles, int numTiles, bool batchForSecure = false, List<int> avoidIndices = null)
    {
        List<int> indices = new List<int>();
        HashSet<int> usedIndices = avoidIndices != null ? new HashSet<int>(avoidIndices) : new HashSet<int>();

        while (indices.Count < numTiles)
        {
            int randIndex = Random.Range(1, totalTiles - 1);
            if (!usedIndices.Contains(randIndex))
            {
                indices.Add(randIndex);
                usedIndices.Add(randIndex);

                // For secure tiles, group them together if needed
                if (batchForSecure && indices.Count < numTiles)
                {
                    int nextBatchIndex = randIndex + 1;
                    if (!usedIndices.Contains(nextBatchIndex))
                    {
                        indices.Add(nextBatchIndex); // Adjust to group together
                        usedIndices.Add(nextBatchIndex);
                    }
                }
            }
        }
        return indices;
    }

    string GetTurnDirection(Vector3 currentDirection, Vector3 newDirection)
    {
        currentDirection = currentDirection.normalized;
        newDirection = newDirection.normalized;

        if (newDirection == Vector3.forward)
        {
            if (currentDirection == Vector3.left)
                return "D";
            else if (currentDirection == Vector3.right)
                return "A";
            else if (currentDirection == Vector3.back)
                return "D D"; // Turn twice to face forward
        }
        else if (newDirection == Vector3.back)
        {
            if (currentDirection == Vector3.left)
                return "A";
            else if (currentDirection == Vector3.right)
                return "D";
            else if (currentDirection == Vector3.forward)
                return "D D"; // Turn twice to face backward
        }
        else if (newDirection == Vector3.left)
        {
            if (currentDirection == Vector3.forward)
                return "A";
            else if (currentDirection == Vector3.back)
                return "D";
            else if (currentDirection == Vector3.right)
                return "A A"; // Turn twice to face left
        }
        else if (newDirection == Vector3.right)
        {
            if (currentDirection == Vector3.forward)
                return "D";
            else if (currentDirection == Vector3.back)
                return "A";
            else if (currentDirection == Vector3.left)
                return "D D"; // Turn twice to face right
        }

        return "";
    }


    void CalculateMoves(List<string> moveList)
    {
        requiredMoves = 0;
        allowedMoves = 0;

        int consecutiveForwards = 0;

        for (int i = 0; i < moveList.Count; i++)
        {
            string move = moveList[i];

            if (move == "W")
            {
                consecutiveForwards++;
            }
            else
            {
                if (consecutiveForwards > 1)
                {
                    requiredMoves++; // Count the loop as 1 move
                    allowedMoves += consecutiveForwards; // Count each forward move
                }
                else
                {
                    requiredMoves += consecutiveForwards;
                    allowedMoves += consecutiveForwards;
                }

                consecutiveForwards = 0;

                if (move.Contains("D D") || move.Contains("A A"))
                {
                    requiredMoves += 2;
                    allowedMoves +=2 ;
                }
                else
                {
                    requiredMoves += 1;
                    allowedMoves += 1;
                }

                
            }
        }

        // Handle any remaining forward moves
        if (consecutiveForwards > 1)
        {
            requiredMoves++; // Count the loop as 1 move
            allowedMoves += consecutiveForwards; // Count each forward move
        }
        else
        {
            requiredMoves += consecutiveForwards;
            allowedMoves += consecutiveForwards;
        }
        FindObjectOfType<PlayerMovement>().SetMovesAllowed(allowedMoves, requiredMoves);
        //PlayerMovement.requiredMoves = requiredMoves;
        //PlayerMovement.allowedMoves = allowedMoves;
    }


    void InstantiateTiles()
    {
        // Instantiate the first block
        Instantiate(firstBlock, pathPositions[0], Quaternion.identity);

        // Instantiate the rest of the path
        for (int i = 1; i < pathPositions.Count - 1; i++)
        {
            GameObject tileInstance = Instantiate(tilePrefab, pathPositions[i], Quaternion.identity);
            Renderer renderer = tileInstance.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = tiles[i].Material;
            }
            Debug.Log("Tile# " + i + "... Material " + tiles[i].Material);
        }

        // Instantiate the last block and goal
        Vector3 lastPosition = pathPositions[pathPositions.Count - 1];
        Instantiate(lastBlock, lastPosition, Quaternion.identity);
        Vector3 goalPos = lastPosition + Vector3.up; // Adjust the position if needed
        GameObject goalInstance = Instantiate(goalPrefab, goalPos, Quaternion.identity);
        Goal goalScript = goalInstance.GetComponent<Goal>();
        goalScript.endLevelScript = endLevelScript; // Assign the EndLevel script
    }

    List<Vector3> GetValidMoves()
    {
        List<Vector3> possiblePositions = new List<Vector3>
        {
            currentPos + Vector3.forward * 6,
            currentPos + Vector3.back * 6,
            currentPos + Vector3.left * 6,
            currentPos + Vector3.right * 6
        };

        List<Vector3> validPositions = new List<Vector3>();

        foreach (Vector3 pos in possiblePositions)
        {
            if (!generatedPositions.Contains(pos) && !IsBackwardMove(pos) && HasOnlyOneNeighbor(pos))
            {
                validPositions.Add(pos);
            }
        }

        return validPositions;
    }

    bool IsBackwardMove(Vector3 nextPos)
    {
        Vector3 moveDirection = nextPos - currentPos;
        return moveDirection == -lastMoveDirection; // Prevent direct retracing
    }

    bool HasOnlyOneNeighbor(Vector3 position)
    {
        int neighborCount = 0;
        Vector3[] neighbors = new Vector3[]
        {
            position + Vector3.forward * 6,
            position + Vector3.back * 6,
            position + Vector3.left * 6,
            position + Vector3.right * 6
        };

        foreach (Vector3 neighbor in neighbors)
        {
            if (generatedPositions.Contains(neighbor))
            {
                neighborCount++;
            }
        }

        return neighborCount <= 1;
    }

    void PrintList(List<string> list)
    {
        foreach(var x in list)
        {
            Debug.Log(x.ToString());
        }
    }

    Material GetMaterialForType(TileType tileType)
    {
        int index = (int)tileType;

        //Debug.Log("Tile Type and index" + index + tileType);

        if (index >= 0 && index < tileMaterials.Count)
        {
            return tileMaterials[index];
        }

        // Fallback to the first material or a default if out of range
        return tileMaterials.Count > 0 ? tileMaterials[0] : null;
    }

    TileType DetermineTileType(Vector3 position)
    {
        //return TileType.Normal;
        // Example logic to determine tile type based on position or other factors
        // You can customize this logic to suit your game's needs
        if (tiles.Count == 2)
        {
            return TileType.Frozen;
        }
        else if (tiles.Count == 4)
        {
            return TileType.Lava;
        }
        else if (tiles.Count >= 5)
        {
            return TileType.SecureTile123;
        }
        else
        {
            return TileType.Normal;
        }

    }

    private TileType DetermineSpecialTileType()
    {
        // Randomly select between Lava and Freeze
        return (Random.Range(0, 2) == 0) ? TileType.Lava : TileType.Frozen;
    }

    private TileType DetermineSecureTileType(int index, List<int> secureTileIndices)
    {
        // Check if the current index is the first in its batch, to decide the type
        if (index == secureTileIndices[0] || index == secureTileIndices[secureTileIndices.IndexOf(index) - 1])
        {
            // Randomly select between SecureTile123 and SecureTile321
            return (Random.Range(0, 2) == 0) ? TileType.SecureTile123 : TileType.SecureTile321;
        }
        else
        {
            // Ensure consistent type within a batch
            return secureTileIndices[0] == ((int)TileType.SecureTile123) ? TileType.SecureTile123 : TileType.SecureTile321;
        }
    }

    TileType DetermineTileTypeForRandomPath(int position)
    {
        // Custom logic based on user preferences for random tile types
        if (GameController.Instance.includeSpecialTiles && ShouldPlaceSpecialTile(position))
        {
            return TileType.Lava; // Example special tile type
        }
        else if (GameController.Instance.includeSecureTiles && ShouldPlaceSecureTile(position))
        {
            return TileType.SecureTile123; // Example secure tile type
        }
        else if (GameController.Instance.includeDetours && ShouldPlaceDetour(position))
        {
            return TileType.Normal; // Implement detour logic
        }
        else
        {
            return TileType.Normal; // Default to normal tile
        }
    }

    // Method to retrieve a tile by its position
    public Tile GetTileAtPosition(Vector3 position)
    {
        if (position.y > 0 && position.y < 5)
        {
            position.y = 0;
        }
        foreach (Tile tile in tiles)
        {
            if (tile.Position == position)
            {
                return tile;
            }
        }
        return null; // Return null if no tile is found at the position
    }

    private bool ShouldPlaceSpecialTile(int currentIndex)
    {
        // Logic to decide if a special tile should be placed at the current index
        int specialTileChance = Mathf.Clamp(2 + (numberOfTiles - 20) / 5, 2, 15);
        return Random.Range(0, numberOfTiles) < specialTileChance;
    }

    private bool ShouldPlaceSecureTile(int currentIndex)
    {
        // Logic to decide if a secure tile should be placed at the current index
        int secureTileChance = Mathf.Clamp(2 + (numberOfTiles - 20) / 5, 2, 15);
        return Random.Range(0, numberOfTiles) < secureTileChance;
    }

    private bool ShouldPlaceDetour(int currentIndex)
    {
        // Logic to decide if a detour should be placed at the current index
        int detourChance = Mathf.Clamp(2 + (numberOfTiles - 20) / 5, 2, 15);
        return Random.Range(0, numberOfTiles) < detourChance;
    }


    private int CalculateSpecialTileLimit(int totalTiles)
    {
        if (!GameController.Instance.includeSpecialTiles) return 0;

        // Simple logic: 1 special tile per 10 tiles, adjusted for totalTiles
        return Mathf.Clamp(totalTiles / 10, 2, 15);
    }

    private int CalculateSecureTileLimit(int totalTiles)
    {
        if (!GameController.Instance.includeSecureTiles) return 0;

        // Secure tiles should be less frequent than special tiles, e.g., 1 secure tile per 15 tiles
        return Mathf.Clamp(totalTiles / 15, 2, 10);
    }

}
