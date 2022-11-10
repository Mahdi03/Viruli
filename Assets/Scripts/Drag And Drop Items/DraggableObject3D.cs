using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DraggableObject3D : MonoBehaviour {
	private float mouseZCoordinate;
	private Vector3 mouseOffset;
	private void OnMouseDown() {
		//throw new System.NotImplementedException();
		//Debug.Log("Are we being called?");
		
		mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

		mouseOffset = gameObject.transform.position - GetMousePositionInWorldCoordinates();


	}
	private Vector3 GetMousePositionInWorldCoordinates() {
        Vector3 mousePos = Input.mousePosition;
		mousePos.z = mouseZCoordinate;

		return Camera.main.ScreenToWorldPoint(mousePos);
    }

	private void OnMouseDrag() {
		transform.position = GetMousePositionInWorldCoordinates() + mouseOffset;
		//throw new System.NotImplementedException();
	}
	private void OnMouseUp() {
		
	}
	private void Update() {
		Vector3 pos = transform.position;
		pos.y = 2;
		transform.position = pos;
	}
}