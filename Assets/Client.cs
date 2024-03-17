using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using TMPro;

public class Client : MonoBehaviour
{
    private static Client instance;
    public static Client Instace
    {
        get{return instance;}
    }

    private void Awake() {
        if(Client.Instace != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    TcpClient tc;
    NetworkStream stream;
    StreamReader sr;
    StreamWriter sw;
    private Thread m_ThrdClientReceive;
    public bool login = false;

    
    [SerializeField]
    TextMeshProUGUI textMeshProUGUI;
    
    private void Start() {
        try
        {
            tc = new TcpClient("127.0.0.1", 7000);
            stream = tc.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream); 
            m_ThrdClientReceive = new Thread(new ThreadStart(ListenForData));
            m_ThrdClientReceive.IsBackground = true;
            m_ThrdClientReceive.Start();
        }
        catch
        {
            Debug.Log("Connect Error!");
        }
    }

    private void ListenForData()
    {
        int length;
        Byte[] bytes = new Byte[1024];
        while (true)
        {
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incommingData = new byte[length];
                Array.Copy(bytes, 0, incommingData, 0, length);

                string serverMessage = Encoding.Default.GetString(incommingData);
                Debug.Log(serverMessage); // 받은 값
                textMeshProUGUI.text = serverMessage;
            }
        }
    }


    public void Communication(string msg)
    {
        Debug.Log(msg);
        byte[] buff = Encoding.UTF8.GetBytes(msg);
        stream.Write(buff, 0, buff.Length);
    }

    private void OnApplicationQuit()
    {
        tc.Close();
        stream.Close();
    }
}