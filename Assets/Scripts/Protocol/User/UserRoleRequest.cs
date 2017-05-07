using SimpleJson;
using System;
using Pomelo.DotNetClient;

// 获取用户信息协议
public class UserRoleRequest : BaseRequest
{
	public void getInfo ()
	{
		this.Request (ProtocolRoute.USER_ROLE_GETINFO, new JsonObject ());
	}
}