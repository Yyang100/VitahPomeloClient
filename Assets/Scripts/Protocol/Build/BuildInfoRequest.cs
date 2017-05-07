using UnityEngine;
using System.Collections;
using SimpleJson;

public class BuildInfoRequest : BaseRequest
{
	public void getInfo ()
	{
		this.Request (ProtocolRoute.BUILDING_GETINFO, new JsonObject ());
	}
}
