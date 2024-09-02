
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VariableBinder : MonoBehaviour
{
    public GameObject variableBinderPanel;
    public TMP_InputField inputFieldX;
    public TMP_InputField inputFieldY;
    public TMP_InputField inputFieldZ;
    public Button closeButton;
    public GameObject variableTips; // The GameObject containing variable tips
    public TextMeshProUGUI variableTipsText; // The TextMeshProUGUI for the tips

    private bool isPanelOpen = false;
    private bool variablesAssigned = false; // Flag to check if variables have been assigned

    private void Start()
    {
        closeButton.onClick.AddListener(ValidateAndClosePanel);
        variableBinderPanel.SetActive(false);
        variableTips.SetActive(false); // Initially disable VariableTips

        // Set placeholder texts
        //SetPlaceholderText(inputFieldX, "Numbers between 1 - 10 only");
        //SetPlaceholderText(inputFieldY, "Numbers between 1 - 10 only");
        //SetPlaceholderText(inputFieldZ, "Numbers between 1 - 10 only");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!isPanelOpen)
            {
                OpenPanel();
            }
            else
            {
                ValidateAndClosePanel();
            }
        }
    }

    private void OpenPanel()
    {
        GameStateManager.Instance.SetGameState(GameState.VariableBinding);
        isPanelOpen = true;
        variableBinderPanel.SetActive(true);
    }

    //private void TogglePanel()
    //{
    //    if (!isPanelOpen)
    //    {
    //        GameStateManager.Instance.SetGameState(GameState.VariableBinding);
    //    }
    //    else
    //    {
    //        ValidateAndClosePanel();
    //    }

    //    isPanelOpen = !isPanelOpen;
    //    variableBinderPanel.SetActive(isPanelOpen);
    //}

    private void ValidateAndClosePanel()
    {
        ValidateInputField(inputFieldX);
        ValidateInputField(inputFieldY);
        ValidateInputField(inputFieldZ);

        if (IsValidInput(inputFieldX) && IsValidInput(inputFieldY) && IsValidInput(inputFieldZ))
        {
            isPanelOpen = false;
            variableBinderPanel.SetActive(false);
            GameStateManager.Instance.SetGameState(GameState.Playing);

            variablesAssigned = true;
            UpdateVariableTips();
            variableTips.SetActive(true);
        }
    }

    private void ValidateInputField(TMP_InputField inputField)
    {
        int value;
        if (inputField.text == "")
        {
            inputField.text = "1";
        }
        if (!int.TryParse(inputField.text, out value) || value < 1 || value > 10)
        {
            inputField.text = "";
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Numbers between 1 - 10 only";
            inputField.image.enabled = true; // Highlight the input field in red
        }
        else
        {
            inputField.image.enabled = false; // Reset the input field color if valid
        }
    }

    private bool IsValidInput(TMP_InputField inputField)
    {
        int value;
        return int.TryParse(inputField.text, out value) && value >= 1 && value <= 10;
    }

    private void SetPlaceholderText(TMP_InputField inputField, string placeholderText)
    {
        TextMeshProUGUI placeholder = inputField.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholder != null)
        {
            placeholder.text = placeholderText;
        }
    }

    public int GetVariableX()
    {
        int value;
        if (int.TryParse(inputFieldX.text, out value))
        {
            return Mathf.Clamp(value, 1, 10);
        }
        return 1;
    }

    public int GetVariableY()
    {
        int value;
        if (int.TryParse(inputFieldY.text, out value))
        {
            return Mathf.Clamp(value, 1, 10);
        }
        return 1;
    }

    public int GetVariableZ()
    {
        int value;
        if (int.TryParse(inputFieldZ.text, out value))
        {
            return Mathf.Clamp(value, 1, 10);
        }
        return 1;
    }

    public bool IsPanelOpen()
    {
        return isPanelOpen;
    }

    private void UpdateVariableTips()
    {
        int x = GetVariableX();
        int y = GetVariableY();
        int z = GetVariableZ();

        variableTipsText.text = $"{x} forward(s)\n\n{y} forward(s)\n\n{z} forward(s)";
    }
}
