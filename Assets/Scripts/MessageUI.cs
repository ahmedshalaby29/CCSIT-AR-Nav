using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MessageUI : MonoBehaviour
{
    public TextMeshProUGUI senderText;
    public TextMeshProUGUI messageText;

    public void SetMessage(string sender, string message)
    {
        senderText.text = sender;
        messageText.text = message;
    }
}
