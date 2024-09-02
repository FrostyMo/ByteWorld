using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ConditionTemplate : MonoBehaviour
{
    public TMP_Dropdown conditionTypeDropdown;
    public TMP_InputField conditionValueInput;
    public TextMeshProUGUI andText;
    public TMP_InputField conditionValueInput2; // Second input field for "Between"
    public TMP_Dropdown actionDropdown;
    public TMP_Dropdown actionValueDropdown;
    public Button deleteButton;

    private ConditionalEditor conditionalEditor;
    private bool isFirstCondition = false;

    public void Initialize(ConditionalEditor editor, bool firstCondition)
    {
        conditionalEditor = editor;
        isFirstCondition = firstCondition;

        conditionTypeDropdown.onValueChanged.AddListener(delegate { ValidateConditionInput(); OnConditionTypeChanged(); });
        conditionValueInput.onValueChanged.AddListener(delegate { ValidateConditionInput(); });
        conditionValueInput2.onValueChanged.AddListener(delegate { ValidateConditionInput(); });
        actionDropdown.onValueChanged.AddListener(delegate { UpdateActionValueDropdown(); });
        deleteButton.onClick.AddListener(RemoveCondition);

        conditionTypeDropdown.value = 0; // Set to "Equals" by default
        actionDropdown.interactable = false;
        actionValueDropdown.interactable = false;
        // Initially hide the second input field
        conditionValueInput2.gameObject.SetActive(false);
        andText.gameObject.SetActive(false);

        if (isFirstCondition)
        {
            deleteButton.onClick.AddListener(ClearCondition);
            //deleteButton.gameObject.SetActive(false); // Hide delete button for the first condition
        }

        PopulateConditionTypeDropdown();
        PopulateActionDropdown();
    }

    public Condition GetCondition()
    {
        ConditionType conditionType = (ConditionType)conditionTypeDropdown.value;
        ActionType actionType = (ActionType)actionDropdown.value;
        if (conditionValueInput.text == "")
        {
            conditionValueInput.text = "0";
        }
        int conditionValue = int.Parse(conditionValueInput.text);

        // must only be active if Between type selected
        int conditionValue2 = conditionType == ConditionType.Between ? int.Parse(conditionValueInput2.text) : 0;

        string actionValue = "";
        if (actionValueDropdown.options.Count != 0)
         actionValue = actionValueDropdown.options[actionValueDropdown.value].text; // Get action value

        return new Condition(conditionType, conditionValue, conditionValue2, actionType,actionValue);
    }

    private void PopulateConditionTypeDropdown()
    {
        conditionTypeDropdown.ClearOptions();
        conditionTypeDropdown.AddOptions(new List<string> { "Equals", "GreaterThan & Eq", "LessThan & Eq", "Between" });
    }

    private void PopulateActionDropdown()
    {
        actionDropdown.ClearOptions();
        actionDropdown.AddOptions(new List<string> { "ApplyArmor", "TakeDetour", "VerifyUsing"});
        //UpdateActionValueDropdown();
    }

    private void UpdateActionValueDropdown()
    {
        actionValueDropdown.ClearOptions();
        List<string> options = new List<string>();

        if (actionDropdown.value == (int)ActionType.ApplyArmor)
        {
            options.Add("Cold Armor");
            options.Add("Fire Armor");
        }
        else if (actionDropdown.value == (int)ActionType.TakeDetour)
        {
            options.Add("Left 1");
            options.Add("Left X");
            options.Add("Left Y");
            options.Add("Left Z");
            options.Add("Right 1");
            options.Add("Right X");
            options.Add("Right Y");
            options.Add("Right Z");
        }
        else if (actionDropdown.value == (int)ActionType.VerifyUsing)
        {
            options.Add("123");
            options.Add("321");
        }

        actionValueDropdown.AddOptions(options);
        actionValueDropdown.interactable = true;
    }


    private void ValidateConditionInput()
    {
        int value;
        bool isValid = int.TryParse(conditionValueInput.text, out value) && value >= 0;

        if (conditionTypeDropdown.value == (int)ConditionType.Between)
        {
            int value2;
            isValid &= int.TryParse(conditionValueInput2.text, out value2) && value2 >= 0 && value2 >= value;
        }

        if (isValid)
        {
            actionDropdown.interactable = true;
            UpdateActionValueDropdown();
        }
        else
        {
            actionDropdown.interactable = false;
            actionValueDropdown.interactable = false;
        }

        //int value;
        //if (int.TryParse(conditionValueInput.text, out value) && value > 0)
        //{
        //    actionDropdown.interactable = true;
        //    UpdateActionValueDropdown();
        //    //actionValueDropdown.interactable = true;
        //}
        //else
        //{
        //    actionDropdown.interactable = false;
        //    actionValueDropdown.interactable = false;
        //}
    }

    private void RemoveCondition()
    {
        if (!isFirstCondition)
        {
            conditionalEditor.RemoveCondition(this);
        }
        else
        {
            ClearCondition();
        }
    }

    private void OnConditionTypeChanged()
    {
        // If "Between" is selected, show the second input field and adjust spacing
        if (conditionTypeDropdown.value == (int)ConditionType.Between)
        {
            conditionValueInput2.gameObject.SetActive(true);
            andText.gameObject.SetActive(true);
        }
        else
        {
            conditionValueInput2.gameObject.SetActive(false);
            andText.gameObject.SetActive(false);
        }
    }

    
    private void ClearCondition()
    {
        conditionTypeDropdown.value = 0;
        conditionValueInput.text = "";
        actionDropdown.interactable = false;
        actionDropdown.ClearOptions();
        actionValueDropdown.interactable = false;
        actionValueDropdown.ClearOptions();
        // Update the conditions list in ConditionalEditor
        conditionalEditor.ApplyConditions();
        PopulateConditionTypeDropdown();
        PopulateActionDropdown();
    }
}
