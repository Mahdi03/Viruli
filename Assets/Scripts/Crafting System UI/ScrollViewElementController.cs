using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public interface IHoverable2D : IPointerEnterHandler, IPointerExitHandler { }
public interface IClickable2D : IPointerClickHandler { }
public class ScrollViewElementController : MonoBehaviour, IHoverable2D, IClickable2D {
    public GameObject background;
    private Image backgroundImage;
    public Color backgroundHoverColor = new Color(1, 1, 1, 0.06f);

    public Color backgroundSelectedColor = new Color(1, 1, 1, 0.2f);

    public GameObject iconPlaceholderPrefab;
    public TextMeshProUGUI textBox;

    /// <summary>
    /// The Selected property is used to keep track of whether each individual element is in fact, selected
    /// 
    /// The set function for this property takes care of the background color of the UI element as well
    /// </summary>
    /// 
    private bool selected = false;
    public bool Selected {
        get { return this.selected; }
        set {
            if (value == false) {
                //Remove all background color
                removeAllBackgroundColor();
            }
            else {
                //We are selecting this value, set a static background color
                backgroundImage.color = backgroundSelectedColor;
            }
            this.selected = value; //Setting this property itself to the same value results in circular logic
        }
    }


    private int itemID = -1;
    

    private void Awake() {
        backgroundImage = background.GetComponent<Image>();
        this.Selected = false; //Automatically initialize to false
    }

    public void setIcon(GameObject prefab) {
        GameManager.clearAllChildrenOfObj(iconPlaceholderPrefab);
        Instantiate(prefab, iconPlaceholderPrefab.transform);
    }
    public void setText(string txt) {
        textBox.text = txt;
    }

    public void setItemID(int id) {
        this.itemID = id;
    }


    //On hover enter highlight white background but on hover exit remove highlight
    public void OnPointerEnter(PointerEventData eventData) {
        //Use this little catch to prevent the 
        if (!this.selected) {
            backgroundImage.color = backgroundHoverColor; //On Hover enter, add transparent background
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Use this little catch to keep the background color from changing when we hover on something else
        if (!this.selected) {
            removeAllBackgroundColor(); //On hover exit, remove transparent background
        }

    }
    private void removeAllBackgroundColor() {
        backgroundImage.color = new Color(1, 1, 1, 0);
    }


    public void OnPointerClick(PointerEventData eventData) {
        //When we click on our element, the rest of the other elements will be unselected, this one will be selected, and then we will load it
        //Loop through all the elements in our parent and for each one, unselect it
        for (int i = 0; i < transform.parent.childCount; i++) {
            ScrollViewElementController scrollViewElementController = transform.parent.GetChild(i).GetComponent<ScrollViewElementController>();
            scrollViewElementController.Selected = false;
        }
        //Then select this one - the background color stuff is taken care of with the Selected property
        this.Selected = true;

        //Now load the rest of the craftable item's data from the database into the UI using the itemID
        clearAllInfo();
        ShowCraftableItemInfo();
        ShowCraftableItemAction();
    }
    protected void clearAllInfo() {
        var CraftingUIInfoContainer = GameObject.FindGameObjectWithTag("CraftingUIInfoContainer");
        var CraftingUIActionContainer = GameObject.FindGameObjectWithTag("CraftingUIActionContainer");
        //GameManager.clearAllChildrenOfObj(CraftingUIInfoContainer);
        GameManager.clearAllChildrenOfObj(CraftingUIActionContainer);
    }
    void ShowCraftableItemInfo() {
        var CraftingUIInfoContainer = GameObject.FindGameObjectWithTag("CraftingUIInfoContainer");

        var CraftableItemInfoGroup = CraftingUIInfoContainer.transform.GetChild(0);
        //TODO: Next time we might need to instantiate the prefab and not just find it in the hierarchy depending on how the walls system works
        var craftableUIInfoGroupContainerController = CraftableItemInfoGroup.GetComponent<CraftableUIInfoGroupContainerController>();
        CraftableItemInfoGroup.gameObject.SetActive(true);

        var item = InGameItemsDatabaseManager.Instance.getItemByID(itemID);

        craftableUIInfoGroupContainerController.SetIcon(item.TwoDimensionalPrefab);
        craftableUIInfoGroupContainerController.SetItemName(item.itemName);
        craftableUIInfoGroupContainerController.SetItemDescription(item.ItemDescription);
        craftableUIInfoGroupContainerController.SetItemStatsText("- Effect Radius: " + item.EffectRadius + "ft\n- Effect Timeout: " + item.EffectTimeout + "sec");
    }
    private Color tableBorderColor = Color.white;
    private Color tablePaddingColor = Color.HSVToRGB(213 / 360f, 17 / 100f, 21 / 100f);

    void ShowCraftableItemAction() {
        var CraftingUIActionContainer = GameObject.FindGameObjectWithTag("CraftingUIActionContainer");
        var table = Table.createNewTable(CraftingUIActionContainer.transform, 220, 100);
        var arrOfRecipeItems = InGameItemsDatabaseManager.Instance.getItemByID(itemID).Recipe;
        var craftable = true;
        foreach (var item in arrOfRecipeItems) {
            var id = item.Item1;
            var requiredItem = InGameItemsDatabaseManager.Instance.getItemByID(id);
            var countRequired = item.Item2 * 1;//TODO: multiply by the amount they put in the input
            var countAvailable = InventoryManager.Instance.getItemCountByID(id);
            //Make a row
            var row = Table.createTableRow(table.transform, 30f);
            //In the first cell instantiate the prefab
            var iconCell = Table.createTableCell(row.transform, cellWidth: 30f, tableBorderColor, borderWidth: 1f, tablePaddingColor);
            var requiredItemIcon = Instantiate(requiredItem.TwoDimensionalPrefab, iconCell.transform);
            //Add hover tooltip
            Item.attachItemInstance(requiredItemIcon, id); //Give the 2D Icon an ID so that the HoverTooltip can read it
            Item.allowHoverTooltip(requiredItemIcon);

            //In the second cell put in the amount required
            var requiredAmountCell = Table.createTableCell(row.transform, cellWidth: 190f, tableBorderColor, borderWidth: 1f, tablePaddingColor);
            var textbox = new GameObject("Required Amount");
            
            var text = textbox.AddComponent<TextMeshProUGUI>();
            text.text = "<color=\"" + ((countAvailable < countRequired) ? "red" : "green") + "\">" + countAvailable + "</color>/" + countRequired;
            //Set font size to 16
            text.fontSize = 16f;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            if (countAvailable < countRequired) {
                craftable = false;
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

        //TODO: Instantiate the input group prefab
        Instantiate(CraftingUIController.Instance.craftingUIPotionCraftingInputGroup, CraftingUIActionContainer.transform);
    }

}
