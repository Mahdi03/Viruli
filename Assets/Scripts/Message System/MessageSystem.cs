using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageSystem : MonoBehaviour {

    private static MessageSystem instance;
    public static MessageSystem Instance { get { return instance; } }

    [SerializeField]
    private GameObject messageSystemUI;
    private MessageSystemUIController messageSystemUIController;

    private void Awake() {
        //Singleton initialization code
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Now we can initialize stuff
        }
    }


    private Queue<Message> messages = new Queue<Message>();

    // Start is called before the first frame update
    void Start() {
        messageSystemUIController = messageSystemUI.GetComponent<MessageSystemUIController>();
    }

    public void PostMessage(string message, bool muted = false, bool alert = false) {
        Message newMessage = new Message(message, muted, alert);
        messages.Enqueue(newMessage); //We technically don't even need to store the messages in a queue
        messageSystemUIController.PostMessage(newMessage);
    }

    public void ToggleMessageBoardVisibility() {
        messageSystemUIController.ToggleMessageBoardVisibility();
    }

}

public struct Message {
    public string message;
    public DateTime timestamp;
    public bool muted, alert;
    public Message(string message, bool muted = false, bool alert = false) {
        this.message = message;
        timestamp = DateTime.Now;
        this.muted = muted;
        this.alert = alert;
    }
};