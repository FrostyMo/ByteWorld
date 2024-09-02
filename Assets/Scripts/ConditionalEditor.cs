using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConditionalEditor : MonoBehaviour
{
    public GameObject conditionalEditorPanel;
    public GameObject scrollViewContent;
    public GameObject conditionTemplatePrefab;
    public Button addConditionButton;
    public Button applyButton;

    private List<ConditionTemplate> conditionTemplates = new List<ConditionTemplate>();
    private PlayerMovement playerMovement;

    private bool isPanelOpen = false;

    private void Start()
    {
        

        playerMovement = FindObjectOfType<PlayerMovement>();

        addConditionButton.onClick.AddListener(AddCondition);
        applyButton.onClick.AddListener(ApplyConditions);

        conditionalEditorPanel.SetActive(false);

        // Initialize the first condition template in the editor
        InitializeFirstConditionTemplate();
    }

    private void InitializeFirstConditionTemplate()
    {
        GameObject firstConditionTemplate = scrollViewContent.transform.GetChild(0).gameObject;
        firstConditionTemplate.SetActive(true);
        ConditionTemplate conditionTemplate = firstConditionTemplate.GetComponent<ConditionTemplate>();
        conditionTemplate.Initialize(this, true);
        conditionTemplates.Add(conditionTemplate);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && GameStateManager.Instance.CurrentState != GameState.ConditionalEditing)
        {
            TogglePanel();
        }
    }

    private void AddCondition()
    {
        GameObject newCondition = Instantiate(conditionTemplatePrefab, scrollViewContent.transform);
        ConditionTemplate conditionTemplate = newCondition.GetComponent<ConditionTemplate>();
        conditionTemplate.Initialize(this, false);

        // Position the new condition template
        RectTransform rectTransform = newCondition.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -conditionTemplates.Count * 125);

        conditionTemplates.Add(conditionTemplate);
    }

    public void RemoveCondition(ConditionTemplate conditionTemplate)
    {
        conditionTemplates.Remove(conditionTemplate);
        Destroy(conditionTemplate.gameObject);
        UpdateConditionPositions();
    }

    private void UpdateConditionPositions()
    {
        for (int i = 0; i < conditionTemplates.Count; i++)
        {
            RectTransform rectTransform = conditionTemplates[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * 125);
        }
    }

    public void ApplyConditions()
    {
        List<Condition> conditions = new List<Condition>();
        foreach (ConditionTemplate template in conditionTemplates)
        {
            conditions.Add(template.GetCondition());
        }

        playerMovement.SetConditions(conditions);
        conditionalEditorPanel.SetActive(false);
        isPanelOpen = false;
        GameStateManager.Instance.SetGameState(GameState.Playing);
    }

    public void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        conditionalEditorPanel.SetActive(isPanelOpen);
        if (isPanelOpen)
        {
            GameStateManager.Instance.SetGameState(GameState.ConditionalEditing);
        }
    }
}
