using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMechanic : MonoBehaviour
{
    public InputField inputBoxLegacy;
    public static bool isInputActive = false; // start mati

    public int maxMessage = 5   ;
    public GameObject chatPanel, textObject;
    public InputField inputBox;

    public Color playerMessage, info;

    [SerializeField]
    private List<Message> messageList = new List<Message>();


    //incase kalau nk guna
    private int startGameCallCount = 0;  // Counter for StartGame calls
    private const int startGameCallLimit = 3;  // Limit for StartGame calls
    //add  nie


    public GameObject SpawnBoxOne;

    void Start()
    {
        if (inputBoxLegacy != null)
        {
            inputBoxLegacy.gameObject.SetActive(false); // start mati
            isInputActive = false;
        }

        if (chatPanel == null)
        {
            chatPanel = GameObject.FindWithTag("ChatPanel");
            if (chatPanel == null)
            {
                Debug.LogError("chatPanel is not assigned and could not be found with the tag 'ChatPanel'.");
            }
        }

        if (textObject == null)
        {
            Debug.LogError("textObject is not assigned.");
        }

        if (inputBox == null)
        {
            inputBox = FindObjectOfType<InputField>();
            if (inputBox == null)
            {
                Debug.LogError("inputBox is not assigned and could not be found dynamically.");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isInputActive) // kalau ad perkataan
            {
                if (!string.IsNullOrEmpty(inputBox.text)) // klau ad perkataan dia hantar ke SendMessageToChat
                {
                    SendMessageToChat(inputBox.text, Message.MessageType.playerMessage);
                    inputBox.text = "";
                }
                inputBox.DeactivateInputField();
                isInputActive = false;
            }
            else
            {
                ToggleInputField(); // kalau tak ada perkataan
            }
        }

        // Call the method to recognize messages and execute specific functions
        RecognizeMessages();

    }

    public void ToggleInputField() // klau tak ada perkataan (dia fikir input box mati)
    {
        if (inputBoxLegacy != null) //check wujud ke tk
        {
            isInputActive = !isInputActive;
            inputBoxLegacy.gameObject.SetActive(isInputActive);

            if (isInputActive) // kalau mati hidupkan
            {
                inputBoxLegacy.Select();
                inputBoxLegacy.ActivateInputField();
            }
            else // kalau hidup matikan
            {
                inputBoxLegacy.DeactivateInputField();
                inputBoxLegacy.gameObject.SetActive(false);
            }
        }
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if (chatPanel == null || textObject == null)
        {
            Debug.LogError("chatPanel or textObject is not assigned.");
            return;
        }

        if (messageList.Count >= maxMessage)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.RemoveAt(0);
        }

        Message newMessage = new Message { text = text };

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        Text newTextComponent = newText.GetComponent<Text>() ?? newText.GetComponentInChildren<Text>();
        if (newTextComponent == null)
        {
            Debug.LogError("No Text component found on the instantiated textObject or its children.");
            return;
        }

        newMessage.textObject = newTextComponent;
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        return messageType == Message.MessageType.playerMessage ? playerMessage : info;
    }


    //contoh2 function yang dia boleh pakai
    // New method to recognize messages and run specific functions
    void RecognizeMessages()
    {
        foreach (Message message in messageList)
        {


            //testing area
            if (message.text.Contains("spawn"))
            {
                // Run the StartGame function
                SpawnStuff();
            }
            //


            else if (message.text.Contains("start"))
            {
                // Run the StartGame function
                StartGame();
            }
            else if (message.text.Contains("stop"))
            {
                // Run the StopGame function
                StopGame();
            }
            else if (message.text.Contains("jump"))
            {
                // Run the Jump function
                Jump();
            }
            // Add more conditions and functions as needed

            else
            {
                WrongComment();
            }
        }
    }

    void StartGame()
    {
        if (startGameCallCount < startGameCallLimit)
        {
            Debug.Log("Starting game...");
            // Implement your start game logic here

            startGameCallCount++;  // Increment the call counter
        }
        else
        {
            Debug.Log("StartGame has already been called the maximum number of times.");
        }
    }


    //testing area
    void SpawnStuff()
    {
        if (SpawnBoxOne != null && !SpawnBoxOne.activeSelf)
        {
            SpawnBoxOne.SetActive(true);
            Debug.Log("spawning kapal testing");
        }
    }


    void WrongComment()
    {
        Debug.Log("wrong comment");
    }
    //


    void StopGame()
    {
        Debug.Log("Stopping game...");
        // Implement your stop game logic here
    }

    void Jump()
    {
        Debug.Log("Jumping...");
        // Implement your jump logic here
    }



}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public enum MessageType
    {
        playerMessage,
        info
    }
}
