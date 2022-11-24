using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// This component will use the ItemInstance to get the itemID and necessary prefabs to instantiate
/// </summary>
[RequireComponent(typeof(ItemInstance))]
public class DroppableObject3D : MonoBehaviour, IDragHandler, IEndDragHandler {
    private Canvas canvas;
    private int itemID;
    GameObject sphere;
    
    private void Awake() {
        canvas = GameObject.FindObjectOfType<Canvas>(); //Store the canvas in the scene so we can get its dimensions
        this.itemID = GetComponent<ItemInstance>().itemID;

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Renderer>().material.color = Color.green;
    }

    private Vector2 getViewportCoordinatesOf2DRectTransform(RectTransform rectTransform) {
        //rectTransform.position gives us screen coordinates
        
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 canvasDimensions = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height) * canvas.scaleFactor;

        //Normalize screen coordinates to get viewport coordinates
        Vector2 viewportCoordinates = new Vector2(
            x:rectTransform.position.x / canvasDimensions.x,
            y:rectTransform.position.y / canvasDimensions.y
            );
        return viewportCoordinates;
    }
    public void OnDrag(PointerEventData eventData) {
        RectTransform rectTransformOfDraggableObj = eventData.pointerDrag.GetComponent<RectTransform>();
        //We don't need to check if we are over the inventory or anything like that because we are only in the middle of a drag event right now
        
        Vector2 viewportCoordinates = getViewportCoordinatesOf2DRectTransform(rectTransformOfDraggableObj);

        //Send a ray through that viewport point, if we hit the ground layermask then we can continue, else we need to disable
        Ray ray = Camera.main.ViewportPointToRay(viewportCoordinates);
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, 1<<3)) { //Use bit shift by 3 to get 3rd layer
            //Then we hit something in the droppable ground
            Debug.Log("Hovering over ground");
            //Get distance to hit and make that our distance from camera
            var worldPointAtGround = hit.point;
            sphere.transform.position = worldPointAtGround; //Place ring here


            //Debug.DrawRay(Camera.main.transform.position, ray.direction * hit.distance, Color.blue, 10, false);
            //Gizmos.DrawRay(ray.origin, ray.direction * hit.distance);
        }
        else {
            //We are hovering over something that is not a droppable ground
            Debug.Log("Oops we missed the ground");

        }
        
    }

    public void OnEndDrag(PointerEventData eventData) {
        RectTransform rectTransformOfDraggableObj = eventData.pointerDrag.GetComponent<RectTransform>();
        //It looks like the OnDrop event handler doesn't even allow this to run
        //Here we need to check whether we are on top of the inventory UI before we decide anything else
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(eventData, results);
        foreach (RaycastResult result in results) {
            Debug.Log(result.ToString());
        }
        Debug.Log("woah");

    }

    

}
