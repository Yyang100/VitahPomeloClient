
// 数据中心


using SimpleJson;
using System.Collections;
using System.Collections.Generic;

public class DataPool : Singleton<DataPool>
{
	public RoleDataManager Role { get; private set; }

	public void OnInit()
	{
		this.Role = new RoleDataManager();
	}
}