using SimpleJson;
using System.Collections;

public class BuildItemData
{
	public int BuildId { get; private set; }

	public int Lv { get; private set; }

	public int Type { get; private set; }

	public int UpEndTime { get; private set; }

	public void Init (JsonObject jsonObject)
	{
		this.BuildId = JsonUtil.GetInt32 (jsonObject, "build_id", this.BuildId);
		this.Lv = JsonUtil.GetInt32 (jsonObject, "lv", this.Lv);
		this.Type = JsonUtil.GetInt32 (jsonObject, "type", this.Type);
		this.UpEndTime = JsonUtil.GetInt32 (jsonObject, "up_end_time", this.UpEndTime);
	}
}
