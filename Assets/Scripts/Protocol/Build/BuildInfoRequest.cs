using UnityEngine;
using System.Collections;
using SimpleJson;

public class BuildInfoRequest : BaseRequest {
	public void getInfo()
	{
		this.Request(ProtocolRoute.BUILD_INFO, new JsonObject());
	}
}
