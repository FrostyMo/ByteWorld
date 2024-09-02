using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomGameOptions : MonoBehaviour
{
    public Toggle specialTileToggle;
    public Toggle secureTileToggle;
    public Toggle detourToggle;
    public Toggle limitedLivesToggle;
    public Slider tileCountSlider;
    public TextMeshProUGUI sliderValue;
    public Button okButton;
    public Button cancelButton;

    private int numberOfTiles;

    private ColorBlock toggleOnColors;
    private ColorBlock toggleOffColors;

    private void Start()
    {
        okButton.onClick.AddListener(OnOkClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        tileCountSlider.onValueChanged.AddListener(delegate { UpdateTileCount(); });
        if (specialTileToggle != null)
        {
            toggleOnColors = specialTileToggle.colors;
            toggleOffColors = toggleOnColors;
            toggleOffColors.normalColor = Color.white;

            toggleOnColors.selectedColor = toggleOnColors.normalColor;
            
        }
     
    }

    private void UpdateTileCount()
    {
        numberOfTiles = Mathf.RoundToInt(tileCountSlider.value);
        sliderValue.text = numberOfTiles.ToString();
    }

    public void fieldToggled(string name)
    {
        switch (name)
        {
            case "SpecialTile":
                specialTileToggle.colors = specialTileToggle.isOn ? toggleOnColors : toggleOffColors;
                break;
            case "SecureTile":
                secureTileToggle.colors = secureTileToggle.isOn ? toggleOnColors : toggleOffColors;
                break;
            case "Detour":
                detourToggle.colors = detourToggle.isOn ? toggleOnColors : toggleOffColors;
                break;
            case "LimitedLives":
                limitedLivesToggle.colors = limitedLivesToggle.isOn ? toggleOnColors : toggleOffColors;
                break;
        }
    }

    private void OnOkClicked()
    {
        UpdateTileCount();
        GameController.Instance.includeSpecialTiles = specialTileToggle.isOn;
        GameController.Instance.includeSecureTiles = secureTileToggle.isOn;
        GameController.Instance.includeDetours = detourToggle.isOn;
        GameController.Instance.limitedLives = limitedLivesToggle.isOn;
        GameController.Instance.numberOfTiles = numberOfTiles;
        // Hide the popup
        gameObject.SetActive(false);

        // Load the Level scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }

    private void OnCancelClicked()
    {
        // Hide the popup without doing anything
        gameObject.SetActive(false);
    }
}