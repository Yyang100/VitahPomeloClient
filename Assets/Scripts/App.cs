using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
	void Start ()
	{
		// 初始化loom
		Loom.CreateThreadPoolScheduler ();

		// 载入登陆窗口
		UIManager.Instance.PushWindow (UIWindowDefine.LoginWindow);
	}
}
