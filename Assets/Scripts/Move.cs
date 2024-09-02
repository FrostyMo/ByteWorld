public class Move
{
    public string Direction { get; set; } // "W", "A", "D"
    public int Count { get; set; } // Number of times this move is executed
    public bool IsLoop { get; set; } // Whether this move is part of a loop
    public bool IsExecuted { get; set; } // To track if the move has been executed

    public Move(string direction, int count)
    {
        Direction = direction;
        Count = count;

        if (Count > 1)
        {
            IsLoop = true;
        }
        IsExecuted = false; // Initially, the move is not executed
    }
}