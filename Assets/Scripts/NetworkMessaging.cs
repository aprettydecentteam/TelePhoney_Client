using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

    private static ClientWebSocket web_socket = new ClientWebSocket();

    public static async void ConnectWebSocketToServerAsync( string uri = "ws://localhost:8095/test" )
    {
        //Replace this with hosted server endpoint
        Uri server_uri = new Uri(uri);
        if( !socketOpen() )
            await web_socket.ConnectAsync( server_uri , CancellationToken.None );

        return;
    }

    public static async Task<string> CheckSocketMessage()
    {
        //Receive from server
        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result = await web_socket.ReceiveAsync(bytesReceived, CancellationToken.None);
        string message = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

        if (message == null)
        {
            message = "";
        }
        else
        {
            
        }

        return message;
    }

    private static void ProcessMessage(string message)
    {
        //Interpret server messages
    }

    public static async void SendSocketMessage( System.Object data)
    {
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
        await web_socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public void ServerTest()
    {
        JsonSerializer serializer = new JsonSerializer();
        ActionMessage action = new ActionMessage();
        action.setVerb("TEST");
        action.setNoun("CONNECTION");
        action.setStep("1");

        SendJsonViaPOST(action);

        return;
    }

    public void SendPOSTRequest( string url = "http://localhost:8095" )
    {
        WebRequest req = WebRequest.Create(url);
        req.Method = "POST";
        //req.ContentType = "application/json";

        WebResponse resp = req.GetResponse();
        Console.WriteLine(((HttpWebResponse)resp).StatusCode);

        return;
    }

    public static System.Object SendJsonViaPOST( System.Object data, string url = "http://localhost:8095/" )
    {
        //Replace with hosted server IP
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        String return_resp = "";
        req.ContentType = "application/json";
        req.Method = "POST";

        try
        {
            string data_string = JsonConvert.SerializeObject(data);

            using (StreamWriter streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(data_string);
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            var reader = new System.IO.StreamReader(resp.GetResponseStream());
            return_resp = reader.ReadToEnd();
        }
        catch (SystemException e)
        {
            return e;
        }

        System.Object temp = JsonConvert.DeserializeObject(return_resp);

        return temp;
    }

    public static bool socketOpen() {
        return (NetworkMessaging.web_socket.State == WebSocketState.Open);
    }

    private void CloseWebSocket()
    {
        web_socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        connected = false;

        return;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application quit");
        CloseWebSocket();

        return;
    }

    private void OnDisable()
    {
        Debug.Log("Application Disable");
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