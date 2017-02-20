using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIMainSceneWindow : UIWindow {
	[SerializeField]
	private Text roleInfoText;

	// Use this for initialization
	void Start () {
		Assert.IsNotNull (this.roleInfoText);
		this.showRoleInfo ();
	}
	
	private void showRoleInfo(){
		this.roleInfoText.text = string.Empty;
		this.roleInfoText.text = this.getRoleInfoStr ();
	}

	private string getRoleInfoStr(){
		return "Uid:" + DataPool.Instance.Role.Uid.ToString () +
		"  Name:" + DataPool.Instance.Role.Name +
		"  Lv:" + DataPool.Instance.Role.Lv.ToString () +
		"  Gold:" + DataPool.Instance.Role.Gold.ToString () +
		"  Diamond:" + DataPool.Instance.Role.Diamond.ToString ();
	}
}
