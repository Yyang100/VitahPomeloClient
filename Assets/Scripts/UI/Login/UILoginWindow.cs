using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILoginWindow : MonoBehaviour
{
	// 用于绑定登陆按钮
	[SerializeField]
	private Button loginBtn;

	public void Start ()
	{
		// 断言，判断按钮对象是否不为空
		Assert.IsNotNull (this.loginBtn);

		// 绑定按钮点击响应函数
		this.loginBtn.onClick.AddListener (this.OnButtonLoginClick);
	}

	private void OnButtonLoginClick ()
	{
		Debug.Log ("登陆按钮点击");

		PomeloManager.Destroy ();
		PomeloManager.Instance.Connect ((code, data) => {
			if (code == ErrorCode.OK) {
				ConnectorEntryRequest request = new ConnectorEntryRequest ();
				request.OnSuccess = this.OnEntry;
				request.Entry ();
			} else {
				Debug.Log ("connect fail");
				this.loginBtn.gameObject.SetActive (true);
			}
		});
	}

	private void OnEntry (RequestData data)
	{
		Debug.Log ("OnEntry");
		Debug.Log (data);

		// 数据池清空
		DataPool.Destroy();

		// 获取相关登陆数据
		LoginData loginData = new LoginData();
		loginData.onSuccess = this.OnLoginDataSuccess;
		loginData.onError = this.OnLoginDataError;
		loginData.Receive();
	}

	// 登陆数据获取成功
	private void OnLoginDataSuccess()
	{
		Loom.DispatchToMainThread(() => SceneManager.LoadScene("Main"));
	}

	// 登陆数据获取失败
	private void OnLoginDataError(RequestData data)
	{
		this.loginBtn.gameObject.SetActive(true);
	}
}
