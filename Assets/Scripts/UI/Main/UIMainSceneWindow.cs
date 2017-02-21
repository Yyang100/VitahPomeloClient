using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIMainSceneWindow : UIWindow {
	//  显示用户信息
	[SerializeField]
	private Text roleInfoText;

	// 添加资源按钮
	[SerializeField]
	private Button addResBtn;

	// Use this for initialization
	void Start () {
		Assert.IsNotNull (this.roleInfoText);
		Assert.IsNotNull (this.addResBtn);
		//this.showRoleInfo ();
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

	public override void OnEnter()
    {
        base.OnEnter();
        PushEventNotifyCenter.Instance.AddNotification(ProtocolFeature.OnRole, this);
    }

    public override void OnExit()
    {
        base.OnExit();
        PushEventNotifyCenter.Instance.RemoveObserver(ProtocolFeature.OnRole, this);
    }

    public override void OnResume()
    {
        base.OnResume();
        this.OnRole();
    }

	private void OnRole()
    {
    }
}
