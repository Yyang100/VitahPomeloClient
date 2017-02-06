using System;

namespace Pomelo.DotNetClient
{
    public enum ProtocolState
    {
        // Just open, need to send handshaking
        start = 1,

        // on handshaking process
        handshaking = 2,

        // can receive and send data
        working = 3,

        // on read body
        closed = 4,
    }
}