using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIPop : MonoBehaviour
{
	public void Start ()
	{
		this.gameObject.GetComponent<Button> ().onClick.AddListener (this.OnButtonPop);
	}

	private void OnButtonPop ()
	{
		UIManager.Instance.PopWindow ();
	}
}