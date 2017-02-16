// 用户数据管理

using SimpleJson;
using System.Collections;
using System.Collections.Generic;

public class RoleDataManager
{
	public uint Uid { get; private set; }

	public uint Lv { get; private set; }

	public string Name { get; private set; }

	public ulong Gold { get; private set; }

	public ulong Diamond { get; private set; }

	public void Init(JsonObject jsonRole)
	{
		this.Uid = JsonUtil.GetUInt32(jsonRole, "uid", this.Uid);
		this.Lv = JsonUtil.GetUInt32(jsonRole, "lv", this.Lv);
		this.Name = JsonUtil.GetString(jsonRole, "name", this.Name);
		this.Gold = JsonUtil.GetUInt64(jsonRole, "gold", this.Gold);
		this.Diamond = JsonUtil.GetUInt64(jsonRole, "diamond", this.Diamond);
	}
}