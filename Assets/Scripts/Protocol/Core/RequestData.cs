// 协议返回数据

using SimpleJson;

public class RequestData
{
	private int code;
	private JsonObject jsonObject;

	public RequestData (int code, string msg, JsonObject jsonObject)
	{
		this.code = code;
		this.jsonObject = jsonObject;
	}

	public int Code {
		get {
			return this.code;
		}
	}

	public JsonObject JsonObject {
		get {
			return this.jsonObject;
		}
	}
}
