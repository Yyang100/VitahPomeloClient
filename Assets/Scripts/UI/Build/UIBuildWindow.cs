using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UIBuildWindow : UIWindow {

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
	void Start () {
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

	private void showBuildInfo(){
		this.buildInfoText.text = string.Empty;
		this.buildInput.text = "输入建筑类型";
		this.upgradeInput.text = "输入建筑ID";
		this.refreshInput.text = "输入建筑ID";

		for (int i = 0; i < DataPool.Instance.Build.ListBuildItem.Count; i++) {
			BuildItemData build_item = DataPool.Instance.Build.ListBuildItem [i];

		}
	}

	private string getBuildInfoStr(){
		string info_str = string.Empty;

		for (int i = 0; i < DataPool.Instance.Build.ListBuildItem.Count; i++) {
			BuildItemData build_item = DataPool.Instance.Build.ListBuildItem [i];
			string build_item_str = "建筑ID:" + build_item.BuildId +
			                        "建筑类型：" + build_item.Type +
			                        "建筑等级：" + build_item.Lv +
			                        "升级结束时间：" + build_item.UpEndTime;
			info_str += build_item_str;
		}
		
		return info_str;
	}

	// 建造按钮点击响应
	private void onBuildBtnClick (){
		BuildBuildRequest req = new BuildBuildRequest ();
		req.OnSuccess = this.onBuildReqSuccess;
		int build_type = 100;
		req.build (build_type);
	}

	// 升级按钮点击响应
	private void onUpgradeBtnClick (){
		BuildUpgradeRequest req = new BuildUpgradeRequest ();
		req.OnSuccess = this.onUpgradeReqSuccess;
		int build_id = 100;
		req.upgrade (build_id);
	}

	// 刷新按钮点击响应
	private void onRefreshBtnClick (){
		BuildRefreshRequest req = new BuildRefreshRequest ();
		req.OnSuccess = this.onRefreshReqSuccess;
		int build_id = 100;
		req.refresh (build_id);
	}

	private void onBuildReqSuccess(RequestData data){
		Debug.Log("建筑建造协议成功");
	}

	private void onUpgradeReqSuccess(RequestData data){
		Debug.Log("建筑升级协议成功");
	}

	private void onRefreshReqSuccess(RequestData data){
		Debug.Log("建筑刷新协议成功");
	}

	public override void OnEnter()
	{
		base.OnEnter();
		PushEventNotifyCenter.Instance.AddNotification(ProtocolFeature.OnBuild, this);
	}
		
	public override void OnExit()
	{
		base.OnExit();
		PushEventNotifyCenter.Instance.RemoveObserver(ProtocolFeature.OnBuild, this);
	}
		
	public override void OnResume()
	{
		base.OnResume();
		this.showBuildInfo ();
	}
}
