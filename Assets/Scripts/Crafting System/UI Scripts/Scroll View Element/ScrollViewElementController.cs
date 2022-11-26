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
	protected Image backgroundImage;
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
	protected bool selected = false;
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


	protected int itemID = -1;

	private bool initialized = false;

	protected void Awake() {
		if (this.GetType().Name != "ScrollViewElementController") {
			/*Then we are in a derived class 
			 * the prefab is guaranteed to have a valid copy of the prefab and its public member variables set thru the editor
			 * 
			 * copy those over
			 */
			ScrollViewElementController scrollViewElementController = GetComponent<ScrollViewElementController>();
			Debug.Assert(this.GetType().Name != scrollViewElementController.GetType().Name);
			if (scrollViewElementController != null) {
				if (this.background == null) {
					this.background = scrollViewElementController.background;
				}
				if (this.iconPlaceholderPrefab == null) {
					this.iconPlaceholderPrefab = scrollViewElementController.iconPlaceholderPrefab;
				}
				if (this.textBox == null) {
					this.textBox = scrollViewElementController.textBox;
				}
				//Destroy(scrollViewElementController); //Now remove that object since we are the deriving member
			}
		}
		backgroundImage = background.GetComponent<Image>();
		this.Selected = false; //Automatically initialize to false
		initialized = true;
	}

	public void setIcon(GameObject prefab) {
		GameManager.clearAllChildrenOfObj(iconPlaceholderPrefab);
		Instantiate(prefab, iconPlaceholderPrefab.transform);
	}
	public void setText(string txt) {
		if (!initialized) { Awake(); }
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
	protected void removeAllBackgroundColor() {
		backgroundImage.color = new Color(1, 1, 1, 0);
	}


	public virtual void OnPointerClick(PointerEventData eventData) {
		//When we click on our element, the rest of the other elements will be unselected, this one will be selected, and then we will load it
		//Loop through all the elements in our parent and for each one, unselect it
		for (int i = 0; i < transform.parent.childCount; i++) {
			ScrollViewElementController scrollViewElementController = transform.parent.GetChild(i).GetComponent<ScrollViewElementController>();
			scrollViewElementController.Selected = false;
		}
		//Then select this one - the background color stuff is taken care of with the Selected property
		this.Selected = true;

		
	}

}
