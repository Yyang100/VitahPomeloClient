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
		Loom.DispatchToMainThread(() => SceneManager.LoadScene("Main"));
	}
}
