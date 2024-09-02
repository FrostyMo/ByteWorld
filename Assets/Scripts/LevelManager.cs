using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static List<Vector3> GetLevelPositions(Level level)
    {
        switch (level)
        {
            case Level.Level1:
                return Level1();
            case Level.Level2:
                return Level2();
            // Add more cases for additional levels...
            case Level.Level3:
                return Level3();
            case Level.Level4:
                return Level4();
            case Level.Level5:
                return Level5();
            case Level.Level6:
                return Level6();
            case Level.Level7:
                return Level7();
            case Level.Level8:
                return Level8();
            case Level.Level9:
                return Level9();
            case Level.Level10:
                return Level10();
            case Level.Level11:
                return Level11();
            default:
                Debug.LogWarning("Level not defined!");
                return new List<Vector3>();
        }
    }

    private static List<Vector3> Level1()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12) // Forward
        };
        return positions;
    }

    private static List<Vector3> Level2()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(6, 0, 6), // Right
            startPos + new Vector3(6, 0, 12), // Forward
            startPos + new Vector3(6, 0, 18), // Forward
            startPos + new Vector3(0, 0, 18) // Left
        };
        return positions;
    }

    private static List<Vector3> Level3()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(6, 0, 6), // Right
            startPos + new Vector3(6, 0, 12), // Forward
            startPos + new Vector3(0, 0, 12), // Left
            startPos + new Vector3(0, 0, 18)  // Forward
        };
        return positions;
    }

    private static List<Vector3> Level4()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(0, 0, 18), // Forward
            startPos + new Vector3(0, 0, 24), // Forward
            startPos + new Vector3(6, 0, 24), // Right
            startPos + new Vector3(6, 0, 18), // Backward
            startPos + new Vector3(6, 0, 12), // Backward
            startPos + new Vector3(6, 0, 6), // Backward
            startPos + new Vector3(12, 0, 6), // Right
            startPos + new Vector3(12, 0, 12) // Forward
        };
        return positions;
    }

    private static List<Vector3> Level5()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(0, 0, 18), // Forward
            startPos + new Vector3(0, 0, 24), // Forward
            startPos + new Vector3(0, 0, 30), // Forward
            startPos + new Vector3(0, 0, 36), // Forward
            startPos + new Vector3(6, 0, 36), // Right
            startPos + new Vector3(6, 0, 42) // Forward
        };
        return positions;
    }

    private static List<Vector3> Level6()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(0, 0, 18), // Forward
            startPos + new Vector3(0, 0, 24), // Forward
            startPos + new Vector3(0, 0, 30), // Forward
            startPos + new Vector3(6, 0, 36), // Right
            startPos + new Vector3(12, 0, 36), // Forward
            startPos + new Vector3(18, 0, 36), // Forward
            startPos + new Vector3(24, 0, 36), // Forward
            startPos + new Vector3(30, 0, 36), // Forward
            startPos + new Vector3(30, 0, 30), // Right
            startPos + new Vector3(30, 0, 24), // Forward
            startPos + new Vector3(30, 0, 18), // Forward
            startPos + new Vector3(30, 0, 12), // Forward
            startPos + new Vector3(30, 0, 6), // Forward
            startPos + new Vector3(30, 0, 0), // Forward

        };
        return positions;
    }

    private static List<Vector3> Level7()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(6, 0, 12), // Right Forward
            startPos + new Vector3(12, 0, 12), // Forward
            startPos + new Vector3(18, 0, 12), // Forward
            startPos + new Vector3(18, 0, 18), // Left Forward
            startPos + new Vector3(18, 0, 24), // Forward
            startPos + new Vector3(18, 0, 30), // Forward
            startPos + new Vector3(18, 0, 36) // Forward
        };
        return positions;
    }

    private static List<Vector3> Level8()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(0, 0, 18), // Forward
            startPos + new Vector3(0, 0, 24), // Forward
            startPos + new Vector3(0, 0, 30), // Forward
            startPos + new Vector3(0, 0, 36), // Forward
            startPos + new Vector3(0, 0, 42), // Forward
            startPos + new Vector3(0, 0, 48), // Forward
            startPos + new Vector3(0, 0, 54), // Forward
            startPos + new Vector3(0, 0, 60), // Forward
            startPos + new Vector3(-6, 0, 60), // Left Forward
            startPos + new Vector3(-12, 0, 60), // Forward
            startPos + new Vector3(-18, 0, 60), // Forward
            startPos + new Vector3(-24, 0, 60), // Forward
            startPos + new Vector3(-30, 0, 60), // Forward
            startPos + new Vector3(-36, 0, 60), // Forward
            startPos + new Vector3(-42, 0, 60), // Forward
            startPos + new Vector3(-48, 0, 60), // Forward
            startPos + new Vector3(-54, 0, 60), // Forward
            startPos + new Vector3(-60, 0, 60), // Forward
            startPos + new Vector3(-60, 0, 66), // Right Forward
            startPos + new Vector3(-60, 0, 72), // Forward
            startPos + new Vector3(-54, 0, 72), // Forward
            startPos + new Vector3(-48, 0, 72), // Forward
            startPos + new Vector3(-42, 0, 72), // Forward
            startPos + new Vector3(-36, 0, 72), // Forward
            startPos + new Vector3(-30, 0, 72), // Forward
            startPos + new Vector3(-24, 0, 72), // Forward
            startPos + new Vector3(-18, 0, 72), // Forward
            startPos + new Vector3(-12, 0, 72), // Forward
            startPos + new Vector3(-6, 0, 72), // Forward
            startPos + new Vector3(0, 0, 72), // Forward
            startPos + new Vector3(0, 0, 78), // Left Forward
            startPos + new Vector3(0, 0, 84), // Forward
            startPos + new Vector3(0, 0, 90), // Forward
            startPos + new Vector3(0, 0, 96), // Forward
            startPos + new Vector3(0, 0, 102), // Forward
            startPos + new Vector3(0, 0, 108), // Forward
            startPos + new Vector3(0, 0, 114), // Forward
            startPos + new Vector3(0, 0, 120), // Forward
            startPos + new Vector3(0, 0, 126), // Forward
            startPos + new Vector3(0, 0, 132), // Forward
            startPos + new Vector3(6, 0, 132), // Right Forward
            startPos + new Vector3(12, 0, 132), // Forward
            startPos + new Vector3(12, 0, 126), // Right Forward
            startPos + new Vector3(12, 0, 120), // Forward
            startPos + new Vector3(12, 0, 114), // Forward
            startPos + new Vector3(12, 0, 108), // Forward
            startPos + new Vector3(12, 0, 102), // Forward
            startPos + new Vector3(12, 0, 96), // Forward
            startPos + new Vector3(12, 0, 90), // Forward
            startPos + new Vector3(12, 0, 84), // Forward
            startPos + new Vector3(12, 0, 78), // Forward
            startPos + new Vector3(12, 0, 72), // Forward
            startPos + new Vector3(18, 0, 72), // Left Forward
            startPos + new Vector3(24, 0, 72), // Forward
            startPos + new Vector3(30, 0, 72), // Forward
            startPos + new Vector3(36, 0, 72), // Forward
            startPos + new Vector3(42, 0, 72), // Forward
            startPos + new Vector3(48, 0, 72), // Forward
            startPos + new Vector3(54, 0, 72), // Forward
            startPos + new Vector3(60, 0, 72), // Forward
            startPos + new Vector3(66, 0, 72), // Forward
            startPos + new Vector3(72, 0, 72), // Forward
            startPos + new Vector3(72, 0, 66), // Right Forward
            startPos + new Vector3(72, 0, 60), // Forward
            startPos + new Vector3(66, 0, 60), // Right Forward
            startPos + new Vector3(60, 0, 60), // Forward
            startPos + new Vector3(54, 0, 60), // Forward
            startPos + new Vector3(48, 0, 60), // Forward
            startPos + new Vector3(42, 0, 60), // Forward
            startPos + new Vector3(36, 0, 60), // Forward
            startPos + new Vector3(30, 0, 60), // Forward
            startPos + new Vector3(24, 0, 60), // Forward
            startPos + new Vector3(18, 0, 60), // Forward
            startPos + new Vector3(12, 0, 60), // Forward
            startPos + new Vector3(12, 0, 54), // Left Forward
            startPos + new Vector3(12, 0, 48), // Forward
            startPos + new Vector3(12, 0, 42), // Forward
            startPos + new Vector3(12, 0, 36), // Forward
            startPos + new Vector3(12, 0, 30), // Forward
            startPos + new Vector3(12, 0, 24), // Forward
            startPos + new Vector3(12, 0, 18), // Forward
            startPos + new Vector3(12, 0, 12), // Forward
            startPos + new Vector3(12, 0, 6), // Forward
            startPos + new Vector3(12, 0, 0), // Forward
        };
        return positions;
    }

    private static List<Vector3> Level9()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(0, 0, 18), // Forward
            startPos + new Vector3(0, 0, 24), // Forward
        };
        return positions;
    }

    private static List<Vector3> Level10()
    {
        Vector3 startPos = Vector3.zero;
        List<Vector3> positions = new List<Vector3>
        {
            startPos,
            startPos + new Vector3(0, 0, 6), // Forward
            startPos + new Vector3(0, 0, 12), // Forward
            startPos + new Vector3(0, 0, 18), // Forward
            startPos + new Vector3(0, 0, 24), // Forward
            startPos + new Vector3(0, 0, 30), // Forward
            startPos + new Vector3(0, 0, 36), // Forward
            startPos + new Vector3(0, 0, 42), // Forward
        };
        return positions;
    }

    private static List<Vector3> Level11()
    {
        List<Vector3> positions = Level8();
        positions.Add(new Vector3(12, 0, -6));// Forward
        positions.Add(new Vector3(12, 0, -12)); // Forward
        positions.Add(new Vector3(12, 0, -18));// Forward
        positions.Add(new Vector3(12, 0, -24));// Forward
        positions.Add(new Vector3(12, 0, -30));// Forward
        positions.Add(new Vector3(12, 0, -36)); // Forward
        positions.Add(new Vector3(12, 0, -42)); // Forward
        return positions;
    }

    public static void ShowLevelDialog(Level level)
    {
        string[] messages = new string[0];
        string prefKey = "";
        bool showHUD = true;

        switch (level)
        {
            case Level.Level1:
                messages = new string[]
                {
                "Hi there. Welcome to the byte world. The first block is always red. The goal block is always green. You can press tab to check top view.",
                "You can press W to move forward one tile. A and D keys will allow you to face left and right respectively.",
                "You need to provide all moves and then press enter to play level. You only get one try to get to goal, if you fall or fail the level restarts."
                };
                prefKey = "Level1DialogShown";
                showHUD = false;
                break;
            case Level.Level2:
                messages = new string[]
                {
                "Good work, now let's have some turns enabled.",
                "Remember, turning right or left does not move character anywhere. It just turns it.",
                "You can also see the number of moves made below and required moves. Later levels introduce what required moves are"
                };
                prefKey = "Level2DialogShown";
                showHUD = false;
                break;
            case Level.Level3:
                messages = new string[]
                {
                "Great job! Now let's add more turns and forward moves.",
                "Keep an eye on the path and plan your moves carefully."
                };
                prefKey = "Level3DialogShown";
                showHUD = false;
                break;
            case Level.Level4:
                messages = new string[]
                {
                "Now you will experience a more complex path.",
                "Don't forget to use the top view with Tab for better planning."
                };
                prefKey = "Level4DialogShown";
                showHUD = false;
                break;
            case Level.Level5:
                messages = new string[]
                {
                "Excellent progress! This level introduces loops. Loops are what the name entails, they repeat something",
                "You can loop 'W' by holding the 'Shift' key and pressing 'W' with it. That will enable a pop up",
                "Type any number you want, this will add that number of moves for forwards",
                "Remember, you can use loops to reduce your move count. Less move count means greater score!"
                };
                prefKey = "Level5DialogShown";
                showHUD = false;
                break;
            case Level.Level6:
                messages = new string[]
                {
                "Loops will make your moves more efficient.",
                "Try to complete the path with fewer moves."
                };
                prefKey = "Level6DialogShown";
                showHUD = false;
                break;
            case Level.Level7:
                messages = new string[]
                {
                "Great job so far! Let's add more challenge.",
                "You can use 'variables' that enable moving more with one button. Imagine them being like 'W' to move forward but more than one step",
                "To use variables, you can press 'V'. You will see a pop up that will allow you to set values for x,y,z",
                "This is similar to what you do in a math class. Now you finally use x :D",
                "For now, set x as 2, y as 3 and z as 4. Then close the pop up and see what pressing each of these does"
                };
                prefKey = "Level7DialogShown";
                showHUD = false;
                break;
            case Level.Level8:
                messages = new string[]
                {
                "This level may seem daunting but set any variable (x, y, z) to 10 and see how easily you can cross this level",
                "Variables also use only 1 move per all the steps of variable. Its a good way to level up",
                };
                prefKey = "Level8DialogShown";
                showHUD = false;
                break;
            case Level.Level9:
                messages = new string[]
                {
                "In this round, you'll see Special Tiles. You need to put special armor on such tiles to get max score.",
                "This is called conditioning. IF tile SPECIAL PutArmor SPECIAL",
                "In programming, IF conditions are used to evaluate a certain action IF some value/expression is true.",
                "Press C to open conditions panel. For this level, add this statement 'IF position = 3 ApplyArmor ColdArmor'"
                };
                prefKey = "Level9DialogShown";
                showHUD = false;
                break;
            case Level.Level10:
                messages = new string[]
                {
                "Great. This level introduces Secure Tiles. These are like passwords, if you enter correct password, you are authenticated",
                "Secure Tiles are mostly consecutively placed (meaning, always two or more of them together)",
                "Think several being together, would you use Equal condition type for each tile?",
                "Use conditions learned previously to VerifyUsing correct password. *HINT* Try of using LessThan & Eq condition"
                };
                prefKey = "Level10DialogShown";
                showHUD = false;
                break;
            case Level.Level11:
                messages = new string[]
                {
                "Final level. Let's apply everything we've learnt.",
                "Use loops, variables and conditions to maximize score.",
                };
                prefKey = "Level11DialogShown";
                showHUD = false;
                break;
            default:
                Debug.LogWarning("Level not defined!");
                break;
        }

        GameStateManager.Instance.ShowDialog(messages, prefKey, showHUD);
    }

    public static TileType DetermineTileType(int tileNumber)
    {
        
        TileType tile = TileType.Normal;
        switch (TileGenerator.levelNumber)
        {
            case Level.Level9:
                switch (tileNumber)
                {
                    case 3:
                        tile = TileType.Frozen;
                        break;
                }
                break;
            case Level.Level10:
                switch (tileNumber)
                {
                    case 1:
                    case 2:
                    case 3:
                        tile = TileType.SecureTile123;
                        break;
                }
                break;
            case Level.Level11:
                switch (tileNumber)
                {
                    case int n when (n > 1 && n <= 5):
                        tile = TileType.SecureTile123;
                        break;
                    case int n when (n > 6 && n <= 10):
                        tile = TileType.SecureTile321;
                        break;
                    case 15:
                        tile = TileType.Frozen;
                        break;
                    case 17:
                    case 18:
                        tile = TileType.Lava;
                        break;
                    case int n when (n > 30 && n <= 35):
                        tile = TileType.SecureTile321;
                        break;
                }
                break;
            default:
                return tile;
            //case Level.Level10:
            //    return Level10();
        }
        return tile;
    }
}
