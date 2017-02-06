using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Pomelo.DotNetClient
{
    public class SocketCreator
    {
        private const int TimeoutMSec = 8000;

        private static bool isConnectionSuccessful = false;
        private static ManualResetEvent timeoutEvent = new ManualResetEvent(false);

        public static Socket Connect(string host, int port)
        {
            timeoutEvent.Reset();
            isConnectionSuccessful = false;

            UnityEngine.NetworkReachability state = UnityEngine.Application.internetReachability;
            if (state == UnityEngine.NetworkReachability.NotReachable)
            {
                return null;
            }

            IPAddress address;
            if (System.Net.IPAddress.TryParse(host, out address) == false)
            {
                return null;
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ie = new IPEndPoint(address, port);
            socket.BeginConnect(ie, new AsyncCallback(CallbackConnect), socket);

            if (timeoutEvent.WaitOne(TimeoutMSec, false))
            {
                if (isConnectionSuccessful)
                {
                    return socket;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                socket.Close();
                return null;
            }
        }

        private static void CallbackConnect(IAsyncResult result)
        {
            try
            {
                isConnectionSuccessful = false;
                Socket socket = result.AsyncState as Socket;

                if (socket != null)
                {
                    socket.EndConnect(result);
                    isConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                isConnectionSuccessful = false;
                Debug.LogException(ex);
            }
            finally
            {
                timeoutEvent.Set();
            }
        }
    }
}