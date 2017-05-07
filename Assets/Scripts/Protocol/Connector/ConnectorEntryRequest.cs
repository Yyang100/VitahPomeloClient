using SimpleJson;
using System;
using Pomelo.DotNetClient;
using UnityEngine;

// 连接协议
public class ConnectorEntryRequest : BaseRequest
{
	public void Entry ()
	{
		int my_uid = 10001;
		JsonObject msg = new JsonObject ();
		msg.Add ("uid", my_uid);
		this.Request (ProtocolRoute.CONNECTOR_ENTRY, msg);
	}
}