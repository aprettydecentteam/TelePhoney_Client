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

    // Start is called before the first frame update
    void Start()
    {
        step_dropdown = GameObject.Find("step_dropdown").GetComponent<Dropdown>();
        verb_dropdown = GameObject.Find("verb_dropdown").GetComponent<Dropdown>();
        noun_dropdown = GameObject.Find("noun_dropdown").GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void send_message()
    {
        next_message.setStep(step_dropdown.captionText.text.ToString());
        next_message.setNoun(noun_dropdown.captionText.text.ToString());
        next_message.setVerb(verb_dropdown.captionText.text.ToString());

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
