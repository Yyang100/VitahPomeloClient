using SimpleJson;
using System.Collections;
using System.Collections.Generic;

public class DataPool : Singleton<DataPool>
{
	public RoleDataManager Role { get; private set; }

	public BuildDataManager Build { get; private set; }

	public void OnInit ()
	{
		this.Role = new RoleDataManager ();
		this.Build = new BuildDataManager ();
	}
}