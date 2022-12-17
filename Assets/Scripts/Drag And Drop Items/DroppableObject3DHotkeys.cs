using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DroppableObject3DHotkeys : MonoBehaviour {

    private static DroppableObject3DHotkeys instance;
    public static DroppableObject3DHotkeys Instance { get { return instance; } }


    private Canvas canvas;

    private GameObject twoDimensionalItemsContainerForDraggingInScreenSpace, threeDimensionalItemsContainerForDraggingInWorldSpace;
    
    private CanvasGroup canvasGroup;

    private GameObject twoDimensionalPrefab, threeDimensionalPrefab;
    private RectTransform twoDimensionalPrefabRectTransform;

    private IItem carryingItem;
    public int currentlyCarryingItemID { get; private set; } = -1;
    public int currentlySelectedInventoryID { get; private set; } = -1;

    // Start is called before the first frame update
    void Start() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Now we can instantiate stuff
            canvas = GameManager.Instance.mainCanvas; //Store the canvas in the scene so we can get its dimensions
            twoDimensionalItemsContainerForDraggingInScreenSpace = GameObject.FindGameObjectWithTag("2DItemsContainerForDraggingInCanvas");
            threeDimensionalItemsContainerForDraggingInWorldSpace = GameObject.FindGameObjectWithTag("threeDimensionalItemsContainerForDraggingInWorldSpace");
        }
        
    }
    List<KeyCode> numericKeys = new List<KeyCode>() { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };
    
    // Update is called once per frame
    void Update() {
        try {
            //Only allow this to happen if the crafting menu isn't open
            if (!CraftingUITabsManager.Instance.gameObject.activeSelf) {
                OnDragBegin();
                OnDrag();
                OnDragEnd();

            }
        }
        catch {
            //If that did not work, that means the crafting menu hasn't opened yet, we can just act regular
            OnDragBegin();
            OnDrag();
            OnDragEnd();
        }
        
    }


    private void OnDragBegin() {
        //These are all the OnDragBegin loops - loop through each key and set it to connect with a quick inventory slot
        for (int i = 0; i < numericKeys.Count; i++) {
            //Here i is technically the index of our inventory slot as well!
            if (Input.GetKeyDown(numericKeys[i])) {
                if (currentlySelectedInventoryID == i) {
                    //We already selected this, so let's just unselect it and do nothing else
                    ClearHand();
                    return;
                }
                if (currentlyCarryingItemID > -1) {
                    //We were carrying smth, destroy the prefabs to get ready for a new one
                    ClearHand();
                }
                currentlySelectedInventoryID = i;
                //Now let's select the new item by getting it from the inventory at that position
                currentlyCarryingItemID = InventoryManager.Instance.getItemIDAtSlotLocation(currentlySelectedInventoryID);
                if (currentlyCarryingItemID < 0) {
                    //We selected an empty object, so do nothing and just return
                    ClearHand(); //It's as if our hand is empty anyways
                    return;
                }
                //If we made it this far, let's instantiate the prefabs
                carryingItem = InGameItemsDatabaseManager.Instance.getItemByID(currentlyCarryingItemID);

                twoDimensionalPrefab = Instantiate(carryingItem.TwoDimensionalPrefab);
                IItem.attachItemInstance(twoDimensionalPrefab, currentlyCarryingItemID);
                //Probably check first to see if we really need hover text
                twoDimensionalPrefabRectTransform = twoDimensionalPrefab.GetComponent<RectTransform>();
                twoDimensionalPrefabRectTransform.SetParent(twoDimensionalItemsContainerForDraggingInScreenSpace.transform, false);
                canvasGroup = twoDimensionalPrefab.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0.75f; //Originally show the 2D item

                threeDimensionalPrefab = Instantiate(carryingItem.ThreeDimensionalPrefab);
                IItem.attachItemInstance(threeDimensionalPrefab, currentlyCarryingItemID);
                threeDimensionalPrefab.transform.SetParent(threeDimensionalItemsContainerForDraggingInWorldSpace.transform, false);
                threeDimensionalPrefab.SetActive(false); //Originally hide the 3D prefab

                //if we are dealing with a potion, resize the effect ring visually to match the radius
                if (carryingItem.itemType == "potion") {
                    float ringRadius = carryingItem.EffectRadius;
                    threeDimensionalPrefab.transform.GetChild(2).transform.localScale = new Vector3(ringRadius, ringRadius, ringRadius);
                }





                //A key was clicked and the slot of the item we need to grab is in slot i
                //TODO: add item in slot i to hand
                /*
                 * 
                 * Get item ID at slot id i
                 * temporarily remove count by one for that object and update changes in UI
                 * spawn the 2D prefab for that object and follow mouse position on screen (parent 2D dragging stuff)
                 * spawn 3D prefab for that object and follow at math point on scene
                 * alternate visibilities on update frame depending on whether we ahve 
                 * 
                 */
            }
        }

    }
    private void OnDrag() {
        //This is the OnDrag loop
        if (currentlyCarryingItemID > -1) {
            //We are carrying something, let's update its position and check if droppable
            overInventoryUI = false;
            //Here we need to check whether we are on top of the inventory UI before we decide anything else
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();

            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(null);
            eventData.position = Input.mousePosition;
            gr.Raycast(eventData, results);
            foreach (RaycastResult result in results) {
                if (result.gameObject.name.ToLower().Contains("inventory")) {
                    overInventoryUI = true;
                }
                //Debug.Log(result.gameObject.name);
            }
            if (overInventoryUI) {
                //Then we need to disable the 3D one
                threeDimensionalPrefab.SetActive(false);
                canvasGroup.alpha = 0.75f; //Make it reappear

                //Position it at the right place
                translate2DPrefab();

                //And then not do any of the rest of the 3D physics checking
                return;
            }

            Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(twoDimensionalPrefabRectTransform);
            //Debug.Log(twoDimensionalPrefabRectTransform.position);
            //Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
            Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, GameManager.LAYER_DroppableGround)) {
                //Then we hit something in the droppable ground
                canvasGroup.alpha = 0f; //Make it completely transparent so that it APPEARS we are dragging the 3D object

                //Get distance to hit and make that our distance from camera
                var worldPointAtGround = hit.point;
                threeDimensionalPrefab.SetActive(true);
                if (carryingItem.GetType().IsSubclassOf(typeof(RawMaterial))) {
                    threeDimensionalPrefab.transform.position = worldPointAtGround + verticalOffsetFor3DDrop;
                }
                else {
                    threeDimensionalPrefab.transform.position = worldPointAtGround;
                }
            }
            else {
                //We are hovering over something that is not a droppable ground
                //Debug.Log("Oops we missed the ground");
                threeDimensionalPrefab.SetActive(false);
                canvasGroup.alpha = 0.75f; //Make it reappear
                                           //Position it at the right place



            }
            //Move the 2D object regardless since its position defines where the 3D object falls
            translate2DPrefab();
        }
    }
    private void OnDragEnd() {
        //This is the OnDragEnd loop
        if (currentlyCarryingItemID > -1 && Input.GetMouseButtonDown(0)) {
            //We were carrying something and we clicked, if droppable point then drop, else do nothing
            if (overInventoryUI) {
                Destroy(threeDimensionalPrefab);
                //Then us clicking needs to actually drop the item into the correct inventory slot
                //TODO: Get attached inventory slot ID and swap places with currently selected ones, then reset 
            }

            Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(twoDimensionalPrefabRectTransform);

            //Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
            Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, GameManager.LAYER_DroppableGround)) {
                //Then we hit something in the droppable ground
                //Get distance to hit and make that our distance from camera
                var worldPointAtGround = hit.point;

                //Now activate the prefab, remove one of these from inventory, and then destroy this
                threeDimensionalPrefab.SetActive(true);
                //Debug.Log(item.GetType());
                //If we are dropping a raw material (whether it is for building or crafting)
                if (carryingItem.GetType().IsSubclassOf(typeof(RawMaterial))) {
                    threeDimensionalPrefab.transform.position = worldPointAtGround + verticalOffsetFor3DDrop;

                    /*//Let's make it pick-up able again
                    IItem.enableScript<ClickAddInventory>(threeDimensionalPrefab);*/

                    //make it do damage and get destroyed when it hits an enemy
                    IItem.enableScript<Attack3DDroppableItem>(threeDimensionalPrefab);
                    //Now enable the gravity
                    threeDimensionalPrefab.GetComponent<Rigidbody>().useGravity = true;
                }
                else if (carryingItem.itemType == "Potion") {
                    threeDimensionalPrefab.transform.position = worldPointAtGround;
                    SpellAction spellActionScript = threeDimensionalPrefab.GetComponent<SpellAction>();
                    spellActionScript.EnableSpell();
                }
                else {
                    //Whoops how did we get here
                }

                //Instead of removing by ID, we want to remove from that location
                //When we remove by ID it will remove the first instance of that object and then some weird glitch will happen where it comes back from another one
                //InventoryManager.Instance.removeByID(itemID);
                InventoryManager.Instance.removeAtSlotLocation(currentlySelectedInventoryID);
                InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges(); //Don't forget to update the inventory UI

                //Since we are dropping the item, we want to keep the 3D prefab alive but kill the rest
                DropHand();
            }
            else {
                //We are clicking somewhere that is not droppable ground, clear the hand and then refresh the UI
                ClearHand();
                InventoryManager.Instance.UpdateInventoryUIToReflectInternalInventoryChanges();
            }

        }
    }


    private void translate2DPrefab() {
        //Input.mousePosition has (0,0) and the bottom left corner and the rest are screen coordinates
        Vector2 localPoint;
        //Convert screen coordinates to local coordinates within overarching rectangle
        //Since canvas uses screen space overlay, we set the screen camera to null since we don't have one
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        twoDimensionalPrefabRectTransform.localPosition = localPoint; //Maybe see if we can linearly interpolate this
    }


    private Vector2 getViewportCoordinatesOf2DRectTransform(RectTransform rectTransform) {
        //rectTransform.position gives us screen coordinates

        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 canvasDimensions = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height) * canvas.scaleFactor;

        //Normalize screen coordinates to get viewport coordinates
        Vector2 viewportCoordinates = new Vector2(
            x: rectTransform.position.x / canvasDimensions.x,
            y: rectTransform.position.y / canvasDimensions.y
            );
        return viewportCoordinates;
    }

    private Vector3 verticalOffsetFor3DDrop = new Vector3(0, 6, 0);

    private bool overInventoryUI;
    public void ClearHand() {
        Destroy(twoDimensionalPrefab);
        twoDimensionalPrefab = null;
        Destroy(threeDimensionalPrefab);
        threeDimensionalPrefab = null;
        currentlyCarryingItemID = -1;
        currentlySelectedInventoryID = -1;
    }
    private void DropHand() {
        Destroy(twoDimensionalPrefab);
        //Destroy(threeDimensionalPrefab);
        currentlyCarryingItemID = -1;
        currentlySelectedInventoryID = -1;
    }
}
