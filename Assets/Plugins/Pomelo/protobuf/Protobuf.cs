using System;
using SimpleJson;

namespace Pomelo.Protobuf
{
    public class Protobuf
    {
        private MsgDecoder decoder;
        private MsgEncoder encoder;

        public Protobuf(JsonObject encodeProtos, JsonObject decodeProtos)
        {
            this.encoder = new MsgEncoder(encodeProtos);
            this.decoder = new MsgDecoder(decodeProtos);
        }

        public byte[] Encode(string route, JsonObject msg)
        {
            return this.encoder.Encode(route, msg);
        }

        public JsonObject Decode(string route, byte[] buffer)
        {
            return this.decoder.Decode(route, buffer);
        }
    }
}