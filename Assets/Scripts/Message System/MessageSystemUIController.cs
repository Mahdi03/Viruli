using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystemUIController : MonoBehaviour {
    [SerializeField]
    private GameObject messageButton;
    [SerializeField]
    private GameObject messageBoardUI;
    [SerializeField]
    private GameObject messageBoardButton;
    [SerializeField]
    private GameObject messageBoardButtonUnreadMessagesHighlight; //TODO: add a orange circle over the chat button

    public void PostMessage(Message message) {
        //All messages are added to the message board
        AddNewMessageToMessageBoardUI(message);
        if (!messageBoardUI.activeSelf && !message.muted) {
            //TODO: IF message board is inactive and we are not dealing with a muted message, show orange highlight on view messages button
            newUnreadMessages();
        }
    }

    private void newUnreadMessages() {
        //TODO: Switch chat logo to have orange highlight
        messageBoardButtonUnreadMessagesHighlight.SetActive(true);
    }
    private void readAllMessages() {
        //TODO: Switch chat logo to have no orange highlight
        messageBoardButtonUnreadMessagesHighlight.SetActive(false);
    }

    public void AddNewMessageToMessageBoardUI(Message message) {
        //TODO: append a new message to the bottom of the message board - UI work
        Debug.Log($"Message: {message.message}\n Timestamp: {message.timestamp}");
        //TODO: scroll all the way to the bottom        
    }

    public void MinimizeMessageBoardUI() {
        //TODO: Hide message board and show message button
        messageButton.SetActive(true);
        messageBoardUI.SetActive(false);
        messageBoardButton.SetActive(false);
    }
    public void OpenMessageBoardUI() {
        //Remove message button highlight
        readAllMessages();
        //TODO: Show message board and hide message button
        messageButton.SetActive(false);
        messageBoardUI.SetActive(true);
        messageBoardButton.SetActive(true);
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
