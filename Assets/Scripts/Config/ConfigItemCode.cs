using System;
using SimpleJson;
using UnityEngine;
using System.Collections;

public class ConfigItemCode
{
	public int Id { get; private set; }

	public string Dscp { get; private set; }

	public int Alert { get; private set; }

	public void LoadFromJson (JsonObject jsonObject)
	{
		if (jsonObject == null) {
			Debug.Log ("json文件读取错误");
		} else {
			this.Id = JsonUtil.GetInt32 (jsonObject, "Id");
			this.Dscp = JsonUtil.GetString (jsonObject, "Dscp");
			this.Alert = JsonUtil.GetInt32 (jsonObject, "Alert");
		}
	}
}