using SimpleJson;
using System;
using Pomelo.DotNetClient;
using UnityEngine;

public class BuildPush  {
	
	public static void OnBuild(JsonObject jsonObject)
	{
		Debug.Log("build push :" + jsonObject); // todo
		DataPool.Instance.Build.Init(jsonObject);
		PushEventNotifyCenter.Instance.FireChangeEvent(ProtocolFeature.OnBuild);		
	}

}
