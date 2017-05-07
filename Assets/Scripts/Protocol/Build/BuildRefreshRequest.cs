using UnityEngine;
using System.Collections;
using SimpleJson;

public class BuildRefreshRequest : BaseRequest
{
	public void refresh (int build_id)
	{
		JsonObject msg = new JsonObject ();
		msg.Add ("build_id", build_id);
		this.Request (ProtocolRoute.BUILDING_REFRESH, msg);
	}
}
