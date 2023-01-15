using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSystemUIController : MonoBehaviour {
    [SerializeField]
    private GameObject messageButton;
    [SerializeField]
    private GameObject messageButtonUnreadMessagesHighlight; //add a orange circle over the chat button

    [SerializeField]
    private GameObject messageBoardUI;
    private ScrollRect messageBoardUIScrollRect;

    [SerializeField]
    private GameObject messageBoardScrollViewElementPrefab;

    [SerializeField]
    private Transform alertQueueGameObjectTransform;
    [SerializeField]
    private GameObject alertMessageTextboxPrefab;

    // Start is called before the first frame update
    void Start() {
        messageBoardUIScrollRect = messageBoardUI.GetComponent<ScrollRect>();
    }

    public void PostMessage(Message message) {
        //All messages are added to the message board
        AddNewMessageToMessageBoardUI(message);
        if (!messageBoardUI.activeSelf && !message.muted) {
            //IF message board is inactive and we are not dealing with a muted message, show orange highlight on view messages button
            newUnreadMessages();
        }
    }

    private void newUnreadMessages() {
        //Switch chat logo to have orange highlight
        messageButtonUnreadMessagesHighlight.SetActive(true);
    }
    private void readAllMessages() {
        //Switch chat logo to have no orange highlight
        messageButtonUnreadMessagesHighlight.SetActive(false);
    }

    public void AddNewMessageToMessageBoardUI(Message message) {
        //TODO: append a new message to the bottom of the message board - UI work
        var newMessageBoardScrollViewElement = Instantiate(messageBoardScrollViewElementPrefab, messageBoardUI.transform.GetChild(0).GetChild(0));
        MessageSystemScrollViewElementController newMessageBoardScrollViewElementController = newMessageBoardScrollViewElement.GetComponent<MessageSystemScrollViewElementController>();
        newMessageBoardScrollViewElementController.SetMessageText(message.message);
        newMessageBoardScrollViewElementController.SetTimestampText(message.timestamp);

        //Debug.Log($"Message: {message.message}\n Timestamp: {message.timestamp}");

        //scroll all the way to the bottom        
        scrollToBottomOfMessageBoardUI();
    }

    public void alert(string message) {
        //Alert the message on the screen
        var newAlertMessage = Instantiate(alertMessageTextboxPrefab, alertQueueGameObjectTransform);
        AlertMessageController newAlertMessageController = newAlertMessage.GetComponent<AlertMessageController>();
        newAlertMessageController.SetAlertText(message);
        newAlertMessageController.ShowAlert();
    }

    public void ToggleMessageBoardVisibility() {
        messageBoardUI.SetActive(!messageBoardUI.activeSelf);
        if (messageBoardUI.activeSelf) {
            //Remove message button highlight
            readAllMessages();
            scrollToBottomOfMessageBoardUI();
        }
    }

    private void scrollToBottomOfMessageBoardUI() {
        if (messageBoardUIScrollRect == null) {
            Start();
        }
        messageBoardUIScrollRect.normalizedPosition = new Vector2(0, 0); //Scrolls to bottom
    }


    
}
