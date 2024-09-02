using UnityEngine;
using UnityEngine.UIElements;

public enum TileType
{
    Normal,
    Frozen,
    Lava,
    SecureTile123,
    SecureTile321,
    // Add more types as needed
    First,
    Last,
}

public enum PenaltyType
{
    None,
    LoseScore,
    NoScore,
    DoubleScore,
    ApplyDamage,
    ConditionOnNormal,
    // Add more penalties as needed
}


public class Tile
{
    public TileType Type { get; private set; }
    public Material Material { get; private set; }
    public Vector3 Position { get; private set; }
    public int TileNumber { get; private set; }
    public PenaltyType Penalty;
    public bool IsSteppedOn;
    public bool PenaltyApplies;

    public Tile(TileType type, Material material, Vector3 position, int tileNumber, PenaltyType penaltyType = PenaltyType.None)
    {
        Type = type;
        Material = material;
        Position = position;
        TileNumber = tileNumber;
        IsSteppedOn = false;
        PenaltyApplies = false;
        // Set penalty based on tiletype
        SetPenalty();
    }

    private void SetPenalty()
    {
        switch (Type)
        {
            case TileType.Frozen:
            case TileType.Lava:
                Penalty = PenaltyType.LoseScore;
                break;
            case TileType.SecureTile123:
            case TileType.SecureTile321:
                Penalty = PenaltyType.NoScore;
                break;
            case TileType.Normal:
                Penalty = PenaltyType.ConditionOnNormal;
                break;
        }
    }

    // Method to determine if the player meets the condition
    public bool PlayerMeetsCondition(Condition condition)
    {
        // Logic to determine if the player meets the condition for this tile
        if (condition != null && condition.Evaluate(TileNumber))
        {
            // Additional checks can be added here based on the tileType and condition
            return true;
        }
        return false;
    }

    // Method to determine the penalty if the condition is not met
    public int ApplyPenalty(int currentScore)
    {
        // Will be true unless in default case
        PenaltyApplies = true;
        switch (Penalty)
        {
            case PenaltyType.LoseScore:
                return -20;
            case PenaltyType.NoScore:
                return 0;
            case PenaltyType.DoubleScore:
                return currentScore * 2;
            case PenaltyType.ApplyDamage:
                // Logic to apply damage to the player (e.g., reduce health)
                return currentScore; // No change to score but apply damage elsewhere
            case PenaltyType.ConditionOnNormal:
                return -30;
            default:
                PenaltyApplies = false;
                return currentScore; // No penalty
        }
    }

    // Method to determine if the player meets the condition
    public bool PlayerMeetsCondition(ActionType actionType, string actionValue)
    {
        switch (Type)
        {
            case TileType.Lava:
                // Check if the player has the correct armor (Fire Armor)
                return actionType == ActionType.ApplyArmor && actionValue == "Fire Armor";

            case TileType.Frozen:
                // Check if the player has the correct armor (Cold Armor)
                return actionType == ActionType.ApplyArmor && actionValue == "Cold Armor";

            case TileType.SecureTile123:
                // Check if the player entered the correct code "123"
                return actionType == ActionType.VerifyUsing && actionValue == "123";

            case TileType.SecureTile321:
                // Check if the player entered the correct code "321"
                return actionType == ActionType.VerifyUsing && actionValue == "321";
            default:
                // Normal tiles or others have no special condition
                // so there should be no condition on normal tile, therefore the condition is false
                return false; 
        }
    }



}
