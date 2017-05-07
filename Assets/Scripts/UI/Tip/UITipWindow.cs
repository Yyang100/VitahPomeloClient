using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UITipWindow : MonoBehaviour
{
	[SerializeField]
	private Text tipText;

	public string TipTextStr { get; set; }

	public void Start ()
	{
		Assert.IsNotNull (this.tipText);

		this.tipText.text = this.TipTextStr;
		this.Invoke ("OnPop", 1);
	}

	private void OnPop ()
	{
		GameObject.Destroy (this.gameObject);
	}
}