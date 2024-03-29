﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverManager : MonoBehaviour
{
    List<GuessMessage> message_list = new List<GuessMessage>();
    GuessMessage next_message = new GuessMessage();

    public Dropdown [] verb_Guess;
    public Dropdown [] noun_Guess;

    private bool checkingForMessages = false;

    // Start is called before the first frame update
    void Start()
    {
        playerState.playerRole = "Receiver";
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
                roleSend.role = "Receiver";
                roleSend.id = message.id;
                NetworkMessaging.SendJsonViaPOST(roleSend, "http://35.209.52.72:80/roledemo");
                break;
            case "sentMessage":
                Component[] messageComponents;
                OutputMessage clueUpdate = new OutputMessage();
                clueUpdate.noun = message.noun;
                clueUpdate.verb = message.verb;
                clueUpdate.step = message.step;
                messageComponents = GetComponents<OutputManager>();

                foreach (OutputManager manager in messageComponents)
                    manager.updateSendWindow(clueUpdate);
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
            case "initCypher":
                Component[] cypherComponents;
                OutputMessage cypherUpdate = new OutputMessage();
                cypherUpdate.nouns = message.nouns;
                cypherUpdate.verbs = message.verbs;
                cypherComponents = GetComponents<OutputManager>();

                foreach (OutputManager manager in cypherComponents)
                    manager.updateCipherWindow(cypherUpdate);
                break;
            case "correctGuesses":
                Component[] correctGuessComponents;
                OutputMessage correctGuessUpdate = new OutputMessage();
                correctGuessUpdate.correctGuesses = message.correctGuesses;
                correctGuessComponents = GetComponents<OutputManager>();

                foreach (OutputManager manager in correctGuessComponents)
                    manager.updateCorrectGuessWindow(correctGuessUpdate);
                break;
            case "gameOver":
                playerState.gameResult = message.result;
                sceneManager.changeScene("Result");
                break;
            default:
                Debug.Log("No matching event found for scene...");
                break;
        }
    }

    public void send_message()
    {
        for(int i = 0; i < 4; i++)
        {
            next_message.verbs[i] = verb_Guess[i].captionText.text.ToString();
            next_message.nouns[i] = noun_Guess[i].captionText.text.ToString();
        }
        next_message.playerId = playerState.playerId;
        next_message.sessionId = playerState.sessionId;
        next_message.role = "Receiver";

        message_list.Add(next_message);
        
        try
        {
            NetworkMessaging.SendJsonViaPOST(next_message, "http://35.209.52.72:80/sendguessdemo");
        }
        catch (SystemException e)
        {
            Debug.Log(e.Message.ToString());
        }
    }
}
