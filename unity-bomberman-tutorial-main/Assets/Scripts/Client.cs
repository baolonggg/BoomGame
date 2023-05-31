using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient();
            receiveBuffer = new byte[dataBufferSize];

            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        public void ConnectCallBack(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            if (!socket.Connected)
                return;

            stream = socket.GetStream();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);

            // Gửi thông điệp đăng nhập tới server
            SendData("Login:" + instance.myId);
        }

        public void SendData(string _data)
        {
            byte[] _byteData = Encoding.ASCII.GetBytes(_data);
            stream.Write(_byteData, 0, _byteData.Length);
        }

        private void ReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result);
                if (byteLength <= 0)
                {
                    // Đã mất kết nối với server
                    // TODO: Xử lý đóng kết nối
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                string _message = Encoding.ASCII.GetString(data);

                // TODO: Xử lý thông điệp nhận được từ server
                Debug.Log("Received message from server: " + _message);

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
            }
            catch (Exception _ex)
            {
                Debug.Log("Error receiving TCP data: " + _ex.Message);
                // Đã xảy ra lỗi khi nhận dữ liệu
                // TODO: Xử lý đóng kết nối
            }
        }
    }
}
