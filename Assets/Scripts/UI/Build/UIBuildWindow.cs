using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UIBuildWindow : UIWindow {

	[SerializeField]
	private Text buildInfoText;

	[SerializeField]
	private Button closeBtn;

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
		Assert.IsNotNull (this.closeBtn);
		Assert.IsNotNull (this.buildBtn);
		Assert.IsNotNull (this.upgradeBtn);
		Assert.IsNotNull (this.refreshBtn);
		Assert.IsNotNull (this.buildInput);
		Assert.IsNotNull (this.upgradeInput);
		Assert.IsNotNull (this.refreshInput);
	}

	private void showBuildInfo(){
		this.buildInfoText.text = string.Empty;
	}

	private string getBuildInfoStr(){
		return "建筑ID:" + DataPool.Instance.Role.Uid.ToString () +
		"  建筑类型:" + DataPool.Instance.Role.Name +
		"  建筑等级:" + DataPool.Instance.Role.Lv.ToString () +
		"  建筑升级结束时间:" + DataPool.Instance.Role.Gold.ToString ();
	}

	private void onBuildBtnClick (){
		BuildBuildRequest req = new BuildBuildRequest ();
		req.OnSuccess = this.onBuildReqSuccess;
		int build_type = 100;
		req.build (build_type);
	}

	private void onUpgradeBtnClick (){
		BuildUpgradeRequest req = new BuildUpgradeRequest ();
		req.OnSuccess = this.onUpgradeReqSuccess;
		int build_id = 100;
		req.upgrade (build_id);
	}

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
	}
		
	public override void OnExit()
	{
		base.OnExit();
	}
		
	public override void OnResume()
	{
		base.OnResume();
	}
}
