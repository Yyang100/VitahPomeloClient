using UnityEngine;
using System.Collections;
using SimpleJson;

public class BuildUpgradeRequest : BaseRequest {
	
	public void upgrade(int build_id)
	{
		JsonObject msg = new JsonObject();
		msg.Add ("build_id",build_id);
		this.Request(ProtocolRoute.BUILDING_UPGRADE, msg);
	}
}
