using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Static class used to create tables in the UI<br /><br />
/// 
/// First call <code>.createNewTable()</code>
/// and then for each object, a new <code>.createTableRow()</code>
/// Inside each row add a new <code>.createTableCell()</code>
/// And then you can insert whatever you need directly using the return value,
/// no need to search for children
/// 
/// </summary>
public class Table : MonoBehaviour {
	/// <summary>
	/// Creates a new table wrapper ready to inserts rows and cells.<br />
	/// First function call in process of making table UI in Unity
	/// </summary>
	/// <param name="parent">Transform of parent element</param>
	/// <param name="tableWidth">Width of table (forces rows to extend to this size)</param>
	/// <param name="tableHeight">Height of table</param>
	/// <returns>Returns the <c>GameObject</c> table if you want to modify it later on or grab its transform to add rows to.</returns>
	public static GameObject createNewTable(Transform parent, float tableWidth = 100f, float tableHeight = 100f) {
		var table = new GameObject("Table");

		/*vertical layout group*/
		var verticalLayoutGroup = table.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
		verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;

		verticalLayoutGroup.childControlHeight = false;
		verticalLayoutGroup.childControlWidth = true;

		verticalLayoutGroup.childForceExpandHeight = false;
		verticalLayoutGroup.childForceExpandWidth = true;


		/*rect transform*/
		var rectTransform = table.GetComponent<RectTransform>(); //new GameObject() automatically comes with new transform

		rectTransform.SetParent(parent, false);
		//Will align to center top
		rectTransform.anchorMin = new Vector2(0.5f, 1);
		rectTransform.anchorMax = new Vector2(0.5f, 1);
		rectTransform.pivot = new Vector2(0.5f, 1);
		rectTransform.anchoredPosition = new Vector2(0, 0);
		rectTransform.localScale = new Vector2(1, 1);
		rectTransform.sizeDelta = new Vector2(tableWidth, tableHeight);

		return table;
	}
	/// <summary>
	/// This function creates a new row and appends it to the given table parent object
	/// </summary>
	/// <param name="parentTable">Transform of table object to append row into</param>
	/// <param name="rowHeight">Height of each row (and cell inside it)</param>
	/// <returns>Returns the <c>GameObject</c> row if you want to modify it later on or grab its transform to add cells to.</returns>
	public static GameObject createTableRow(Transform parentTable, float rowHeight = 10f) {
		var row = new GameObject("Table Row");

		/*vertical layout group*/
		var horizontalLayoutGroup = row.AddComponent<HorizontalLayoutGroup>();
		horizontalLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
		horizontalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childControlWidth = false;
		horizontalLayoutGroup.childForceExpandHeight = true;
		horizontalLayoutGroup.childForceExpandWidth = false;

		/*rect transform*/
		var rectTransform = row.GetComponent<RectTransform>();
		rectTransform.SetParent(parentTable, false);
		//Anchors are controlled by parent vertical layout group
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.localScale = new Vector2(1, 1);
		rectTransform.sizeDelta = new Vector2(0, rowHeight);

		return row;
	}

	/// <summary>
	/// This function creates a table cell with a border of a given width and given colors
	/// </summary>
	/// <param name="parentRow">Transform of parent row object of where to append cell to</param>
	/// <param name="cellWidth">The width of each table cell</param>
	/// <param name="borderColor">The color of the border</param>
	/// <param name="borderWidth">The width of the borders (0 if no border)</param>
	/// <param name="paddingColor">The color inside the cell (use background color to give it a feeling of transparency)</param>
	/// <returns>Returns the padding cell inside so that we can directly append text or icons or whatever we need inside of it.</returns>
	public static GameObject createTableCell(Transform parentRow, float cellWidth, Color borderColor, float borderWidth, Color paddingColor) {
		var border = new GameObject("Border");

		var borderImage = border.AddComponent<Image>();
		borderImage.color = borderColor;

		var borderRectTransform = border.GetComponent<RectTransform>();
		borderRectTransform.SetParent(parentRow, false);
		borderRectTransform.sizeDelta = new Vector2(cellWidth, 0);
		borderRectTransform.localScale = new Vector2(1, 1);

		//Inside the border we add the padding and then return the padding so that they can instantiate from the padding directly
		var padding = new GameObject("Padding");

		var paddingImage = padding.AddComponent<Image>();
		paddingImage.color = paddingColor;

		var paddingRectTransform = padding.GetComponent<RectTransform>();
		paddingRectTransform.SetParent(borderRectTransform, false);
		paddingRectTransform.localScale = new Vector2(1, 1);

		//Sets to stretch
		paddingRectTransform.anchorMin = new Vector2(0, 0);
		paddingRectTransform.anchorMax = new Vector2(1, 1);
		//Stretch components: https://stackoverflow.com/questions/30782829/how-to-access-recttransforms-left-right-top-bottom-positions-via-code
		paddingRectTransform.offsetMin = new Vector2(borderWidth, borderWidth); //(Left, Bottom)
		paddingRectTransform.offsetMax = new Vector2(-borderWidth, -borderWidth); //(-Right, -Top)

		return padding;
	}
}
