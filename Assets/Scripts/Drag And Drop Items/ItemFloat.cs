using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFloat : MonoBehaviour {
    private float mainY = 0, /*rotationSpeed = 35f,*/ timeElasped = 0f;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        mainY = rectTransform.position.y;
    }

	// Update is called once per frame
	void Update() {
        timeElasped += Time.deltaTime;
        /*
        //Rotate indefinitely
        var angle = transform.rotation.eulerAngles;
        angle.y += Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Euler(angle);
        */
        //Bobbing effect
        var pos = rectTransform.position;
        pos.y = mainY + 20f * Mathf.Sin(2 * timeElasped);
        rectTransform.position = pos;
    }
}
