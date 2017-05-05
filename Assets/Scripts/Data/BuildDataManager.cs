using SimpleJson;
using System.Collections;
using System.Collections.Generic;

public class BuildDataManager  
{
	private List<BuildItemData> listBuildItem = new List<BuildItemData>();

	public List<BuildItemData> ListBuildItem
	{
		get { return this.listBuildItem; }
	}

	public BuildItemData GetBuildItemById(int buildId)
	{
		for (int i = 0; i < this.listBuildItem.Count; i++)
		{
			BuildItemData item = this.listBuildItem[i];
			if (item.BuildId == buildId)
			{
				return item;
			}
		}

		return null;
	}

	public void Init(JsonObject jsonObject)
	{
		JsonArray jsonArrayBuild = JsonUtil.GetJsonArray(jsonObject, "build_list");
		this.InitBuildList(jsonArrayBuild);
	}


	private void InitBuildList(JsonArray jsonArrayBuild)
	{
		if (jsonArrayBuild == null)
		{
			return;
		}

		IEnumerator enumerator = jsonArrayBuild.GetEnumerator();
		while (enumerator.MoveNext())
		{
			JsonObject jsonNode = (JsonObject)enumerator.Current;
			int buildId = JsonUtil.GetInt32(jsonNode, "build_id");
			var buildItem = this.GetBuildItemById(buildId);
			if (buildItem != null)
			{
				buildItem.Init(jsonNode);
			}
			else
			{
				buildItem = new BuildItemData();
				buildItem.Init(jsonNode);
				this.listBuildItem.Add(buildItem);
			}
		}
	}
}
