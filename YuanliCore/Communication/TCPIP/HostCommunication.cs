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
            tcpListener.Start();
            buffer = new byte[1024];

            Open();


        }

        public event Action<Exception> ReceiverException;


        public void Open()
        {
            receiverTask = Task.Run(Receiver);

        }

        public bool IsConnect { get; set; }


        private void Send(string message)
        {

            NetworkStream stream = tcpClient.GetStream();

            // 傳送指定的 Home 訊息給 B 機器        
            byte[] data = Encoding.ASCII.GetBytes(message + "123");
            stream.Write(data, 0, data.Length);

        }

        private async Task Receiver()
        {

            try
            {
                isReceiver = true;
                tcpClient = tcpListener.AcceptTcpClient();
                while (isReceiver)
                {



                    stream = tcpClient.GetStream();
                    Console.WriteLine("等待 A 機器傳送訊息...");

                    // 接收來自 A 機器的連線


                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // 將接收到的訊息轉換為字串
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("接收到訊息: " + message);

                    // 執行相應的動作，例如切換 Home 狀態
                   
                        // 執行 Home 相關的動作
                        Send(message);
                   


                    await Task.Delay(200);
                }
                // 關閉客戶端連線
                tcpClient.Close();
                tcpListener.Stop();
                /*
                                try
                                {
                                    byte[] Receive_data = new byte[256];
                                    clientSocket_Motion.Receive(Receive_data);
                                    string receive_data = "";
                                    for (int i = 0; i < 256; i++)
                                    {
                                        if (Receive_data[i] == 0)
                                            break;
                                        else
                                        {
                                            receive_data += Convert.ToString(Convert.ToChar(Receive_data[i]));
                                        }
                                    }
                                    //
                                    string[] re_data = Cal_Recive_Data(receive_data);
                                    logger.WriteLog("Receive : " + receive_data);
                                    if (re_data != null)
                                    {
                                        logger.WriteLog("Calculate OK : " + receive_data);
                                        if (re_data[1].IndexOf("YuanLi") >= 0)
                                            Receive_YuanLi();
                                        else if (re_data[1].IndexOf("Init") >= 0)
                                            Receive_Init();
                                        else if (re_data[1].IndexOf("SetRecipe") >= 0)
                                            Receive_SetRecipe(re_data[2]);
                                        else if (re_data[1].IndexOf("Mode") >= 0)
                                            Receive_Mode(re_data[2]);
                                        else if (re_data[1].IndexOf("Start") >= 0)
                                            Receive_Start(Convert.ToInt32(re_data[2]), Convert.ToString(re_data[3]), Convert.ToInt32(re_data[4]));
                                        else if (re_data[1].IndexOf("InPos") >= 0)
                                            Receive_InPos(Convert.ToInt32(re_data[2]));
                                        else if (re_data[1].IndexOf("Stop") >= 0)
                                            Receive_Stop(re_data[2], sender, e);
                                        else if (re_data[1].IndexOf("RFID") >= 0)
                                            Receive_RFID(re_data[2], re_data[3]);
                                        else
                                            logger.WriteErrorLog("No Match Data!");
                                    }
                                    else
                                        logger.WriteErrorLog("Motion Client Receive Error : " + receive_data);
                                }
                                catch (Exception error)
                                {
                                    //MessageBox.Show(error.ToString());
                                    int aaa = error.HResult;
                                    if (aaa == -2147467259)
                                    {
                                        logger.WriteErrorLog("Motion Client Disconnected!" + error.ToString());
                                        clientSocket_Motion = new Socket(SocketType.Stream, ProtocolType.Tcp);
                                        UpdatePicturebox(I_Red, pictureBox_Connect_Status);
                                        connect_Motion_client = false;
                                        timer_Server.Enabled = false;
                                    }
                                    else
                                    {
                                        logger.WriteErrorLog("Motion Client Error! " + error.ToString());
                                    }
                                }*/

            }
            catch (Exception ex)
            {
                ReceiverException?.Invoke(ex);
                //  logger.WriteErrorLog("Motion error" + error.ToString());
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
                if(!tcpClient.Connected)
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
