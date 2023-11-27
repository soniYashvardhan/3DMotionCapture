using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class AnimationCode : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    private bool playing = true;
    private string[] points = {};

    public GameObject[] Body;

    // Start is called before the first frame update
    void Start()
    {
        ThreadStart ts = new ThreadStart(StartListener);
        mThread = new Thread(ts);
        mThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(points.Length > 1) { // greater than 1 because an extra comma at the end of the received string adds an extra element to the string array object
            Debug.Log(points.Length);
            for (int i =0; i<=32;i++)
            {
                float x = float.Parse(points[0 + (i * 3)]) / 100;
                float y = float.Parse(points[1 + (i * 3)]) / 100;
                float z = float.Parse(points[2 + (i * 3)]) / 300;
                Body[i].transform.localPosition = new Vector3(x, y, z);
            }
        }
    }

    private void StartListener()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();
        Debug.Log("listening");
        client = listener.AcceptTcpClient();
        playing = true;
        while (playing)
        {
            SendAndReceiveData();
        }
        listener.Stop();
    }

    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

        if (dataReceived != null)
        {
            points = dataReceived.Split(',');
            
            //---Sending Data to Host----
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this massage?"); //Converting string to byte data
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python
        }
    }
}