using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIPotionCraftingInputGroupController : MonoBehaviour {
    [SerializeField]
    private GameObject craftingButton;
    private Button craftingButtonButton;
    [SerializeField]
    private GameObject integerInput;
    private TMP_InputField integerInputField;
    private bool initialized = false;
    /// <summary>
    /// Use this as an initialization lock to prevent from getting null values
    /// </summary>
    private bool Initialized {
        get { return initialized; }
        set {
            if (value == true) {
                
            }
            initialized = value;
        }
    }

    private void Awake() {
        craftingButtonButton = craftingButton.GetComponent<Button>();
        integerInputField = integerInput.GetComponent<TMP_InputField>();
    }
    public void SetAmountToCraft(int amountToCraft) {
        
        integerInputField.text = amountToCraft.ToString();
    }
    public void EnableCraftingButton() {
        
        craftingButtonButton.interactable = true;
        //Change background color to enabled button
    }
    public void DisableCraftingButton() {
        
        craftingButtonButton.interactable = false;
        //Change background color to disabled button
    }
    /// <summary>
    /// This function is called on click of the button
    /// </summary>
    public void CraftPotion() {
        CraftingUIController.Instance.CraftPotion();
    }
    public void UpdateRecipeTable(string inputText) {
        int result;
        if (inputText == "") {
            //Well then they probably just deleting it, disable the button for now and ignore everything else
            this.DisableCraftingButton();
            return;
        }
        if (!int.TryParse(inputText, out result)) {
            throw new System.FormatException("The input \"" + inputText + "\" is not an acceptable integer type");
        }
        else {
            //result is now defined
            CraftingUIController.Instance.UpdateRecipeTable(result);
        }
    }
}
