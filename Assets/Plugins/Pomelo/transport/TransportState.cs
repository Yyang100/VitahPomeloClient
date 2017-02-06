using System;

namespace Pomelo.DotNetClient
{
    public enum TransportState
    {
        // on read head
        readHead = 1,

        // on read body
        readBody = 2,

        // connection closed, will ignore all the message and wait for clean up
        closed = 3
    }
}