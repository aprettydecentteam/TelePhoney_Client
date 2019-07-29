using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobbyManager : MonoBehaviour
{
    private bool checkingForMessages = false;
    // Start is called before the first frame update
    void Start()
    {
        NetworkMessaging.ConnectWebSocketToServerAsync();
        Debug.Log("Opening socket connection");
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMessaging.socketOpen() && checkingForMessages == false)
        {
            checkingForMessages = true;
            Debug.Log("Socket is open. Checking for messages...");
            checkForMessage();
        }
    }

    public void testSocketMessage() 
    {
        NetworkMessaging.SendSocketMessage("This is a test message");
    }

    private async void checkForMessage() 
    {
        string message = await NetworkMessaging.CheckSocketMessage();

        if ( message != null && message != "" )
        {
            Debug.Log("Message was: " + message);
        }
        checkingForMessages = false;
    }
}
