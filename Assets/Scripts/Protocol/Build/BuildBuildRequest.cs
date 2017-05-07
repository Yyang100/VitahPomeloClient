using UnityEngine;
using System.Collections;
using SimpleJson;

public class BuildBuildRequest : BaseRequest
{
	public void build (int build_type)
	{
		JsonObject msg = new JsonObject ();
		msg.Add ("build_type", build_type);
		this.Request (ProtocolRoute.BUILDING_BUILD, msg);
	}
}
