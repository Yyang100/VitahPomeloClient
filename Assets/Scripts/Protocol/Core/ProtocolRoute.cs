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
	public static string BUILD_BUILD = "build.buildHandler.build";
	public static string BUILD_UPGRADE = "build.upgradeHandler.upgrade";
	public static string BUILD_REFRESH = "build.refreshHandler.refresh";
	#endregion
}