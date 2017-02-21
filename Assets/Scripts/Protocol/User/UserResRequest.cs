using UnityEngine;
using System.Collections;
using SimpleJson;

// 资源增加协议
public class UserResRequest : BaseRequest {

	public void add()
	{
		JsonObject msg = new JsonObject();
		msg.Add ("gold",100);
		msg.Add ("diamond",100);
		this.Request(ProtocolRoute.USER_RES_ADD, new JsonObject());
	}
}
