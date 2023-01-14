using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertMessageController : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI alertText;
    private CanvasGroup canvasGroup;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void SetAlertText(string alertMessage) {
        alertText.text = alertMessage;
    }

    public void ShowAlert() {
        Awake(); //run it just in case it has not already been run
        //Fade in
        StartCoroutine(fadeIn());
    }

    private IEnumerator fadeIn() {
        
        canvasGroup.alpha += 0.25f;
        if (canvasGroup.alpha < 1f) {
            //Debug.Log("uhm1.5");
            yield return new WaitForSecondsRealtime(0.01f);
            //Debug.Log("uhm1");
            StartCoroutine(fadeIn());
        }
        else {
            Debug.Log("uhm2");
            yield return new WaitForSecondsRealtime(5f);
            HideAlert();
        }
        
        
    }
    private IEnumerator waitForUserRead() {
        yield return new WaitForSeconds(5f);
        HideAlert();
    }
    private IEnumerator fadeOut() {
        canvasGroup.alpha -= 0.25f;
        if (canvasGroup.alpha > 0) {
            yield return new WaitForSecondsRealtime(0.01f);
            StartCoroutine(fadeOut());
        }
        else {
            //We are done here, destroy the alert (it is saved in the message board w/ the timestamp too)
            Destroy(gameObject);
        }
    }

    private void HideAlert() {
        //Fade out
        StartCoroutine(fadeOut());
    }

    //TODO: Fade in this object through canvas group opacity, and then keep on screen for 5 sec, and then fade out + self destruct


}
