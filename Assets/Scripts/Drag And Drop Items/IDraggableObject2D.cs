using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This interface inherits from all the interfaces that deal with 2D dragging for simplicity
/// </summary>
public interface IDraggableObject2D :
	/*Drag Events*/
	IBeginDragHandler, IEndDragHandler, //For begin and end drag events
	IDragHandler //For while dragging (called each frame)
{}

