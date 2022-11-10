using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IDraggableObject : IPointerDownHandler,
	/*Drag Events*/
	IBeginDragHandler, IEndDragHandler, //For begin and end drag events
	IDragHandler //For while dragging (called each frame)
{}

