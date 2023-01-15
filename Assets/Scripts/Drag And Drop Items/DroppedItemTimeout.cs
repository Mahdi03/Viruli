using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemTimeout : MonoBehaviour {

    private float timeElapsed;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start() {
        canvasGroup = GetComponent<CanvasGroup>(); //Try to get the canvas group before we make one so we don't keep making one
        if (canvasGroup == null) {
            canvasGroup = (CanvasGroup)gameObject.AddComponent<CanvasGroup>();
        }
    }

    // Update is called once per frame
    void Update() {
        timeElapsed += Time.smoothDeltaTime;
        if (timeElapsed > 10f) {
            Destroy(gameObject); //Time out the pick up item correctly
        }
        canvasGroup.alpha =
            Mathf.Abs(
                Mathf.Sin(
                    2 * Mathf.Pow(Mathf.Exp(timeElapsed / 13f), 4)
                )
            );
    }
}
