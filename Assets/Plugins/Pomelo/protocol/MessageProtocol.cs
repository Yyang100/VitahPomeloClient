using Pomelo.Protobuf;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.DotNetClient
{
    public class MessageProtocol
    {
        private Dictionary<string, ushort> dict = new Dictionary<string, ushort>();
        private Dictionary<ushort, string> abbrs = new Dictionary<ushort, string>();
        private JsonObject encodeProtos = new JsonObject();
        private JsonObject decodeProtos = new JsonObject();
        private Dictionary<uint, string> reqMap;
        private Protobuf.Protobuf protobuf;
        public const int MSG_Route_Limit = 255;
        public const int MSG_Route_Mask = 0x01;
        public const int MSG_Type_Mask = 0x07;

        public MessageProtocol(JsonObject dict, JsonObject serverProtos, JsonObject clientProtos)
        {
            ICollection<string> keys = dict.Keys;

            foreach (string key in keys)
            {
                ushort value = Convert.ToUInt16(dict[key]);
                this.dict[key] = value;
                this.abbrs[value] = key;
            }

            this.protobuf = new Protobuf.Protobuf(clientProtos, serverProtos);
            this.encodeProtos = clientProtos;
            this.decodeProtos = serverProtos;

            this.reqMap = new Dictionary<uint, string>();
        }

        public byte[] Encode(string route, JsonObject msg)
        {
            return this.Encode(route, 0, msg);
        }

        public byte[] Encode(string route, uint id, JsonObject msg)
        {
            int routeLength = ByteUtil.StringByteLength(route);
            if (routeLength > MSG_Route_Limit)
            {
                throw new Exception("Route is too long!");
            }

            // Encode head
            // The maximus length of head is 1 byte flag + 4 bytes message id + route string length + 1byte
            byte[] head = new byte[routeLength + 6];
            int offset = 1;
            byte flag = 0;

            if (id > 0)
            {
                byte[] bytes = Protobuf.Encoder.EncodeUInt32(id);

                ByteUtil.WriteBytes(bytes, offset, head);
                flag |= ((byte)MessageType.MSG_REQUEST) << 1;
                offset += bytes.Length;
            }
            else
            {
                flag |= ((byte)MessageType.MSG_NOTIFY) << 1;
            }

            // Compress head
            if (this.dict.ContainsKey(route))
            {
                ushort cmpRoute = this.dict[route];
                ByteUtil.WriteShort(offset, cmpRoute, head);
                flag |= MSG_Route_Mask;
                offset += 2;
            }
            else
            {
                // Write route length
                head[offset++] = (byte)routeLength;

                // Write route
                ByteUtil.WriteBytes(Encoding.UTF8.GetBytes(route), offset, head);
                offset += routeLength;
            }

            head[0] = flag;

            // Encode body
            byte[] body;
            if (this.encodeProtos.ContainsKey(route))
            {
                body = this.protobuf.Encode(route, msg);
            }
            else
            {
                body = Encoding.UTF8.GetBytes(msg.ToString());
            }

            // Construct the result
            byte[] result = new byte[offset + body.Length];
            for (int i = 0; i < offset; i++)
            {
                result[i] = head[i];
            }

            for (int i = 0; i < body.Length; i++)
            {
                result[offset + i] = body[i];
            }

            // Add id to route map
            if (id > 0)
                this.reqMap.Add(id, route);

            return result;
        }

        public Message Decode(byte[] buffer)
        {
            // Decode head
            // Get flag
            byte flag = buffer[0];

            // Set offset to 1, for the 1st byte will always be the flag
            int offset = 1;

            // Get type from flag;
            MessageType type = (MessageType)((flag >> 1) & MSG_Type_Mask);
            uint id = 0;
            string route;

            if (type == MessageType.MSG_RESPONSE)
            {
                int length;
                id = (uint)Protobuf.Decoder.DecodeUInt32(offset, buffer, out length);
                if (id <= 0 || !this.reqMap.ContainsKey(id))
                {
                    return null;
                }
                else
                {
                    route = this.reqMap[id];
                    this.reqMap.Remove(id);
                }

                offset += length;
            }
            else if (type == MessageType.MSG_PUSH)
            {
                // Get route
                if ((flag & 0x01) == 1)
                {
                    ushort routeId = ByteUtil.ReadShort(offset, buffer);
                    route = this.abbrs[routeId];

                    offset += 2;
                }
                else
                {
                    byte length = buffer[offset];
                    offset += 1;

                    route = Encoding.UTF8.GetString(buffer, offset, length);
                    offset += length;
                }
            }
            else
            {
                return null;
            }

            // Decode body
            byte[] body = new byte[buffer.Length - offset];
            for (int i = 0; i < body.Length; i++)
            {
                body[i] = buffer[i + offset];
            }

            JsonObject msg;
            if (this.decodeProtos.ContainsKey(route))
            {
                msg = this.protobuf.Decode(route, body);
            }
            else
            {
                msg = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(Encoding.UTF8.GetString(body));
            }

            // Construct the message
            return new Message(type, id, route, msg);
        }
    }
}
