using UnityEngine;
using System.Collections;

public class MainSceneStart : MonoBehaviour {

	public void Start()
	{
		UIManager.Instance.PushWindow(UIWindowDefine.MainWindow);
	}
}
