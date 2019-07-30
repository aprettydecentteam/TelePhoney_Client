using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverManager : MonoBehaviour
{
    List<GuessMessage> message_list = new List<GuessMessage>();
    GuessMessage next_message = new GuessMessage();

    Dropdown [] verb_Guess;
    Dropdown [] noun_Guess;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            verb_Guess[i] = GameObject.Find("VerbDropdownGuess " + "(" + i + ")").GetComponent<Dropdown>();
            noun_Guess[i] = GameObject.Find("NounDropdownGuess " + "(" + i + ")").GetComponent<Dropdown>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

        message_list.Add(next_message);
        
        try
        {
            NetworkMessaging.SendJsonViaPOST(next_message);
        }
        catch (SystemException e)
        {
            Debug.Log(e.Message.ToString());
        }
    }
}
