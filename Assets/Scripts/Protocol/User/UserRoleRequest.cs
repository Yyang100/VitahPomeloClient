using SimpleJson;
using System;
using Pomelo.DotNetClient;

// 获取用户信息协议
public class UserRoleRequest : BaseRequest
{
	public void getInfo()
	{
		// 定义我的Uid
		int my_uid = 10000;
		JsonObject msg = new JsonObject();
		msg.Add("uid", my_uid);
		this.Request(ProtocolRoute.USER_ROLE_GETINFO , msg);
	}
}