using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobbyManager : MonoBehaviour
{
    private bool checkingForMessages = false;
    private bool joinedSession = false;
    // Start is called before the first frame update
    void Start()
    {
        var playerRequest = new newPlayerRequest();
        string response = "";

        playerRequest.playerName = SystemInfo.deviceUniqueIdentifier;
        playerRequest.deviceID = SystemInfo.deviceUniqueIdentifier;

        try 
        {
            response = NetworkMessaging.SendJsonViaPOST(playerRequest, "http://localhost:8095/newPlayer").ToString();
        }
        catch (SystemException e) 
        {
            Debug.Log("error was: " + e);
        }

        playerState.playerId = response;
        NetworkMessaging.ConnectWebSocketToServerAsync("ws://localhost:8095/connect");
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMessaging.socketOpen())
        {
            if (!joinedSession)
            {
                var joinSessionReq = new joinSessionRequest();
                joinSessionReq.playerId = playerState.playerId;
                NetworkMessaging.SendSocketMessage(joinSessionReq);
                joinedSession = true;
            }
            else if (!checkingForMessages)
            {
                checkingForMessages = true;
                checkForMessage();
            }
        }
    }

    private void checkForMessage() 
    {
        if(playerState.messageQueue.Count > 0)
        {
            
        }

        checkingForMessages = false;
    }

    public void enterSender() => sceneManager.changeScene("Sender");
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

public class joinSessionRequest
{
    public string reqEvent { get; set; } = "joinSession";
    public string playerId { get; set; }
}
