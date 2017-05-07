using UnityEngine;
using System.Collections;
using SimpleJson;

// 资源增加协议
public class UserResRequest : BaseRequest
{
	public void add (int gold, int diamond)
	{
		JsonObject msg = new JsonObject ();
		msg.Add ("gold", gold);
		msg.Add ("diamond", diamond);
		this.Request (ProtocolRoute.USER_RES_ADD, msg);
	}
}
