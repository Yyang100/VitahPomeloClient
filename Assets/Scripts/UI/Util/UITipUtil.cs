using UnityEngine;
using System.Collections;

public class UITipUtil
{
	public static void Show (string tipContent)
	{
		var obj = GameObject.Instantiate (Resources.Load (UIWindowDefine.TipWindow)) as GameObject;
		var tip = obj.GetComponent<UITipWindow> ();
		if (tip != null) {
			tip.TipTextStr = tipContent;
			obj.transform.SetParent (GameObject.Find ("Canvas").GetComponent<Transform> (), false);
		}
	}
}