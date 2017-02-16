// 用于登陆是获取信息


using SimpleJson;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class LoginData
{
	public class LoginRequestItem
	{
		public Type RequestClassType;
		public Action<JsonObject> DataFunc;
		public string JsonKeyName;

		public LoginRequestItem(Type requestClassType, Action<JsonObject> dataFunc, string jsonKeyName)
		{
			this.RequestClassType = requestClassType;
			this.DataFunc = dataFunc;
			this.JsonKeyName = jsonKeyName;
		}
	}

	public Action onSuccess;
	public Action<int> onProgress;
	public Action<RequestData> onError;

	private List<LoginRequestItem> loginRequestList = new List<LoginRequestItem>();

	private int currentRequestIndex;

	public LoginData()
	{
		this.currentRequestIndex = 0;
		this.AddRequestItem(typeof(UserRoleRequest), DataPool.Instance.Role.Init, "role_info");
	}

	public void Receive()
	{
		
		if (this.currentRequestIndex >= this.loginRequestList.Count)
		{
			this.onSuccess();
			return;
		}
			
		var requestItem = this.loginRequestList[this.currentRequestIndex];
		var request = (BaseRequest)Activator.CreateInstance(requestItem.RequestClassType);

		request.OnSuccess = this.OnInfo;
		request.OnError = this.onError;

		MethodInfo mi = requestItem.RequestClassType.GetMethod("getInfo");
		if (mi != null)
		{
			mi.Invoke(request, null);
		}
	}

	private void OnInfo(RequestData data)
	{
		var requestItem = this.loginRequestList[this.currentRequestIndex];
		requestItem.DataFunc(JsonUtil.GetJsonObject(data.JsonObject, requestItem.JsonKeyName));
		this.currentRequestIndex++;
		this.Receive();
	}

	private void OnError(RequestData data)
	{
		this.onError(data);
	}

	private void AddRequestItem(Type requestClassType, Action<JsonObject> dataFunc, string jsonKeyName)
	{
		this.loginRequestList.Add(new LoginRequestItem(requestClassType, dataFunc, jsonKeyName));
	}
}