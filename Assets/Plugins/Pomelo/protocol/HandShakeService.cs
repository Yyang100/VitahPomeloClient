using SimpleJson;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Pomelo.DotNetClient
{
    public class HandShakeService
    {
        public const string Version = "0.3.0";
        public const string Type = "unity-socket";

        private Protocol protocol;
        private Action<JsonObject> callback;

        public HandShakeService(Protocol protocol)
        {
            this.protocol = protocol;
        }

        public void Request(JsonObject user, Action<JsonObject> callback)
        {
            byte[] body = Encoding.UTF8.GetBytes(this.BuildMsg(user).ToString());
            this.protocol.Send(PackageType.PKG_HANDSHAKE, body);
            this.callback = callback;
        }

        public void InvokeCallback(JsonObject data)
        {
            // Invoke the handshake callback
            if (this.callback != null)
            {
                this.callback.Invoke(data);
            }
        }

        public void Ack()
        {
            this.protocol.Send(PackageType.PKG_HANDSHAKE_ACK, new byte[0]);
        }

        private JsonObject BuildMsg(JsonObject user)
        {
            if (user == null)
            {
                user = new JsonObject();
            }

            // Build sys option
            JsonObject sys = new JsonObject();
            sys["version"] = Version;
            sys["type"] = Type;
            sys["protoVersion"] = this.getProtoVersion();
            sys["dictVersion"] = this.getDictVersion(); ;

            // Build handshake message
            JsonObject msg = new JsonObject();
            msg["sys"] = sys;
            msg["user"] = user;

            return msg;
        }

        private string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            string hexString = string.Empty;
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                strB.Append(bytes[i].ToString("x2"));
            }

            hexString = strB.ToString();
            return hexString;
        }

        private string ToString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            string hexString = string.Empty;
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                strB.Append(bytes[i].ToString());
            }

            hexString = strB.ToString();
            return hexString;
        }

        private string getDictVersion()
        {
            string dict = PlayerPrefs.GetString("PomeloDict");
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            return Convert.ToBase64String(provider.ComputeHash(System.Text.Encoding.Default.GetBytes(dict)));
        }

        private string getProtoVersion()
        {
            string serverProto = PlayerPrefs.GetString("PomeloServerProtos");
            string clientProto = PlayerPrefs.GetString("PomeloClientProtos");
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            return Convert.ToBase64String(provider.ComputeHash(System.Text.Encoding.Default.GetBytes(clientProto + serverProto)));
        }
    }
}