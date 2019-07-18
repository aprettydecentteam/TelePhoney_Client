using Newtonsoft.Json;
using System.IO;
using System.Net;
using UnityEngine;

public class LocalServerNetwork : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void ServerTest()
    {
        JsonSerializer serializer = new JsonSerializer();
        GameEvent userAction = new GameEvent();
        userAction.setAction("TEST");

        string jsonString = JsonConvert.SerializeObject(userAction);

        /* could also use
              string json = new JavaScriptSerializer().Serialize(new
                {
                    user = "Foo",
                    password = "Baz"
                });
         */

        SendJSON(jsonString);
        
        return;
    }

    public void SendJSON( string jsonString)
    {
        HttpWebRequest jsonReq = (HttpWebRequest)WebRequest.Create("http://localhost:8095/");
        jsonReq.ContentType = "application/json";
        jsonReq.Method = "POST";

        using (StreamWriter streamWriter = new StreamWriter(jsonReq.GetRequestStream()))
        {
            streamWriter.Write(jsonString);
        }

        HttpWebResponse jsonResp = (HttpWebResponse)jsonReq.GetResponse();
        return;
    }
}

public class GameEvent
{
    public string action;

    public GameEvent()
        {
        }

    public void setAction(string action)
    {
        this.action = action;
    }

    public string getAction()
    {
        return this.action;
    }
}
