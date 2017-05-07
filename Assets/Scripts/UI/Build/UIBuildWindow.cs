using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;

public class UIBuildWindow : UIWindow
{

	[SerializeField]
	private Text buildInfoText;

	[SerializeField]
	private Button buildBtn;

	[SerializeField]
	private Button upgradeBtn;

	[SerializeField]
	private Button refreshBtn;

	[SerializeField]
	private InputField buildInput;

	[SerializeField]
	private InputField upgradeInput;

	[SerializeField]
	private InputField refreshInput;

	// Use this for initialization
	void Start ()
	{
		Assert.IsNotNull (this.buildInfoText);
		Assert.IsNotNull (this.buildBtn);
		Assert.IsNotNull (this.upgradeBtn);
		Assert.IsNotNull (this.refreshBtn);
		Assert.IsNotNull (this.buildInput);
		Assert.IsNotNull (this.upgradeInput);
		Assert.IsNotNull (this.refreshInput);

		this.buildBtn.onClick.AddListener (this.onBuildBtnClick);
		this.upgradeBtn.onClick.AddListener (this.onUpgradeBtnClick);
		this.refreshBtn.onClick.AddListener (this.onRefreshBtnClick);
	}

	private void showBuildInfo ()
	{
		this.buildInfoText.text = this.getBuildInfoStr ();
		this.emptyInput ();
	}

	private string getBuildInfoStr ()
	{
		if (DataPool.Instance.Build.ListBuildItem.Count == 0) {
			return "建筑列表为空";
		}

		string info_str = string.Empty;
		DateTime temp_dt;
		System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime (new System.DateTime (1970, 1, 1)); // 当地时区
			
		for (int i = 0; i < DataPool.Instance.Build.ListBuildItem.Count; i++) {
			BuildItemData build_item = DataPool.Instance.Build.ListBuildItem [i];
			var upEndTimeStr = string.Empty;
			if (build_item.UpEndTime == 0) {
				upEndTimeStr = "   建筑不在升级中";
			} else {
				temp_dt = startTime.AddSeconds (build_item.UpEndTime);
				upEndTimeStr = "   升级结束时间：" + temp_dt.ToString ("yyyy/MM/dd HH:mm:ss");
			}

			string build_item_str = " ID:" + build_item.BuildId +
			                        "   类型：" + build_item.Type +
			                        "   等级：" + build_item.Lv + upEndTimeStr;
			info_str += build_item_str + "\n";
		}
		
		return info_str;
	}

	// 建造按钮点击响应
	private void onBuildBtnClick ()
	{
		BuildBuildRequest req = new BuildBuildRequest ();
		req.OnSuccess = this.onBuildReqSuccess;
		int build_type = int.Parse (this.buildInput.text);
		req.build (build_type);
	}

	// 升级按钮点击响应
	private void onUpgradeBtnClick ()
	{
		BuildUpgradeRequest req = new BuildUpgradeRequest ();
		req.OnSuccess = this.onUpgradeReqSuccess;
		int build_id = int.Parse (this.upgradeInput.text);
		req.upgrade (build_id);
	}

	private void emptyInput ()
	{
		this.buildInput.text = string.Empty;
		this.upgradeInput.text = string.Empty;
		this.refreshInput.text = string.Empty;
	}

	// 刷新按钮点击响应
	private void onRefreshBtnClick ()
	{
		BuildRefreshRequest req = new BuildRefreshRequest ();
		req.OnSuccess = this.onRefreshReqSuccess;
		int build_id = int.Parse (this.refreshInput.text);
		req.refresh (build_id);
	}

	private void onBuildReqSuccess (RequestData data)
	{
		Debug.Log ("建筑建造协议成功");
		this.emptyInput ();
	}

	private void onUpgradeReqSuccess (RequestData data)
	{
		Debug.Log ("建筑升级协议成功");
		this.emptyInput ();
	}

	private void onRefreshReqSuccess (RequestData data)
	{
		Debug.Log ("建筑刷新协议成功");
		this.emptyInput ();
	}

	public override void OnEnter ()
	{
		base.OnEnter ();
		this.showBuildInfo ();
		PushEventNotifyCenter.Instance.AddNotification (ProtocolFeature.OnBuild, this);
	}

	public override void OnExit ()
	{
		base.OnExit ();
		PushEventNotifyCenter.Instance.RemoveObserver (ProtocolFeature.OnBuild, this);
	}

	public override void OnResume ()
	{
		base.OnResume ();
		this.OnBuild ();
	}

	private void OnBuild ()
	{
		this.showBuildInfo ();
	}
}
