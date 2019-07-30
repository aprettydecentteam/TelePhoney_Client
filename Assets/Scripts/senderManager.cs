using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class senderManager : MonoBehaviour
{
    List<ActionMessage> message_list = new List<ActionMessage>();
    ActionMessage next_message = new ActionMessage();

    GuessMessage answer = new GuessMessage();
    GuessMessage cypher = new GuessMessage();
    Dropdown step_dropdown;
    Dropdown noun_dropdown;
    Dropdown verb_dropdown;

    private bool checkingForMessages = false;
    private bool receiverInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        playerState.playerRole = "Sender";
        step_dropdown = GameObject.Find("step_dropdown").GetComponent<Dropdown>();
        verb_dropdown = GameObject.Find("verb_dropdown").GetComponent<Dropdown>();
        noun_dropdown = GameObject.Find("noun_dropdown").GetComponent<Dropdown>();

        answer.verbs = new string[] {"Push", "Turn", "Turn", "Cut"};
        answer.nouns = new string[] {"Wire", "Dial", "Switch", "Switch"};

        cypher.verbs = new string[] {"Push", "", "Turn", ""};
        cypher.nouns = new string[] {"", "Dial", "", "Switch"};

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
                receiverInitialized = true;
                NetworkMessaging.SendJsonViaPOST(cypher, "http://35.209.52.72:80/initreceiverdemo");
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

    private void evaluateGuess (clientEvent message)
    {
        int correctGuesses = 0;

        foreach (var index in new int[] {0, 1, 2, 3})
        {
            if (message.nouns[index] == answer.nouns[index] &&
                message.verbs[index] == answer.verbs[index])
            {
                correctGuesses++;
            }

            if (correctGuesses == 4)
            {
                clientEvent winner = new clientEvent();
                winner.role = message.role;
                NetworkMessaging.SendJsonViaPOST(winner, "http://35.209.52.72:80/winnerdemo");
            }

            else
            {
                clientEvent numCorrect = new clientEvent();
                numCorrect.correctGuesses = correctGuesses.ToString();
                NetworkMessaging.SendJsonViaPOST(numCorrect, "http://35.209.52.72:80/sendcorrectguessesdemo");
            }
        }
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
                NetworkMessaging.SendJsonViaPOST(roleSend, "http://35.209.52.72:80/roledemo");
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

                evaluateGuess(message);
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
        next_message.step = (step_dropdown.captionText.text.ToString());
        next_message.noun = (noun_dropdown.captionText.text.ToString());
        next_message.verb = (verb_dropdown.captionText.text.ToString());
        next_message.playerId = playerState.playerId;
        next_message.sessionId = playerState.sessionId;

        message_list.Add(next_message);

        try
        {
            NetworkMessaging.SendJsonViaPOST(next_message, "http://35.209.52.72:80/sendmessagedemo");
        }
        catch (SystemException e)
        {
            Debug.Log(e.Message.ToString());
        }
    }
}
