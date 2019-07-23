using UnityEngine;
using UnityEngine.Networking;

public class NewBehaviourScript : MonoBehaviour
{
    public bool atStartup = true;
    NetworkClient client;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(atStartup)
        {
            if (Input.GetKeyDown(KeyCode.C))
                SetupClient();
            if (Input.GetKeyDown(KeyCode.S))
                SetupServer();
        }
    }

    public void SetupServer()
    {
        NetworkServer.Listen(8095);
        atStartup = false;
    }

    public void SetupClient()
    {
        client = new NetworkClient();
        client.RegisterHandler(MsgType.Connect, OnConnected);
        client.Connect("", 8095);
        //atStartup = false;
    }

    public void OnConnected(NetworkMessage msg)
    {
        Debug.Log("Connected.");
    }
}
