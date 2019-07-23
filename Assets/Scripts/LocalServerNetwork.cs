using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class LocalServerNetwork : MonoBehaviour
{
    TcpClient tcpClient = new TcpClient();
    NetworkStream serverStream = default(NetworkStream);
    string readData = string.Empty;
    string msg = "Connected";

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
        var reader = new System.IO.StreamReader(jsonResp.GetResponseStream());
        Debug.Log(reader.ReadToEnd());
        return;
    }

    public void StartClient()
    {
        if(!tcpClient.Connected)
            tcpClient.Connect("127.0.0.1", 8095);

        serverStream = tcpClient.GetStream();

        if (tcpClient.Connected)
        {
            Debug.Log(msg);

            byte[] response = new byte[4096];
            int bytes = 0;

            try
            {
                //bytes = serverStream.Read(response, 0, 4096);
                byte[] outByte = Encoding.ASCII.GetBytes("TEST LADS");
                serverStream.Write(outByte, 0, outByte.Length);
                serverStream.Flush();
            }
            catch
            {
                Debug.Log("OH NO");
            }

            ASCIIEncoding encoding = new ASCIIEncoding();

            Debug.Log(encoding.GetString(response, 0, bytes));
        }
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
