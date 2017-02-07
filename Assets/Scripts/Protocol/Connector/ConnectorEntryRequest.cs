using SimpleJson;
using System;
using Pomelo.DotNetClient;

// 连接协议
public class ConnectorEntryRequest : BaseRequest
{
    public void Entry(string signedKey)
    {
        JsonObject msg = new JsonObject();
        this.Request(ProtocolRoute.CONNECTOR_ENTRY, msg);
    }
}