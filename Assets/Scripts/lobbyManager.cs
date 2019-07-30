using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobbyManager : MonoBehaviour
{
    private bool checkingForMessages = false;
    // Start is called before the first frame update
    void Start()
    {
        var playerRequest = new newPlayerRequest();
        string response = "";

        playerRequest.playerName = SystemInfo.deviceUniqueIdentifier;
        playerRequest.deviceID = SystemInfo.deviceUniqueIdentifier;
/*
        try 
        {
            response = NetworkMessaging.SendJsonViaPOST(playerRequest, "http://localhost:8095/newPlayer").ToString();
        }
        catch (SystemException e) 
        {
            Debug.Log("error was: " + e);
        }

        playerState.playerId = response;
*/
        NetworkMessaging.ConnectWebSocketToServerAsync("ws://localhost:8095/joinSession");
        while(!NetworkMessaging.socketOpen())
        {
            // do nothing :(
        }
        NetworkMessaging.SendSocketMessage("3");
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
