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
            System.Object newMessage = playerState.messageQueue.Dequeue();
            resolveMessage((lobbyMessage)newMessage);
        }

        checkingForMessages = false;
    }


    private void resolveMessage(lobbyMessage message)
    {
        switch(message.msgEvent)
        {
            case "searchingForSession":
                Debug.Log("Searching for Session...");
                break;
            case "joinedSession":
                Debug.Log("Joined Session: " + message.sessionId);
                playerState.sessionId = message.sessionId;
                break;
            case "sessionStart":
                sceneManager.changeScene(message.sessionRole);
                break;
            default:
                Debug.Log("No matching event found for Lobby...");
                break;
        }
    }
}

public class lobbyMessage
{
    public string sessionId { get; set; }
    public string msgEvent { get; set; }
    public string sessionRole { get; set; }
}

public class newPlayerRequest
{
    public string playerName { get; set; } = "";
    public string deviceID { get; set; } = "";
}

public class joinSessionRequest
{
    public string reqEvent { get; set; } = "joinSession";
    public string playerId { get; set; } = "";
}
