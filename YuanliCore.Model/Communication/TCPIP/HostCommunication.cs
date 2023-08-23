using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YuanliCore.Communication
{

    public class HostCommunication
    {

        private TcpClient tcpClient;

        private TcpListener tcpListener;
        private Task receiverTask = Task.CompletedTask;
        private bool isReceiver;

        private NetworkStream stream;
        private byte[] buffer;

        public HostCommunication(string ipAddress, int port)
        {

            IPAddress ip = IPAddress.Parse(ipAddress);


            tcpListener = new TcpListener(ip, port);

            buffer = new byte[1024];

            Open();


        }
        /// <summary>
        /// 接收到訊息
        /// </summary>
        public event Action<string> ReceiverMessage;
       /// <summary>
       /// 確認連線成功
       /// </summary>
        public event Action<bool> ReceiverIsConnect;
        public event Action<Exception> ReceiverException;


        public void Open()
        {
            tcpListener.Start();

            receiverTask = Task.Run(Receiver);

        }
        public void Close()
        {
            isReceiver = false;
            // 關閉客戶端連線
            tcpClient.Close();
            tcpListener.Stop();

        }
        public bool IsConnect { get; set; }


        public void Send(string message)
        {

            NetworkStream stream = tcpClient.GetStream();

            // 傳送指定的 Home 訊息給 B 機器        
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }

        private async Task Receiver()
        {

            try
            {
                isReceiver = true;
                if (tcpClient == null || !tcpClient.Connected)
                    tcpClient = tcpListener.AcceptTcpClient();
                int notMessageCount = 0;
                ReceiverIsConnect?.Invoke(true);
                while (isReceiver)
                {


                    int id = Thread.CurrentThread.ManagedThreadId;
                    stream = tcpClient.GetStream();
                    Console.WriteLine("等待 A 機器傳送訊息...");

                    // 接收來自 A 機器的連線


                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // 將接收到的訊息轉換為字串
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("接收到訊息: " + message);
                    if (message == "")
                    {
                        notMessageCount++;
                        if (notMessageCount > 10)
                        {
                            tcpClient.Close();
                            throw new Exception(" Is Disconnect");
                        }
                    }
                    // 執行相應的動作，例如切換 Home 狀態
                    ReceiverMessage?.Invoke(message);
                    // 執行 Home 相關的動作
                    //            Send(message);



                    await Task.Delay(200);
                }

            }
            catch (Exception ex)
            {
                ReceiverException?.Invoke(ex);
                ReceiverIsConnect?.Invoke(false);
                receiverTask = Task.Run(Receiver);
                //  Close();

            }
        }

    }


    public class ClientCommunication
    {
        private TcpClient tcpClient;
        private bool isReceiver;
        private string message;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private Task receiverTask = Task.CompletedTask;
        private string ipAddress;
        private int port;



        public ClientCommunication(string ipAddress, int port)
        {

            tcpClient = new TcpClient();
            this.ipAddress = ipAddress;
            this.port = port;

            tcpClient.Connect(ipAddress, port);
            isReceiver = true;

            receiverTask = Task.Run(Receiver);

        }
        public bool IsReceiver { get => isReceiver; }
        public event Action<string> ReceiverMessage;
        public event Action<Exception> ReceiverException;

        public void Open()
        {
            try
            {
                if (!tcpClient.Connected)
                    tcpClient.Connect(ipAddress, port);
                isReceiver = true;

                receiverTask = Task.Run(Receiver);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /*
        public async Task Close()
        {
            try
            {
                isReceiver = false;
                stream.Close();
                await receiverTask;

                tcpClient.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }*/
        public void Dispose()
        {
            isReceiver = false;
            stream.Close();
            receiverTask.Wait();

            tcpClient.Close();
            tcpClient.Dispose();


        }


        public void Send(string message)
        {

            NetworkStream stream = tcpClient.GetStream();

            // 傳送指定的 Home 訊息給 B 機器        
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }

        private async Task Receiver()
        {

            try
            {
                isReceiver = true;

                while (isReceiver)
                {
                    stream = tcpClient.GetStream();

                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // 將接收到的訊息轉換為字串
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("接收到訊息: " + message);

                    if (message == "errTest123") throw new Exception($"Error Test . ThreadID :{Thread.CurrentThread.ManagedThreadId }");
                    // 執行相應的動作，例如切換 Home 狀態
                    ReceiverMessage?.Invoke(message);


                    await Task.Delay(200);
                }
                // 關閉客戶端連線
                tcpClient.Close();



            }
            catch (Exception error)
            {
                isReceiver = false;
                ReceiverException?.Invoke(error);

            }
        }

    }
}
