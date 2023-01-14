using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageSystemScrollViewElementController : MonoBehaviour {

    public TextMeshProUGUI messageText;
    public GameObject timestampText;
    private TextMeshProUGUI timestampTextText;
    private RectTransform timestampTextRectTransform;

    private void Start() {
        timestampTextText = timestampText.GetComponent<TextMeshProUGUI>();
        timestampTextRectTransform = timestampText.GetComponent<RectTransform>();
    }

    public void SetMessageText(string message) {
        messageText.text = message;
    }
    public void SetTimestampText(string timestamp) {
        Start();
        timestampTextText.text = timestamp;
        timestampTextRectTransform.sizeDelta = new Vector2(timestampTextText.preferredWidth, timestampTextRectTransform.sizeDelta.y);
    }
}
