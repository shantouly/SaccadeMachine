using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.OpenXR.Input;
using Valve.VR;
using System.Text;
using UnityEngine.UI;
using System;

public class Connect : MonoBehaviour
{
    public SteamVR_Action_Boolean action = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    public SteamVR_Action_Boolean forward = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ForWard");
    public SteamVR_Action_Boolean backword = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BackWard");
    public SteamVR_Action_Boolean left = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Left");
    public SteamVR_Action_Boolean right = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Right");
    public SteamVR_Action_Vector2 remoter = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("remoter");
    public SteamVR_Behaviour_Pose pose;

    string serverIP = "192.168.8.6";
    int serverIPPort = 8877;
    TcpClient client;
    public NetworkStream stream;

    Texture2D texture;
    int width = 640;
    int height = 480;

    string messageRight = "4";
    string messageLeft = "3";
    string messageForWard = "1";
    string messageBackWard = "2";

    private void Start()
    {
        texture = new Texture2D(width, height);
        client = new TcpClient();
        client.Connect(serverIP, serverIPPort);
        stream = client.GetStream();
    }

    private void Update()
    {
        if (action.GetStateDown(pose.inputSource))
        {
            Debug.Log("11");
            Stop();
        }
        
        /*
        if (remoter.GetChanged(pose.inputSource))
        {
            if (remoter.GetAxis(pose.inputSource).x > 0 && remoter.GetAxis(pose.inputSource).y <= 0.4 && remoter.GetAxis(pose.inputSource).y >= -0.4)
            {
                //Debug.Log("1111");
                // 右转
                //Right();
                //ForWard();
            }
            if (remoter.GetAxis(pose.inputSource).x < 0 && remoter.GetAxis(pose.inputSource).y <= 0.4 && remoter.GetAxis(pose.inputSource).y >= -0.4)
            {
                // 左转
                //Left();
               // BackWard();
            }
            if (remoter.GetAxis(pose.inputSource).x >= -0.4 && remoter.GetAxis(pose.inputSource).x <= 0.4 && remoter.GetAxis(pose.inputSource).y > 0)
            {
                // 前进
                //ForWard();
                //Left();
            }
            if (remoter.GetAxis(pose.inputSource).x >= -0.4 && remoter.GetAxis(pose.inputSource).x <= 0.4 && remoter.GetAxis(pose.inputSource).y < 0)
            {
                // 向后
                //BackWard();
                //Right();
            }

            if (forward.GetStateDown(pose.inputSource))
            {
                Debug.Log(messageForWard);
            }
            if (backword.GetStateDown(pose.inputSource))
            {
                Debug.Log(messageBackWard);
            }
            if (left.GetStateDown(pose.inputSource))
            {
                Debug.Log(messageLeft);
            }
            if (right.GetStateDown(pose.inputSource))
            {
                Debug.Log(messageRight);
            }
        }*/

        
        if (forward.GetStateDown(pose.inputSource))
        {
            Debug.Log("向前");
            Left();
        }
        if (backword.GetStateDown(pose.inputSource))
        {
            Debug.Log("向后");
            Right();
        }
        if (left.GetStateDown(pose.inputSource))
        {
            Debug.Log("向左");
            BackWard();
        }
        if (right.GetStateDown(pose.inputSource))
        {
            Debug.Log("向右");
            ForWard();
        }

        
        if (Input.GetKeyDown(KeyCode.J))
        {
            Stop();
            ChangeMessage();
        } else if (Input.GetKeyDown(KeyCode.K))
        {
            Message();
        }
        
    }

    
    void Message()
    {
        messageRight = "4";
        messageLeft = "3";
        messageForWard = "1";
        messageBackWard = "2";
    }

    void ChangeMessage()
    {
        messageRight = "-1";
        messageLeft = "-1";
        messageForWard = "-1";
        messageBackWard = "-1";
    }
    

    void Right()
    {
        //string messageRight = "4";
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageRight);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    void Left()
    {
        //string messageLeft = "3";
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageLeft);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    void ForWard()
    {
        //string messageForWard = "1";
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageForWard);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    void BackWard()
    {
        //string messageBackWard = "2";
        byte[] messageBytes = Encoding.UTF8.GetBytes(messageBackWard);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    void Stop()
    {
        string message = "5";
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        stream.Write(messageBytes, 0, messageBytes.Length);
    }

    private void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}
