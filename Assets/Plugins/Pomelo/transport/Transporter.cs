using System;
using System.Net.Sockets;
using UnityEngine;

namespace Pomelo.DotNetClient
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        internal byte[] buffer = new byte[BufferSize];
    }

    public class Transporter
    {
        public const int HeadLength = 4;

        private Socket socket;
        private Action<byte[]> messageProcesser;

        // Used for get message
        private StateObject stateObject = new StateObject();
        private TransportState transportState;
        private IAsyncResult asyncReceive;
        private IAsyncResult asyncSend;
        private bool onSending = false;
        private bool onReceiving = false;
        private byte[] headBuffer = new byte[4];
        private byte[] buffer;
        private int bufferOffset = 0;
        private int pkgLength = 0;
        internal Action onDisconnect = null;

        public Transporter(Socket socket, Action<byte[]> processer)
        {
            this.socket = socket;
            this.messageProcesser = processer;
            this.transportState = TransportState.readHead;
        }

        public void Start()
        {
            this.Receive();
        }

        public void Send(byte[] buffer)
        {
            if (this.transportState != TransportState.closed)
            {
                this.socket.BeginSend(
                    buffer,
                    0,
                    buffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(this.SendCallback),
                    this.socket);
            }
        }

        private void SendCallback(IAsyncResult asyncSend)
        {
            if (this.transportState == TransportState.closed)
            {
                return;
            }

            this.socket.EndSend(asyncSend);
        }

        public void Receive()
        {
            try
            {
                this.socket.BeginReceive(
                    this.stateObject.buffer,
                    0,
                    this.stateObject.buffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(this.EndReceive),
                    this.stateObject);
            }
            catch
            {
                try
                {
                    this.socket.BeginReceive(
                        this.stateObject.buffer,
                        0,
                        this.stateObject.buffer.Length,
                        SocketFlags.None,
                        new AsyncCallback(this.EndReceive),
                        this.stateObject);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception happend in begin accept : " + ex);
                }
            }

        }

        internal void Close()
        {
            this.transportState = TransportState.closed;
        }

        private void EndReceive(IAsyncResult asyncReceive)
        {
            if (this.transportState == TransportState.closed)
            {
                return;
            }

            StateObject state = (StateObject)asyncReceive.AsyncState;
            Socket socket = this.socket;

            try
            {
                int length = socket.EndReceive(asyncReceive);

                if (length > 0)
                {
                    this.ProcessBytes(state.buffer, 0, length);
                    if (this.transportState != TransportState.closed)
                    {
                        this.Receive();
                    }
                }
                else
                {
                    if (this.onDisconnect != null)
                    {
                        this.onDisconnect();
                    }
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                if (this.onDisconnect != null)
                {
                    this.onDisconnect();
                }
            }
        }

        internal void ProcessBytes(byte[] bytes, int offset, int limit)
        {
            if (this.transportState == TransportState.readHead)
            {
                ReadHead(bytes, offset, limit);
            }
            else if (this.transportState == TransportState.readBody)
            {
                ReadBody(bytes, offset, limit);
            }
        }

        private bool ReadHead(byte[] bytes, int offset, int limit)
        {
            int length = limit - offset;
            int headNum = HeadLength - bufferOffset;

            if (length >= headNum)
            {
                //Write head buffer
                WriteBytes(bytes, offset, headNum, bufferOffset, headBuffer);
                //Get package length
                pkgLength = (headBuffer[1] << 16) + (headBuffer[2] << 8) + headBuffer[3];

                if (pkgLength > 10 * 1024)
                {
                    Debug.Log("pkg too long:" + pkgLength.ToString());
                }

                //Init message buffer
                buffer = new byte[HeadLength + pkgLength];
                WriteBytes(headBuffer, 0, HeadLength, buffer);
                offset += headNum;
                bufferOffset = HeadLength;
                this.transportState = TransportState.readBody;

                if (offset <= limit) ProcessBytes(bytes, offset, limit);
                return true;
            }
            else
            {
                WriteBytes(bytes, offset, length, bufferOffset, headBuffer);
                bufferOffset += length;
                return false;
            }
        }

        private void ReadBody(byte[] bytes, int offset, int limit)
        {
            int length = pkgLength + HeadLength - bufferOffset;
            if ((offset + length) <= limit)
            {
                WriteBytes(bytes, offset, length, bufferOffset, buffer);
                offset += length;

                //Invoke the protocol api to handle the message
                this.messageProcesser.Invoke(buffer);
                this.bufferOffset = 0;
                this.pkgLength = 0;

                if (this.transportState != TransportState.closed)
                    this.transportState = TransportState.readHead;
                if (offset < limit)
                    ProcessBytes(bytes, offset, limit);
            }
            else
            {
                WriteBytes(bytes, offset, limit - offset, bufferOffset, buffer);
                bufferOffset += limit - offset;
                this.transportState = TransportState.readBody;
            }
        }

        private void WriteBytes(byte[] source, int start, int length, byte[] target)
        {
            this.WriteBytes(source, start, length, 0, target);
        }

        private void WriteBytes(byte[] source, int start, int length, int offset, byte[] target)
        {
            for (int i = 0; i < length; i++)
            {
                target[offset + i] = source[start + i];
            }
        }

        private void Print(byte[] bytes, int offset, int length)
        {
            for (int i = offset; i < length; i++)
            {
                Console.Write(Convert.ToString(bytes[i], 16) + " ");
            }

            Console.WriteLine();
        }
    }
}