using System.Net;
using System.IO;
using System.Threading;
using System;
using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

//Http for game events, websockets for connections

public class NetworkMessaging : MonoBehaviour
{
    private bool socketReady = false;
    private bool connected = false;

    ClientWebSocket web_socket = new ClientWebSocket();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (web_socket.State == WebSocketState.Open)
        {
            CheckSocketMessage();

            //if there's a message
            //SendSocketMessage(string);
        }
    }

    private async void ConnectWebSocketToServerAsync()
    {
        Uri server_uri = new Uri("ws://localhost:8095/test");
        if( web_socket.State != WebSocketState.Open )
            await web_socket.ConnectAsync( server_uri , CancellationToken.None );

        return;
    }

    private async void CheckSocketMessage()
    {
        //Receive from server
        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result = await web_socket.ReceiveAsync(bytesReceived, CancellationToken.None);
        Debug.Log(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));

        return;
    }

    private async void SendSocketMessage(string m)
    {
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(m));
        await web_socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

        return;
    }

    public void ServerTest()
    {
        JsonSerializer serializer = new JsonSerializer();
        ActionMessage action = new ActionMessage();
        action.setVerb("TEST");
        action.setNoun("CONNECTION");
        action.setStep("1");

        string jsonString = JsonConvert.SerializeObject(action);

        SendJSONString(jsonString);

        return;
    }

    public void SendPOSTRequest()
    {
        WebRequest req = WebRequest.Create("http://localhost:8095");
        req.Method = "POST";
        //req.ContentType = "application/json";

        WebResponse resp = req.GetResponse();
        Console.WriteLine(((HttpWebResponse)resp).StatusCode);

        return;
    }

    public void SendJSONString( string jsonString )
    {
        HttpWebRequest jsonReq = (HttpWebRequest)WebRequest.Create("http://localhost:8095/");
        jsonReq.ContentType = "application/json";
        jsonReq.Method = "POST";

        using (StreamWriter streamWriter = new StreamWriter(jsonReq.GetRequestStream()))
        {
            streamWriter.Write(jsonString);
        }

        HttpWebResponse jsonResp = (HttpWebResponse)jsonReq.GetResponse();
        var reader = new System.IO.StreamReader(jsonResp.GetResponseStream());
        //Debug.Log(reader.ReadToEnd());

        return;
    }

    private void CloseWebSocket()
    {
        web_socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        connected = false;

        return;
    }

    private void OnApplicationQuit()
    {
        CloseWebSocket();

        return;
    }

    private void OnDisable()
    {
        CloseWebSocket();

        return;
    }
}

public class ActionMessage
{
    public string step;
    public string noun;
    public string verb;

    public ActionMessage()
    {
        step = "";
        noun = "";
        verb = "";
    }

    public void setStep(string step)
    {
        this.step = step;
    }

    public string getAction()
    {
        return this.step;
    }

    public void setNoun(string noun)
    {
        this.noun = noun;
    }

    public string getNoun()
    {
        return this.noun;
    }

    public void setVerb(string verb)
    {
        this.verb = verb;
    }

    public string getVerb()
    {
        return this.verb;
    }
}