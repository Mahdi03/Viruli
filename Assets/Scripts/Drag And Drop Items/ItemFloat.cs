using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloat : MonoBehaviour {

	private float mainY = 0, timeElasped = 0f;
	private RectTransform rectTransform;

	// Start is called before the first frame update
	void Start() {
		rectTransform = GetComponent<RectTransform>();
		mainY = rectTransform.position.y;
	}

	// Update is called once per frame
	void Update() {
		timeElasped += Time.deltaTime;

		//Bobbing effect
		var pos = rectTransform.position;
		pos.y = mainY + 15f * Mathf.Sin(2 * timeElasped);
		rectTransform.position = pos;
	}
}
