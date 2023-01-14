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


    private List<Message> messages = new List<Message>();

    // Start is called before the first frame update
    void Start() {
        messageSystemUIController = messageSystemUI.GetComponent<MessageSystemUIController>();
    }

    private void alertMessage(string message) {
        messageSystemUIController.alert(message); //Pass it on to the UI logic
    }

    public void PostMessage(string message, bool muted = false, bool alert = false, string timestamp = "") {
        Message newMessage = new Message(message, muted, alert, timestamp);
        if (alert) {
            Debug.Log(message);
            alertMessage(message);
        }
        messages.Add(newMessage);
        Debug.Log(messages);
        messageSystemUIController.PostMessage(newMessage);
    }

    public void ToggleMessageBoardVisibility() {
        messageSystemUIController.ToggleMessageBoardVisibility();
    }

    public string SaveMessages() {
        Debug.Log(messages.ToArray());
        return JsonHelper.ToJson(messages.ToArray());
    }

    public void LoadMessages(string allMessagesJSONString) {
        Start(); //Make sure that messageSystemUIController is defined since we are calling this function directly from game manager on game start
        Message[] messagesArr = JsonHelper.FromJson<Message>(allMessagesJSONString);
        foreach (Message msg in messagesArr) {
            //Debug.Log(msg.timestamp.ToShortTimeString());
            PostMessage(msg.message, true, false, msg.timestamp);
        }
    }

}

[Serializable]
public struct Message {
    public string message;
    public string timestamp;
    public bool muted, alert;
    public Message(string message, bool muted = false, bool alert = false, string timestamp = "") {
        this.message = message;
        if (timestamp == "") { this.timestamp = DateTime.Now.ToShortTimeString(); }
        else { this.timestamp = timestamp; }
        this.muted = muted;
        this.alert = alert;
    }
};