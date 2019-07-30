using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class senderManager : MonoBehaviour
{
    List<ActionMessage> message_list = new List<ActionMessage>();
    ActionMessage next_message = new ActionMessage();

    Dropdown step_dropdown;
    Dropdown noun_dropdown;
    Dropdown verb_dropdown;

    private bool checkingForMessages = false;
    private bool receiverInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        step_dropdown = GameObject.Find("step_dropdown").GetComponent<Dropdown>();
        verb_dropdown = GameObject.Find("verb_dropdown").GetComponent<Dropdown>();
        noun_dropdown = GameObject.Find("noun_dropdown").GetComponent<Dropdown>();

        Debug.Log("Connecting to web socket");
        if (!NetworkMessaging.socketOpen())
        {
            NetworkMessaging.ConnectWebSocketToServerAsync("ws://localhost:8095/connectdemo");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMessaging.socketOpen())
        {
            Debug.Log("Socket is open");
            if (!receiverInitialized)
            {
                // Need to generate answer and cypher
                NetworkMessaging.SendJsonViaPOST(roleSend, "http://localhost:8095/initreceiverdemo");
            }

            if (!checkingForMessages)
            {
                Debug.Log("Checking for messages");
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
            resolveMessage((clientEvent)newMessage);
        }

        checkingForMessages = false;
    }

    private void resolveMessage(clientEvent message)
    {
        switch(message.msgEvent)
        {
            case "connected":
                playerState.id = message.id;
                roleRequest roleSend = new roleRequest();
                roleSend.role = "Sender";
                roleSend.id = message.id;
                NetworkMessaging.SendJsonViaPOST(roleSend, "http://localhost:8095/roledemo");
                break;
            case "updateGuess":
                Component[] guessComponents;
                OutputMessage guessUpdate = new OutputMessage();
                guessUpdate.nouns = message.nouns;
                guessUpdate.verbs = message.verbs;
                guessUpdate.role = message.role;
                guessComponents = GetComponents<OutputManager>();

                foreach (OutputManager manager in guessComponents)
                    manager.updateGuessWindow(guessUpdate);
                break;
            case "correctGuesses":
                Component[] correctGuessComponents;
                OutputMessage correctGuessUpdate = new OutputMessage();
                correctGuessUpdate.correctGuesses = message.correctGuesses;
                break;
            case "gameOver":
                break;
            default:
                Debug.Log("No matching event found for scene...");
                break;
        }
    }

    public void send_message()
    {
        next_message.step = (step_dropdown.captionText.text.ToString());
        next_message.noun = (noun_dropdown.captionText.text.ToString());
        next_message.verb = (verb_dropdown.captionText.text.ToString());
        next_message.playerId = playerState.playerId;
        next_message.sessionId = playerState.sessionId;

        message_list.Add(next_message);

        try
        {
            NetworkMessaging.SendJsonViaPOST(next_message, "http://localhost:8095/sendmessagedemo");
        }
        catch (SystemException e)
        {
            Debug.Log(e.Message.ToString());
        }
    }
}
