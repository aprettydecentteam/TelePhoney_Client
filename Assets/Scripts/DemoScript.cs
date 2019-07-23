using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net.WebSockets;
using System.Text;

public class DemoScript : MonoBehaviour
{
    private bool socketReady = false;
    private bool connected = false;
    private TcpClient socket;
    ClientWebSocket websock = new ClientWebSocket();
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    // Start is called before the first frame update
    void Start()
    {
        ConnectWebSockToServerAsync();
    }

    // Update is called once per frame
    void Update()
    {
        //ConnectTcpToServer();

        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    private async void ConnectWebSockToServerAsync()
    {
        Uri servUri = new Uri("ws://localhost:8095/test");
        if( websock.State != WebSocketState.Open )
            await websock.ConnectAsync( servUri , CancellationToken.None );

        while(websock.State == WebSocketState.Open)
        {
            //string m = "test";
            //ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(m));
            //await websock.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await websock.ReceiveAsync(bytesReceived, CancellationToken.None);
            Debug.Log(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
        }
    }

    private void OnIncomingData(string data)
    {
        if ( data.Length > 0 )
        {
            
        }
        else
        {
            
        }
    }

    public void ConnectTcpToServer()
    {
        // Only connect if not yet connected
        if (!socketReady)
        {
            string host = "127.0.0.1";
            int port = 80;

            // create socket
            try
            {
                socket = new TcpClient(host, port);
                stream = socket.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
                socketReady = true;

                writer.WriteLine("Test!");
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Socket error: " + e.Message);
            }
        }

    }

    private void Send(string data)
    {
        if (socketReady)
        {
            writer.WriteLine(data);
            writer.Flush();
        }
    }

    public void OnSendButton()
    {
        //string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        //Send(message);
    }

    public void OnNameButton()
    {
        //clientName = GameObject.Find("NameInput").GetComponent<InputField>().text;
        if (socketReady)
        {
            //Send("&NAME|" + clientName);
        }
    }

    private void CloseSocket()
    {
        if (socketReady)
        {
            writer.Close();
            reader.Close();
            socket.Close();
            socketReady = false;
        }
    }

    private void CloseWebSock()
    {
        websock.CloseAsync( WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None );
        connected = false;
    }

    private void OnApplicationQuit()
    {
        //CloseSocket();
        CloseWebSock();
    }

    private void OnDisable()
    {
        //CloseSocket();
        CloseWebSock();
    }
}

