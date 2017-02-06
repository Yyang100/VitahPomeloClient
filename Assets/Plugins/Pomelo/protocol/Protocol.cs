using SimpleJson;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Pomelo.DotNetClient
{
    public class Protocol
    {
        private MessageProtocol messageProtocol;
        private ProtocolState state;
        private Transporter transporter;
        private HandShakeService handshake;
        private HeartBeatService heartBeatService = null;
        private PomeloClient pc;
        private RSACryptoServiceProvider provider = new RSACryptoServiceProvider(512);

        public Protocol(PomeloClient pc, System.Net.Sockets.Socket socket)
        {
            this.pc = pc;
            this.transporter = new Transporter(socket, this.ProcessMessage);
            this.transporter.onDisconnect = this.OnDisconnect;
            this.handshake = new HandShakeService(this);
            this.state = ProtocolState.start;
        }

        public PomeloClient GetPomeloClient()
        {
            return this.pc;
        }

        public void Start(JsonObject user, Action<JsonObject> callback)
        {
            this.transporter.Start();
            this.handshake.Request(user, callback);
            this.state = ProtocolState.handshaking;
        }

        // Send notify, do not need id
        public void Send(string route, JsonObject msg)
        {
            this.Send(route, 0, msg);
        }

        // Send request, user request id
        public void Send(string route, uint id, JsonObject msg)
        {
            if (this.state != ProtocolState.working)
            {
                return;
            }

            byte[] body = this.messageProtocol.Encode(route, id, msg);
            this.Send(PackageType.PKG_DATA, body);
        }

        public string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "  
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("x2"));
                }

                hexString = strB.ToString();
            }

            return hexString;
        }

        public void Send(PackageType type)
        {
            if (this.state == ProtocolState.closed)
            {
                return;
            }

            this.transporter.Send(PackageProtocol.Encode(type));
        }

        // Send system message, these message do not use messageProtocol
        public void Send(PackageType type, JsonObject msg)
        {
            // This method only used to send system package
            if (type == PackageType.PKG_DATA)
            {
                return;
            }

            byte[] body = Encoding.UTF8.GetBytes(msg.ToString());
            this.Send(type, body);
        }

        // Send message use the transporter
        public void Send(PackageType type, byte[] body)
        {
            if (this.state == ProtocolState.closed)
            {
                return;
            }

            byte[] pkg = PackageProtocol.Encode(type, body);
            this.transporter.Send(pkg);
        }

        public RSACryptoServiceProvider GetRsaProvide()
        {
            return this.provider;
        }

        // Invoke by Transporter, process the message
        public void ProcessMessage(byte[] bytes)
        {
            Package pkg = PackageProtocol.Decode(bytes);
            Loom.DispatchToMainThread(() =>
            {
                // Ignore all the message except handshading at handshake stage
                if (pkg.Type == PackageType.PKG_HANDSHAKE && this.state == ProtocolState.handshaking)
                {
                    // Ignore all the message except handshading
                    JsonObject data = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(Encoding.UTF8.GetString(pkg.Body));

                    this.ProcessHandshakeData(data);

                    this.state = ProtocolState.working;
                }
                else if (pkg.Type == PackageType.PKG_HEARTBEAT && this.state == ProtocolState.working)
                {
                    this.heartBeatService.ResetTimeout();
                }
                else if (pkg.Type == PackageType.PKG_DATA && this.state == ProtocolState.working)
                {
                    this.heartBeatService.ResetTimeout();
                    this.pc.ProcessMessage(this.messageProtocol.Decode(pkg.Body));
                }
                else if (pkg.Type == PackageType.PKG_KICK)
                {
                    this.GetPomeloClient().Kick();
                    this.Close();
                }
            });
        }

        public void Close()
        {
            this.transporter.Close();
            if (this.heartBeatService != null)
            {
                this.heartBeatService.Stop();
            }

            this.state = ProtocolState.closed;
        }

        private void ProcessHandshakeData(JsonObject msg)
        {
            // Handshake error
            if (!msg.ContainsKey("code") || !msg.ContainsKey("sys") || Convert.ToInt32(msg["code"]) != 200)
            {
                throw new Exception("Handshake error! Please check your handshake config.");
            }

            // Set compress data
            JsonObject sys = (JsonObject)msg["sys"];

            JsonObject dict = new JsonObject();
            if (sys.ContainsKey("useDict"))
            {
                if (sys.ContainsKey("dict"))
                {
                    dict = (JsonObject)sys["dict"];
                    PlayerPrefs.SetString("PomeloDict", dict.ToString());
                }
                else
                {
                    dict = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(PlayerPrefs.GetString("PomeloDict"));
                }
            }

            JsonObject protos = new JsonObject();
            JsonObject serverProtos = new JsonObject();
            JsonObject clientProtos = new JsonObject();

            if (sys.ContainsKey("useProto"))
            {
                if (sys.ContainsKey("protos"))
                {
                    protos = (JsonObject)sys["protos"];
                    serverProtos = (JsonObject)protos["server"];
                    clientProtos = (JsonObject)protos["client"];
                    PlayerPrefs.SetString("PomeloServerProtos", serverProtos.ToString());
                    PlayerPrefs.SetString("PomeloClientProtos", clientProtos.ToString());
                }
                else
                {
                    serverProtos = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(PlayerPrefs.GetString("PomeloServerProtos"));
                    clientProtos = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(PlayerPrefs.GetString("PomeloClientProtos"));
                }
            }

            this.messageProtocol = new MessageProtocol(dict, serverProtos, clientProtos);

            // Init heartbeat service
            int interval = 0;
            if (sys.ContainsKey("heartbeat"))
            {
                interval = Convert.ToInt32(sys["heartbeat"]);
            }

            this.heartBeatService = new HeartBeatService(interval, this);
            if (interval > 0)
            {
                this.heartBeatService.Start();
            }

            // send ack and change protocol state
            this.handshake.Ack();
            this.state = ProtocolState.working;

            // Invoke handshake callback
            JsonObject user = new JsonObject();
            if (msg.ContainsKey("user"))
            {
                user = (JsonObject)msg["user"];
            }

            this.handshake.InvokeCallback(user);
        }

        // The socket disconnect
        private void OnDisconnect()
        {
            this.pc.Disconnect();
        }
    }
}