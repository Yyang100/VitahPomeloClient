using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UILoginWindow : MonoBehaviour {

	// 用于绑定登陆按钮
	[SerializeField]
	private Button loginBtn;

	public void Start()
	{
		// 断言，判断按钮对象是否不为空
		Assert.IsNotNull(this.loginBtn);

		// 绑定按钮点击响应函数
		this.loginBtn.onClick.AddListener(this.OnButtonLoginClick);
	}
		
	private void OnButtonLoginClick()
	{
		Debug.Log ("登陆按钮点击");
	}
}
