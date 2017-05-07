// 定义协议

public static class ProtocolRoute
{
	#region connector

	public static string CONNECTOR_ENTRY = "connector.entryHandler.entry";

	#endregion

	#region user

	public static string USER_ROLE_GETINFO = "user.roleHandler.getInfo";
	public static string USER_RES_ADD = "user.resHandler.add";

	#endregion

	#region build

	public static string BUILDING_GETINFO = "building.buildHandler.getInfo";
	public static string BUILDING_BUILD = "building.buildHandler.build";
	public static string BUILDING_UPGRADE = "building.buildHandler.upgrade";
	public static string BUILDING_REFRESH = "building.buildHandler.refresh";

	#endregion
}