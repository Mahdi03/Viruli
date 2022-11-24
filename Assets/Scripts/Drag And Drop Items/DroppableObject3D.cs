using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// This component will use the ItemInstance to get the itemID and necessary prefabs to instantiate
/// </summary>
[RequireComponent(typeof(ItemInstance))]
public class DroppableObject3D : MonoBehaviour, IDragHandler, IEndDragHandler {
    private Canvas canvas;
    private int itemID;

    
    private void Awake() {
        canvas = GameObject.FindObjectOfType<Canvas>(); //Store the canvas in the scene so we can get its dimensions
        this.itemID = GetComponent<ItemInstance>().itemID;
    }

    private Vector3 getWorldSpaceCoordinatesOf2DRectTransform(RectTransform rectTransform) {
        //Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
        //Vector2 screenCoordinates = 
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height) * canvas.scaleFactor;
        Debug.Log("Canvas Size:" + canvasSize);
        Vector3 screenPoint = rectTransform.TransformPoint(0,0,0); //This gives me the position of my object from where bottom left corner is (0, 0)
        Debug.Log("Hey: " + rectTransform.position);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        return screenPoint;
    }
    public void OnDrag(PointerEventData eventData) {
        RectTransform rectTransformOfDraggableObj = eventData.pointerDrag.GetComponent<RectTransform>();
        Vector3 worldSpace = getWorldSpaceCoordinatesOf2DRectTransform(rectTransformOfDraggableObj);
        Debug.Log(worldSpace);
    }

    public void OnEndDrag(PointerEventData eventData) {

    }

    

}
