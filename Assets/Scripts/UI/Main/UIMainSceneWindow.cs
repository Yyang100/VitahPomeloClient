using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;

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

		this.addResBtn.onClick.AddListener (this.onAddResBtnClick);
	}

	// 显示玩家信息内容
	private void showRoleInfo(){
		this.roleInfoText.text = string.Empty;
		this.roleInfoText.text = this.getRoleInfoStr ();
	}

	// 生成玩家信息的字符串
	private string getRoleInfoStr(){
		return "ID:" + DataPool.Instance.Role.Uid.ToString () +
		"  名字:" + DataPool.Instance.Role.Name +
		"  等级:" + DataPool.Instance.Role.Lv.ToString () +
		"  金币:" + DataPool.Instance.Role.Gold.ToString () +
		"  钻石:" + DataPool.Instance.Role.Diamond.ToString ();
	}

	// 添加资源按钮点击响应
	private void onAddResBtnClick (){
		UserResRequest req = new UserResRequest ();
		req.OnSuccess = this.onAddResReqSuccess;
		int gold = 100;
		int diamond = 100;
		req.add (gold,diamond);
	}

	private void onBuildBtnClick(){
		UIManager.Instance.PushWindow(UIWindowDefine.BuildWindow, true);
	}

	// 添加资源协议成功回调
	private void onAddResReqSuccess(RequestData data){
		Debug.Log("资源增加协议成功");
	}

	// 进入时触发
	public override void OnEnter()
    {
        base.OnEnter();

		// 显示用户信息
		this.showRoleInfo ();

		// 添加监听
        PushEventNotifyCenter.Instance.AddNotification(ProtocolFeature.OnRole, this);
    }

	// 退出这个界面时调用
	public override void OnExit()
    {
        base.OnExit();
        PushEventNotifyCenter.Instance.RemoveObserver(ProtocolFeature.OnRole, this);
    }

	// 每次显示这个界面时调用
	public override void OnResume()
    {
        base.OnResume();
        this.OnRole();
    }

	private void OnRole()
    {
		this.showRoleInfo();
    }
}
