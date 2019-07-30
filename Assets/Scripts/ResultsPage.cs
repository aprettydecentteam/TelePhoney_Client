using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPage : MonoBehaviour
{    
    Transform ResultBanner;
    bool resultDisplayed = false;
    // Start is called before the first frame update
    void Start()
    {
        ResultBanner = this.gameObject.transform.Find("ResultBanner");
    }

    // Update is called once per frame
    void Update()
    {
       if(resultDisplayed == false)
       {
           ResultBanner.gameObject.GetComponent<Text>().text = playerState.gameResult;
           resultDisplayed = true;
       } 
    }

    public void onClickRestart()
    {
        sceneManager.changeScene(playerState.playerRole);
    }

    public void onClickURL()
    {
        Application.OpenURL("https://careers.walmart.com/");
    }
}
