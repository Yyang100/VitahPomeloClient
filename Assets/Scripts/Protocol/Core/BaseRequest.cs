using Pomelo.DotNetClient;
using SimpleJson;
using System;

public class BaseRequest
{
	public Action<RequestData> OnSuccess;
	public Action<RequestData> OnError;
	public Action OnFail;

	public bool IsShowTip = true;

	protected void Request (string route, JsonObject msg)
	{
		PomeloManager.Instance.Request (
			route,
			msg,
			(data) => {
				if (data.Code == 200) {
					if (this.OnSuccess != null) {
						this.OnSuccess (data);
					}
				} else {
					if (this.OnError != null) {
						this.OnError (data);
					}

					// 当协议返回错误码时，弹出错误码对应内容
					if (this.IsShowTip == true) {
						var codeConf = ConfigManager.Instance ().Code ().GetItem (data.Code);
						if (codeConf != null) {
							UITipUtil.Show (codeConf.Dscp);
						} else {
							UITipUtil.Show ("错误码:" + data.Code);
						}
					}
				}
			},
			() => {
				if (this.OnFail != null) {
					this.OnFail ();
				}
			});
	}

	protected void Notify (string route, JsonObject msg)
	{
		PomeloManager.Instance.Notify (route, msg);
	}
}
