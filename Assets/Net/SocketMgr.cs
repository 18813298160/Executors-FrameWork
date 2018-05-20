using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class SocketMgr    
{
    private Socket clientSocket;
    const string ip = "127.0.0.1";
    private int port = 1234;
    private const int bufferSize = 1024;
    private byte[] buffer = new byte[bufferSize];
    private List<byte> recBuffer = new List<byte>();

    private static SocketMgr _instance;

	public static SocketMgr instance
    {
        get
        {
            if (_instance == null)
                _instance = new SocketMgr();
            return _instance;
        }
    }


    public void OnReqConnect()
    {
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, 
                                      SocketType.Stream, 
                                      ProtocolType.Tcp);
            IPAddress ipAdress = IPAddress.Parse(ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAdress, port);
            IAsyncResult result = clientSocket.BeginConnect(ipEndPoint, OnConnectSuccess, clientSocket);
            bool success = result.AsyncWaitHandle.WaitOne(500, true);
            if(!success)
            {
                OnConnectTimeOut();
            }
        }

        catch(Exception ex)
        {
            OnConnectError(ex);
        }

    }
   
    private void OnConnectSuccess(IAsyncResult ar)
    {
        Debug.Log("socket connected!");
		//clientSocket.EndSend(ar); 是一个必须的操作，结束后台线程的挂起
		clientSocket.EndSend(ar);
        StartRecieve();
    }

    private void OnConnectTimeOut()
    {
        Debug.LogError("time out!");
    }

    private void OnConnectError(Exception ex)
    {
        Debug.LogError(ex.Message);
    }

    private void StartRecieve()
    {
        clientSocket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, OnRecieve, null);
    }

    private void OnRecieve(IAsyncResult ar)
    {
        int read = clientSocket.EndReceive(ar);
		//read : 真实读取的数据字节数
		if(read > 0)
        {
            byte[] data = new byte[read];
            Buffer.BlockCopy(buffer, 0, data, 0, read);
            recBuffer.AddRange(data);
        }

		//判断前4个字节，这里有网络数据大端转c#小端的过程，一般是网络数据长度。
        //如果拿不到长度，说明数据没有发完，继续接收，如果拿到了完整的数据流。
        //那么我们就把整个数据流方式receiveBuffer里面，这样receiveBuffer
        //就有了整个数据缓冲字节，可以交给逻辑代码处理。
		if(recBuffer.Count > 4)
        {
            byte[] lenBytes = recBuffer.GetRange(0, 4).ToArray();
            int len = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(lenBytes, 0));
			// one protocol data received  
			if (recBuffer.Count - 4 >= len)
			{
				byte[] dataBytes = recBuffer.GetRange(4, len).ToArray();
			}
			else
			{
				// protocol data not complete   
			}
		}

        //递归接收数据，不断监听网络数据
        StartRecieve();

    }

	public void SocketSend(Action<NetStream> OnSetStream, Action OnComplete)
	{
        Debug.Assert(OnSetStream != null, "Please set send OnSetStream");
		Debug.Assert(OnComplete != null, "Please set send OnComplete");

		NetStream stream = new NetStream();

		stream.WriteInt32(0); // temp length  

		OnSetStream(stream);

		stream.Seek(0);
		// real length  
		stream.WriteInt32(stream.GetLength() - 4);

		byte[] buffer = stream.GetBuffer();

        clientSocket.BeginSend
		(
			buffer,
			0,
			buffer.Length,
			SocketFlags.None,
			(IAsyncResult ar) =>
			{
				clientSocket.EndSend(ar);
				stream.Close();
				OnComplete();
			},
			null // no need  
		);
	}
}
