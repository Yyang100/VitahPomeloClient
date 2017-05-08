using UnityEngine;
using SimpleJson;

public class ConfigManager
{

	private static ConfigManager instance = null;

	private ConfigCode configCode = new ConfigCode ();

	private ConfigManager ()
	{
	}

	public static ConfigManager Instance ()
	{
		if (instance == null) {
			instance = new ConfigManager ();
			instance.Load ();
		}

		return instance;
	}

	public ConfigCode Code ()
	{
		return this.configCode;
	}

	public void Load ()
	{
		this.configCode.LoadFromJson (this.JsonFromFile ("code"));
	}

	public JsonObject JsonFromFile (string filepath)
	{
		JsonObject jsonObject = null;
		TextAsset fileName = UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/Config/" + filepath + ".json", typeof(TextAsset)) as TextAsset;

		if (fileName == null) {
			Debug.Log ("find json file faild!");
		} else {
			jsonObject = SimpleJson.SimpleJson.DeserializeObject<JsonObject> (fileName.text);
		}

		return jsonObject;
	}
}
