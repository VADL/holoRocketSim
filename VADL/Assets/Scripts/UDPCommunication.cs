using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
//using HoloToolkit.Unity;
using System.Collections.Generic;
#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;

#endif
public class UDPCommunication : MonoBehaviour
{

    public RollFeedback rocket;

    public string port = "12345";
    public string externalIP = "10.1.10.200";
    public string externalPort = "54321";

    const string STOP_MESSAGE = "STOP";
    const string FORWARD_MESSAGE = "FORWARD";
    const string LEFT_MESSAGE = "LEFT";
    const string RIGHT_MESSAGE = "RIGHT";
    const string CLEAR_MESSAGE = "+++CLEAR LOGS";

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();


#if !UNITY_EDITOR
    DatagramSocket socket;
#endif
    // use this for initialization
#if !UNITY_EDITOR
    async void Start()
    {
        Debug.Log("Waiting for a connection...");

        socket = new DatagramSocket();
        socket.MessageReceived += Socket_MessageReceived;

        HostName IP = null;
        try
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            IP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
            .SingleOrDefault(
                hn =>
                    hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                    == icp.NetworkAdapter.NetworkAdapterId);

            await socket.BindEndpointAsync(IP, port);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            return;
        }
        
        Debug.Log("exit start");
    }

    private async System.Threading.Tasks.Task SendMessage(string message)
    {
        using (var stream = await socket.GetOutputStreamAsync(new Windows.Networking.HostName(externalIP), externalPort))
        {
            using (var writer = new Windows.Storage.Streams.DataWriter(stream))
            {
                var data = Encoding.UTF8.GetBytes(message);

                writer.WriteBytes(data);
                await writer.StoreAsync();
                Debug.Log("Sent: " + message);
            }
        }
    }

    public async void Forward()
    {
        await SendMessage(FORWARD_MESSAGE);
    }

    public async void Stop()
    {
        await SendMessage(STOP_MESSAGE); 
    }

    public async void Left()
    {
        await SendMessage(LEFT_MESSAGE);
    }

    public async void Right()
    {
        await SendMessage(RIGHT_MESSAGE);
    }

    public async void Clear()
    {
        await SendMessage(CLEAR_MESSAGE);
    }

#else
    void Start()
    {

    }
#endif

    // Update is called once per frame
    void Update()
    {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
    }

#if !UNITY_EDITOR

    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
        Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        Debug.Log("GOT MESSAGE: ");
        //Read the message that was received from the UDP echo client.
        Stream streamIn = args.GetDataStream().AsStreamForRead();
        StreamReader reader = new StreamReader(streamIn);
        string message = await reader.ReadLineAsync();

        Debug.Log("MESSAGE: " + message);
        //Debug.Log(message.Length + " " + message[message.Length - 1]);

        //Debug.Log(message[0].Equals('<'));
        //Debug.Log(message[message.Length - 1].Equals('>'));
        string[] values = message.Split(',');

        if (values.Length == 4)
        {
            rocket.SetOrientation(float.Parse(values[0]),float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (values.Length == 3)
        {
            rocket.SetOrientation(float.Parse(values[0]),float.Parse(values[1]), float.Parse(values[2]));
        }
       
    }
#endif
}