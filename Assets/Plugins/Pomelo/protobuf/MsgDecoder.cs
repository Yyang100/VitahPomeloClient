using SimpleJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Protobuf
{
    public class MsgDecoder
    {
        // The message format(like .proto file)
        private JsonObject protos { set; get; }

        private int offset { set; get; }

        // The binary message from server.
        private byte[] buffer { set; get; }

        private Util util { set; get; }

        public MsgDecoder(JsonObject protos)
        {
            if (protos == null)
                protos = new JsonObject();

            this.protos = protos;
            this.util = new Util();
        }

        // Decode message from server.
        public JsonObject Decode(string route, byte[] buf)
        {
            this.buffer = buf;
            this.offset = 0;
            object proto = null;
            if (this.protos.TryGetValue(route, out proto))
            {
                JsonObject msg = new JsonObject();
                return this.DecodeMsg(msg, (JsonObject)proto, this.buffer.Length);
            }

            return null;
        }

        // Decode the message.
        private JsonObject DecodeMsg(JsonObject msg, JsonObject proto, int length)
        {
            while (this.offset < length)
            {
                Dictionary<string, int> head = this.GetHead();
                int tag;
                if (head.TryGetValue("tag", out tag) == false)
                {
                    continue;
                }

                object _tags = null;
                if (proto.TryGetValue("__tags", out _tags) == false)
                {
                    continue;
                }

                object name;
                if (((JsonObject)_tags).TryGetValue(tag.ToString(), out name) == false)
                {
                    continue;
                }

                object value;
                if (proto.TryGetValue(name.ToString(), out value) == false)
                {
                    continue;
                }

                object option;
                if (((JsonObject)(value)).TryGetValue("option", out option) == false)
                {
                    continue;
                }

                switch (option.ToString())
                {
                    case "optional":
                    case "required":
                        object type;
                        if (((JsonObject)(value)).TryGetValue("type", out type))
                        {
                            msg.Add(name.ToString(), this.DecodeProp(type.ToString(), proto));
                        }

                        break;
                    case "repeated":
                        object _name;
                        if (!msg.TryGetValue(name.ToString(), out _name))
                        {
                            msg.Add(name.ToString(), new JsonArray());
                        }

                        object value_type;
                        if (msg.TryGetValue(name.ToString(), out _name) && ((JsonObject)(value)).TryGetValue("type", out value_type))
                        {
                            this.DecodeArray((List<object>)_name, value_type.ToString(), proto);
                        }

                        break;
                }
            }
            return msg;
        }

        // Decode array in message.
        private void DecodeArray(List<object> list, string type, JsonObject proto)
        {
            if (this.util.IsSimpleType(type))
            {
                int length = (int)Decoder.DecodeUInt32(this.GetBytes());
                for (int i = 0; i < length; i++)
                {
                    list.Add(this.DecodeProp(type, null));
                }
            }
            else
            {
                list.Add(this.DecodeProp(type, proto));
            }
        }

        // Decode each simple type in message.
        private object DecodeProp(string type, JsonObject proto)
        {
            switch (type)
            {
                case "uInt32":
                    return Decoder.DecodeUInt64(this.GetBytes());
                case "int32":
                case "sInt32":
                    return Decoder.DecodeSInt32(this.GetBytes());
                case "float":
                    return this.DecodeFloat();
                case "double":
                    return this.DecodeDouble();
                case "string":
                    return this.DecodeString();
                default:
                    return this.decodeObject(type, proto);
            }
        }

        // Decode the user-defined object type in message.
        private JsonObject decodeObject(string type, JsonObject proto)
        {
            if (proto == null)
            {
                return new JsonObject();
            }

            object __messages;
            if (proto.TryGetValue("__messages", out __messages) == false)
            {
                return new JsonObject();
            }

            object _type;
            if (((JsonObject)__messages).TryGetValue(type, out _type) || protos.TryGetValue("message " + type, out _type))
            {
                int l = (int)Decoder.DecodeUInt32(this.GetBytes());
                JsonObject msg = new JsonObject();
                return this.DecodeMsg(msg, (JsonObject)_type, this.offset + l);
            }

            return new JsonObject();
        }

        // Decode string type.
        private string DecodeString()
        {
            int length = (int)Decoder.DecodeUInt32(this.GetBytes());
            string msg_string = Encoding.UTF8.GetString(this.buffer, this.offset, length);
            this.offset += length;
            return msg_string;
        }

        // Decode double type.
        private double DecodeDouble()
        {
            double msg_double = BitConverter.Int64BitsToDouble((long)this.ReadRawLittleEndian64());
            this.offset += 8;
            return msg_double;
        }

        // Decode float type
        private float DecodeFloat()
        {
            float msg_float = BitConverter.ToSingle(this.buffer, this.offset);
            this.offset += 4;
            return msg_float;
        }

        // Read long in littleEndian
        private ulong ReadRawLittleEndian64()
        {
            ulong b1 = this.buffer[this.offset];
            ulong b2 = this.buffer[this.offset + 1];
            ulong b3 = this.buffer[this.offset + 2];
            ulong b4 = this.buffer[this.offset + 3];
            ulong b5 = this.buffer[this.offset + 4];
            ulong b6 = this.buffer[this.offset + 5];
            ulong b7 = this.buffer[this.offset + 6];
            ulong b8 = this.buffer[this.offset + 7];
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
            | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }

        // Get the type and tag.
        private Dictionary<string, int> GetHead()
        {
            int tag = (int)Decoder.DecodeUInt32(this.GetBytes());
            Dictionary<string, int> head = new Dictionary<string, int>();
            head.Add("type", tag & 0x7);
            head.Add("tag", tag >> 3);
            return head;
        }

        // Get bytes.
        private byte[] GetBytes()
        {
            List<byte> arrayList = new List<byte>();
            int pos = this.offset;
            byte b;
            do
            {
                b = this.buffer[pos];
                arrayList.Add(b);
                pos++;
            } while (b >= 128);
            this.offset = pos;
            int length = arrayList.Count;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = arrayList[i];
            }
            return bytes;
        }
    }
}