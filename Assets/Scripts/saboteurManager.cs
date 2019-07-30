using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class saboteurManager : MonoBehaviour
{
    List<ActionMessage> message_list = new List<ActionMessage>();
    ActionMessage next_message = new ActionMessage();
    ActionMessage persistMessage = new ActionMessage();
    List<GuessMessage> guess_list = new List<GuessMessage>();
    GuessMessage guess_message = new GuessMessage();

    public Dropdown noun_dropdown;
    public Dropdown verb_dropdown;
    public Dropdown [] verb_Guess;
    public Dropdown [] noun_Guess;

    private bool checkingForMessages = false;


    // Start is called before the first frame update
    void Start()
    {
        playerState.playerRole = "Saboteur";


        if (!NetworkMessaging.socketOpen())
        {
            Debug.Log("Connecting to web socket");
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
                roleSend.role = "Saboteur";
                roleSend.id = message.id;
                NetworkMessaging.SendJsonViaPOST(roleSend, "http://localhost:8095/roledemo");
                break;
            case "sentMessage":
                Component[] messageComponents;
                OutputMessage clueUpdate = new OutputMessage();
                clueUpdate.noun = message.noun;
                persistMessage.noun = clueUpdate.noun;
                clueUpdate.verb = message.verb;
                persistMessage.verb = clueUpdate.verb;
                clueUpdate.step = message.step;
                persistMessage.step = clueUpdate.step;
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

    public void send_guess()
    {
        for(int i = 0; i < 4; i++)
        {
            guess_message.verbs[i] = verb_Guess[i].captionText.text.ToString();
            guess_message.nouns[i] = noun_Guess[i].captionText.text.ToString();
        }
        guess_message.playerId = playerState.playerId;
        guess_message.sessionId = playerState.sessionId;

        guess_list.Add(guess_message);
        
        try
        {
            NetworkMessaging.SendJsonViaPOST(guess_message, "http://localhost:8095/sendguessdemo");
        }
        catch (SystemException e)
        {
            Debug.Log(e.Message.ToString());
        }
    }

    public void send_saboteur_message_noun()
    {
        next_message.step = persistMessage.step;
        next_message.noun = (noun_dropdown.captionText.text.ToString());
        next_message.verb = persistMessage.verb;
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

    public void send_saboteur_message_verb()
    {
        next_message.step = persistMessage.step;
        next_message.noun = persistMessage.noun;
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