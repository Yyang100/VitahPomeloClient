using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SimpleJson;
using UnityEngine.Assertions;

namespace Pomelo.DotNetClient
{
    // network state enum
    public enum NetWorkState
    {
        CLOSED,

        CONNECTING,

        CONNECTED,

        KICK,

        DISCONNECTED,

        TIMEOUT,

        ERROR
    }

    public class PomeloClient : IDisposable
    {
        private EventManager eventManager;
        private Socket socket;
        private Protocol protocol;
        private uint reqId = 1;
        private bool disposed = false;

        public PomeloClient()
        {
        }

        // netwrok changed event
        public event Action<NetWorkState> NetWorkStateChangedEvent;

        public bool InitClient(string host, int port)
        {
            this.socket = SocketCreator.Connect(host, port);
            if (this.socket == null)
            {
                this.NetWorkChanged(NetWorkState.ERROR);
                return false;
            }

            this.eventManager = new EventManager();
            this.protocol = new Protocol(this, this.socket);
            this.NetWorkChanged(NetWorkState.CONNECTED);
            return true;
        }

        public void Connect()
        {
            this.Connect(null, null);
        }

        public void Connect(JsonObject user)
        {
            this.Connect(user, null);
        }

        public void Connect(Action<JsonObject> handshakeCallback)
        {
            this.Connect(null, handshakeCallback);
        }

        public bool Connect(JsonObject user, Action<JsonObject> handshakeCallback)
        {
            try
            {
                this.protocol.Start(user, handshakeCallback);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void Request(string route, Action<StatusCode, JsonObject> action)
        {
            this.Request(route, new JsonObject(), action);
        }

        public void Request(string route, JsonObject msg, Action<StatusCode, JsonObject> action)
        {
            Assert.IsNotNull(msg, "request msg is null.");
            this.eventManager.AddCallBack(this.reqId, action);
            this.protocol.Send(route, this.reqId, msg);
            this.reqId++;
        }

        public void Notify(string route, JsonObject msg)
        {
            Assert.IsNotNull(msg, "notify msg is null.");
            this.protocol.Send(route, msg);
        }

        public void On(string eventName, Action<JsonObject> action)
        {
            this.eventManager.AddOnEvent(eventName, action);
        }

        public void ProcessMessage(Message msg)
        {
            if (msg.type == MessageType.MSG_RESPONSE)
            {
                this.eventManager.InvokeCallBack(msg.id, StatusCode.SUCCESS, msg.data);
            }
            else if (msg.type == MessageType.MSG_PUSH)
            {
                this.eventManager.InvokeOnEvent(msg.route, msg.data);
            }
        }

        public void Kick()
        {
            this.Dispose();
            this.NetWorkChanged(NetWorkState.KICK);
        }

        public void Disconnect()
        {
            this.Dispose();
            this.NetWorkChanged(NetWorkState.DISCONNECTED);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.protocol.Close();
            this.eventManager.Dispose();
            this.disposed = true;

            try
            {
                lock (this)
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                    this.socket = null;
                }
            }
            catch (Exception)
            {
                this.socket = null;
            }
        }

        // 网络状态变化
        private void NetWorkChanged(NetWorkState state)
        {
            if (this.NetWorkStateChangedEvent != null)
            {
                this.NetWorkStateChangedEvent(state);
            }
        }
    }
}