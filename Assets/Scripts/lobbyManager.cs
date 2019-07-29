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
        var playerRequest = new newPlayerRequest();

        playerRequest.playerName = SystemInfo.deviceUniqueIdentifier;
        playerRequest.deviceID = SystemInfo.deviceUniqueIdentifier;

        string temp = NetworkMessaging.SendJsonViaPOST(playerRequest, "http://localhost:8095/newPlayer").ToString();
        Debug.Log("Response was: " + temp);
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

    public void testJsonPOST()
    {
        NetworkMessaging.ServerTest();
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

    public class newPlayerRequest
{
    public string playerName;
    public string deviceID;

    public newPlayerRequest()
    {
        playerName = "";
        deviceID = "";
    }

    public void setPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

    public void setDeviceID(string deviceID)
    {
        this.deviceID = deviceID;
    }
}
