using SimpleJson;
using System;
using Pomelo.DotNetClient;
using UnityEngine;

// 连接协议
public class ConnectorEntryRequest : BaseRequest
{
    public void Entry()
    {
        JsonObject msg = new JsonObject();
        this.Request(ProtocolRoute.CONNECTOR_ENTRY, msg);
    }
}