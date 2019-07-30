using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputManager : MonoBehaviour
{
    public string OutputType;
    private bool checkingForMessages = false;
    // Start is called before the first frame update

    private Transform [] verbOuts;
    private Transform [] nounOuts;

    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            //According to documentation, this will always start looking through children
            //before searching in other objects
            verbOuts[i] = this.gameObject.transform.Find("Verb (" + i + ")");
            nounOuts[i] = this.gameObject.transform.Find("Noun (" + i + ")");

            //Set these to false so they don't actually populate until the first message is received
            verbOuts[i].gameObject.SetActive(false);
            nounOuts[i].gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMessaging.socketOpen())
        {
            if (!checkingForMessages)
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
            resolveMessage((OutputMessage)newMessage);
        }

        checkingForMessages = false;
    }

    private void resolveMessage(OutputMessage message)
    {
        switch(message.msgEvent)
        {
            case "UpdateGuess":
                if(OutputType == "Guess")
                {
                    Debug.Log("Updating Guess");
                    updateGuessWindow(message);
                }
                break;
            case "SentMessage":
                if(OutputType == "Send")
                {
                    Debug.Log("Updating Send");
                    updateSendWindow(message);
                }
                break;
            default:
                Debug.Log("No matching event found for Output...");
                break;
        }
    }

    private void updateGuessWindow(OutputMessage message)
    {
        for(int i = 0; i < 4; i++)
        {
            verbOuts[i].gameObject.SetActive(true);
            verbOuts[i].gameObject.GetComponent<Text>().text = message.verbs[i];
            nounOuts[i].gameObject.SetActive(true);
            nounOuts[i].gameObject.GetComponent<Text>().text = message.nouns[i];
        }
    }

    private void updateSendWindow(OutputMessage message)
    {
        int displayStep = Int32.Parse(message.step);
        for(int i = 0; i < 4; i++)
        {
            if(i == displayStep)
            {
                verbOuts[i].gameObject.SetActive(true);
                verbOuts[i].gameObject.GetComponent<Text>().text = message.verb;
                nounOuts[i].gameObject.SetActive(true);
                nounOuts[i].gameObject.GetComponent<Text>().text = message.noun;
            }
            else
            {
                verbOuts[i].gameObject.SetActive(false);
                nounOuts[i].gameObject.SetActive(false);
            }

        }
    }

    public class OutputMessage
    {
        public string step;
        public string verb;
        public string noun;
        public string [] verbs;
        public string [] nouns;
        public string msgEvent {get; set; }
    }
}
