
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CraftingUIController : MonoBehaviour, IPointerClickHandler {

    [SerializeField]
    private static readonly Color tableBorderColor = Color.white;
    [SerializeField]
    private static readonly Color tablePaddingColor = Color.HSVToRGB(213 / 360f, 17 / 100f, 21 / 100f);

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.rawPointerPress.name.Contains("Overlay")) {
            CloseCraftingMenu();
            //Only close the craftin menu if we clicked on the overlay, ignore all other clicks
        }       
    }

    //OpenCraftingMenu is declared in a different script since it requires both options for 2D and 3D handling
    public void CloseCraftingMenu() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        MainDoorManager.Instance.UnglowAllDoors(); //Since we are closing out of it, stop the door selection
    }

    public static bool fillOutRecipeTable(GameObject recipeTable, List<(int, int)> recipeArr, int amountToCraft = 1) {
        bool recipeRequirementsMet = true;
        foreach (var item in recipeArr) {
            var id = item.Item1;
            var requiredItem = InGameItemsDatabaseManager.Instance.getItemByID(id);
            var countRequired = item.Item2 * amountToCraft;
            var countAvailable = InventoryManager.Instance.getItemCountByID(id);
            //Make a row
            var row = Table.createTableRow(recipeTable.transform, 25f);
            //In the first cell instantiate the prefab
            var iconCell = Table.createTableCell(row.transform, cellWidth: 25f, tableBorderColor, borderWidth: 1f, tablePaddingColor);
            var requiredItemIcon = Instantiate(requiredItem.TwoDimensionalPrefab, iconCell.transform);
            var requiredItemIconTransform = requiredItemIcon.transform;
            requiredItemIconTransform.localScale *= 0.8f; //Scale the icon down a little to fit inside of the smaller table
            //Add hover tooltip
            IItem.attachItemInstance(requiredItemIcon, id); //Give the 2D Icon an ID so that the HoverTooltip can read it
            IItem.enableScript<OnHoverTooltip>(requiredItemIcon);

            //In the second cell put in the amount required
            var requiredAmountCell = Table.createTableCell(row.transform, cellWidth: 190f-5, tableBorderColor, borderWidth: 1f, tablePaddingColor);
            var textbox = new GameObject("Required Amount");

            var text = textbox.AddComponent<TextMeshProUGUI>();
            text.text = "<color=\"" + ((countAvailable < countRequired) ? "red" : "green") + "\">" + countAvailable + "</color>/" + countRequired;
            //Set font size to 10
            text.fontSize = 10f;
            text.font = GameManager.Instance.CRAFTINGUI_costTextFont;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            if (countAvailable < countRequired) {
                recipeRequirementsMet = false;
            }

            var rectTransform = textbox.GetComponent<RectTransform>();
            rectTransform.SetParent(requiredAmountCell.transform, false);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);

            //Give 5px padding on the left
            rectTransform.offsetMin = new Vector2(5, 0); //(Left, Bottom)
            rectTransform.offsetMax = new Vector2(-0, -0); //(-Right, -Top)
        }
        return recipeRequirementsMet;
    }

   
}
