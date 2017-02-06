using SimpleJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Protobuf
{
    public class MsgEncoder
    {
        // The message format(like .proto file)
        private JsonObject protos { set; get; }

        private Encoder encoder { set; get; }

        private Util util { set; get; }

        public MsgEncoder(JsonObject protos)
        {
            if (protos == null)
                protos = new JsonObject();

            this.protos = protos;
            this.util = new Util();
        }

        // Encode the message from server.
        public byte[] Encode(string route, JsonObject msg)
        {
            byte[] returnByte = null;
            object proto;
            if (this.protos.TryGetValue(route, out proto) == false)
            {
                return null;
            }

            if (this.CheckMsg(msg, (JsonObject)proto) == false)
            {
                return null;
            }

            int length = ByteUtil.StringByteLength(msg.ToString()) * 2;
            int offset = 0;
            byte[] buff = new byte[length];
            offset = this.EncodeMsg(buff, offset, (JsonObject)proto, msg);
            returnByte = new byte[offset];
            for (int i = 0; i < offset; i++)
            {
                returnByte[i] = buff[i];
            }

            return returnByte;
        }

        // Check the message.
        private bool CheckMsg(JsonObject msg, JsonObject proto)
        {
            ICollection<string> protoKeys = proto.Keys;
            foreach (string key in protoKeys)
            {
                JsonObject value = (JsonObject)proto[key];
                object proto_option;
                if (value.TryGetValue("option", out proto_option) == false)
                {
                    continue;
                }

                switch (proto_option.ToString())
                {
                    case "required":
                        if (!msg.ContainsKey(key))
                        {
                            return false;
                        }

                        break;
                    case "optional":
                        object value_type;

                        JsonObject messages = (JsonObject)proto["__messages"];

                        value_type = value["type"];

                        if (msg.ContainsKey(key))
                        {
                            Object value_proto;

                            if (messages.TryGetValue(value_type.ToString(), out value_proto) || protos.TryGetValue("message " + value_type.ToString(), out value_proto))
                            {
                                this.CheckMsg((JsonObject)msg[key], (JsonObject)value_proto);
                            }
                        }

                        break;
                    case "repeated":
                        object msg_name;
                        object msg_type;
                        if (value.TryGetValue("type", out value_type) && msg.TryGetValue(key, out msg_name))
                        {
                            if (((JsonObject)proto["__messages"]).TryGetValue(value_type.ToString(), out msg_type) || protos.TryGetValue("message " + value_type.ToString(), out msg_type))
                            {
                                List<object> o = (List<object>)msg_name;
                                foreach (object item in o)
                                {
                                    if (!this.CheckMsg((JsonObject)item, (JsonObject)msg_type))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }

                        break;
                }
            }

            return true;
        }

        // Encode the message.
        private int EncodeMsg(byte[] buffer, int offset, JsonObject proto, JsonObject msg)
        {
            ICollection<string> msgKeys = msg.Keys;
            foreach (string key in msgKeys)
            {
                object value;
                if (proto.TryGetValue(key, out value) == false)
                {
                    continue;
                }

                object value_option;
                if (((JsonObject)value).TryGetValue("option", out value_option) == false)
                {
                    continue;
                }

                switch (value_option.ToString())
                {
                    case "required":
                    case "optional":
                        {
                            object value_type, value_tag;
                            if (((JsonObject)value).TryGetValue("type", out value_type) && ((JsonObject)value).TryGetValue("tag", out value_tag))
                            {
                                offset = ByteUtil.WriteBytes(this.EncodeTag(value_type.ToString(), Convert.ToInt32(value_tag)), offset, buffer);
                                offset = this.EncodeProp(msg[key], value_type.ToString(), offset, buffer, proto);
                            }

                            break;
                        }

                    case "repeated":
                        {
                            object msg_key;
                            if (msg.TryGetValue(key, out msg_key))
                            {
                                List<int> msg_int_list = msg_key as List<int>;
                                if (msg_int_list != null && msg_int_list.Count > 0)
                                {
                                    offset = this.EncodeArray((List<int>)msg_key, (JsonObject)value, offset, buffer, proto);
                                }
                                else
                                {
                                    List<object> msg_object_list = msg_key as List<object>;
                                    if (msg_object_list != null && msg_object_list.Count > 0)
                                    {
                                        offset = this.EncodeArray((List<object>)msg_key, (JsonObject)value, offset, buffer, proto);
                                    }
                                }
                            }

                            break;
                        }
                }
            }

            return offset;
        }

        // Encode the array type.
        private int EncodeArray(List<object> msg, JsonObject value, int offset, byte[] buffer, JsonObject proto)
        {
            object value_type, value_tag;
            if (value.TryGetValue("type", out value_type) && value.TryGetValue("tag", out value_tag))
            {
                if (this.util.IsSimpleType(value_type.ToString()))
                {
                    offset = ByteUtil.WriteBytes(this.EncodeTag(value_type.ToString(), Convert.ToInt32(value_tag)), offset, buffer);
                    offset = ByteUtil.WriteBytes(Encoder.EncodeUInt32((uint)msg.Count), offset, buffer);
                    foreach (object item in msg)
                    {
                        offset = this.EncodeProp(item, value_type.ToString(), offset, buffer, null);
                    }
                }
                else
                {
                    foreach (object item in msg)
                    {
                        offset = ByteUtil.WriteBytes(this.EncodeTag(value_type.ToString(), Convert.ToInt32(value_tag)), offset, buffer);
                        offset = this.EncodeProp(item, value_type.ToString(), offset, buffer, proto);
                    }
                }
            }

            return offset;
        }

        // Encode the array type.
        private int EncodeArray(List<int> msg, JsonObject value, int offset, byte[] buffer, JsonObject proto)
        {
            object value_type, value_tag;
            if (value.TryGetValue("type", out value_type) && value.TryGetValue("tag", out value_tag))
            {
                if (this.util.IsSimpleType(value_type.ToString()))
                {
                    offset = ByteUtil.WriteBytes(this.EncodeTag(value_type.ToString(), Convert.ToInt32(value_tag)), offset, buffer);
                    offset = ByteUtil.WriteBytes(Encoder.EncodeUInt32((uint)msg.Count), offset, buffer);
                    foreach (object item in msg)
                    {
                        offset = this.EncodeProp(item, value_type.ToString(), offset, buffer, null);
                    }
                }
                else
                {
                    foreach (object item in msg)
                    {
                        offset = ByteUtil.WriteBytes(this.EncodeTag(value_type.ToString(), Convert.ToInt32(value_tag)), offset, buffer);
                        offset = this.EncodeProp(item, value_type.ToString(), offset, buffer, proto);
                    }
                }
            }

            return offset;
        }

        // Encode each item in message.
        private int EncodeProp(object value, string type, int offset, byte[] buffer, JsonObject proto)
        {
            switch (type)
            {
                case "uInt32":
                    ByteUtil.WriteUInt32(buffer, ref offset, value);
                    break;
                case "int32":
                case "sInt32":
                    ByteUtil.WriteInt32(buffer, ref offset, value);
                    break;
                case "float":
                    ByteUtil.WriteFloat(buffer, ref offset, value);
                    break;
                case "double":
                    ByteUtil.WriteDouble(buffer, ref offset, value);
                    break;
                case "string":
                    ByteUtil.WriteString(buffer, ref offset, value);
                    break;
                default:
                    object __messages;
                    object __message_type;

                    if (proto.TryGetValue("__messages", out __messages))
                    {
                        if (((JsonObject)__messages).TryGetValue(type, out __message_type) || protos.TryGetValue("message " + type, out __message_type))
                        {
                            byte[] tembuff = new byte[ByteUtil.StringByteLength(value.ToString()) * 3];
                            int length = 0;
                            length = this.EncodeMsg(tembuff, length, (JsonObject)__message_type, (JsonObject)value);
                            offset = ByteUtil.WriteBytes(Encoder.EncodeUInt32((uint)length), offset, buffer);
                            for (int i = 0; i < length; i++)
                            {
                                buffer[offset] = tembuff[i];
                                offset++;
                            }
                        }
                    }

                    break;
            }

            return offset;
        }

        // Encode tag.
        private byte[] EncodeTag(string type, int tag)
        {
            int flag = this.util.ContainType(type);
            return Encoder.EncodeUInt32((uint)(tag << 3 | flag));
        }
    }
}