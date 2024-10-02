using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


[System.Serializable]
public class Message
{
    public string sender;
    public string messageText;
    public Timestamp timestamp;

    public Message(string sender, string messageText, Timestamp timestamp)
    {
        this.sender = sender;
        this.messageText = messageText;
        this.timestamp = timestamp;
    }
}

public class ChannelManager : MonoBehaviour
{

    public GameObject sendButtonObj; // Assign the prefab in the Unity Inspector

    public GameObject messagePrefab; // Assign the prefab in the Unity Inspector
    public Transform messageParent; // Assign the parent transform for the message UI elements
    public TMP_InputField messageInputField;

    private void Awake()
    {
        if (FirebaseManager.Instance.currentUserData.accountType.Equals("student"))
        {
            sendButtonObj.SetActive(false);
            messageInputField.gameObject.SetActive(false);
        }
        // Start the GetAllMessages coroutine on awake
        StartCoroutine(GetAllMessagesCoroutine());
    }

    // Coroutine to retrieve all messages
    private IEnumerator GetAllMessagesCoroutine()
    {
        // Run the async task inside the coroutine
        Task getAllMessagesTask = GetAllMessages();

        // Wait until the task is completed
        yield return new WaitUntil(() => getAllMessagesTask.IsCompleted);

        // You can check for exceptions if needed
        if (getAllMessagesTask.IsFaulted)
        {
            Debug.LogError("Failed to retrieve messages.");
        }
        else
        {
            Debug.Log("Messages loaded successfully.");
        }
    }


    public async Task GetAllMessages()
    {
        CollectionReference messagesRef = FirebaseManager.Instance.firestore.Collection("messages");
        QuerySnapshot snapshot = await messagesRef.OrderBy("timestamp").GetSnapshotAsync();
        // First, destroy all existing children of messageParent
        foreach (Transform child in messageParent)
        {
            Destroy(child.gameObject); // Destroy each child (message)
        }
        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            if (doc.Exists)
            {
                string sender = doc.GetValue<string>("sender");
                string messageText = doc.GetValue<string>("messageText");
                Timestamp timestamp = doc.GetValue<Timestamp>("timestamp");

                // Instantiate the message prefab
                GameObject messageObject = Instantiate(messagePrefab, messageParent);

                // Assuming the prefab has a script to set the message text (MessageUI script)
                MessageUI messageUI = messageObject.GetComponent<MessageUI>();
                messageUI.SetMessage(sender, messageText);
            }
        }
    }

    // Method to trigger SendMessageCoroutine on a button click
    public void OnSendButtonClick()
    {
        // Start the coroutine to send the message when the button is clicked
        StartCoroutine(SendMessageCoroutine());
    }

    // Coroutine to send a message when called
    public IEnumerator SendMessageCoroutine()
    {
        // Run the SendMessage async task
        Task sendMessageTask = SendMessage();

        // Wait until the task is completed
        yield return new WaitUntil(() => sendMessageTask.IsCompleted);

        // You can check for exceptions if needed
        if (sendMessageTask.IsFaulted)
        {
            Debug.LogError("Failed to send message.");
        }
        else
        {
            Debug.Log("Message sent successfully.");
            StartCoroutine(GetAllMessagesCoroutine());

        }
    }

    public async Task SendMessage()
    {
        DocumentReference messagesRef = FirebaseManager.Instance.firestore.Collection("messages").Document();

        Message newMessage = new Message(FirebaseManager.Instance.currentUserData.email, messageInputField.text, Timestamp.GetCurrentTimestamp());

        // Convert message to a dictionary for Firestore
        Dictionary<string, object> messageData = new Dictionary<string, object>
         {
        { "sender", FirebaseManager.Instance.currentUserData.email },
        { "messageText", messageInputField.text },
        { "timestamp", newMessage.timestamp }
    };

        await messagesRef.SetAsync(messageData);
        Debug.Log("Message sent successfully.");
    }
}
