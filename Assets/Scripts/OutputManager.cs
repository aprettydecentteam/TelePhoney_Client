﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputManager : MonoBehaviour
{
    public string OutputType;
    private bool checkingForMessages = false;
    // Start is called before the first frame update

    public Transform [] verbOuts;
    public Transform [] nounOuts;
    public Transform matchesOut;

    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            //According to documentation, this will always start looking through children
            //before searching in other objects
            verbOuts[i] = this.gameObject.transform.Find("Verb" + i);
            nounOuts[i] = this.gameObject.transform.Find("Noun" + i);

            //Set these to false so they don't actually populate until the first message is received
            verbOuts[i].gameObject.SetActive(false);
            nounOuts[i].gameObject.SetActive(false);
        }
        matchesOut = GameObject.Find("MatchTracker").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateGuessWindow(OutputMessage message)
    {
        if(OutputType == "Guess")
        {
            for(int i = 0; i < 4; i++)
            {
                verbOuts[i].gameObject.SetActive(true);
                verbOuts[i].gameObject.GetComponent<Text>().text = message.verbs[i];
                nounOuts[i].gameObject.SetActive(true);
                nounOuts[i].gameObject.GetComponent<Text>().text = message.nouns[i];
            }
        }
    }

    public void updateSendWindow(OutputMessage message)
    {
        if(OutputType == "Send")
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
    }

    public void updateCorrectGuessWindow(OutputMessage message)
    {
        if(matchesOut != null)
        {
            matchesOut.Find("PlayerRoleText").gameObject.GetComponent<Text>().text = message.role;
            matchesOut.Find("MatchesText").gameObject.GetComponent<Text>().text = message.correctGuesses;
        }
    }

    public void updateCipherWindow(OutputMessage message)
    {
        if(OutputType == "Cipher")
        {
            for(int i = 0; i < 4; i++)
            {
                if(message.verbs[i] != "")
                {
                    verbOuts[i].gameObject.SetActive(true);
                    verbOuts[i].gameObject.GetComponent<Text>().text = message.verbs[i];
                }
                else
                {
                    verbOuts[i].gameObject.SetActive(false);                    
                }
                if(message.nouns[i] != "")
                {
                    nounOuts[i].gameObject.SetActive(true);
                    nounOuts[i].gameObject.GetComponent<Text>().text = message.nouns[i];
                }
                else
                {
                    nounOuts[i].gameObject.SetActive(false);                    
                }
            }
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
    public string role { get; set; }
    public string correctGuesses { get; set; }
}