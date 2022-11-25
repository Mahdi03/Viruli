using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObject3D : MonoBehaviour {
	private float mouseZCoordinate;
	private Vector3 mouseOffset;
	private void OnMouseDown() {
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
	}
	private void OnMouseUp() {

	}
	private void Update() {
		Vector3 pos = transform.position;
		pos.y = 2;
		transform.position = pos;
	}
}