public enum ConditionType { MoveEquals, MoveGreaterThanE, MoveLessThanE, Between }
public enum ActionType { ApplyArmor, TakeDetour, VerifyUsing}

[System.Serializable]
public class Condition
{
    public ConditionType ConditionType;
    public int ConditionValue;
    public int ConditionValue2 { get; private set; } // This is for Between ConditionType
    public ActionType ActionType;
    public string ActionValue { get; private set; } // Add action value

    public Condition(ConditionType conditionType, int conditionValue, int conditionValue2, ActionType actionType, string actionValue)
    {
        this.ConditionType = conditionType;
        this.ConditionValue = conditionValue;
        this.ConditionValue2 = conditionValue2;
        this.ActionType = actionType;
        this.ActionValue = actionValue;
    }


    public bool Evaluate(int currentTileNumber)
    {
        switch (ConditionType)
        {
            case ConditionType.MoveEquals:
                return currentTileNumber == ConditionValue;
            case ConditionType.MoveLessThanE:
                return currentTileNumber <= ConditionValue;
            case ConditionType.MoveGreaterThanE:
                return currentTileNumber >= ConditionValue;
            case ConditionType.Between: 
                return currentTileNumber >= ConditionValue && currentTileNumber <= ConditionValue2;
            default:
                return false;
        }
    }

    public bool Verify(string inputValue)
    {
        return ActionValue == inputValue;
    }
}
