using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIImageCollide : Image
{
	public override bool IsRaycastLocationValid (Vector2 screenPoint, Camera eventCamera)
	{
		return true;
	}

	protected override void Start ()
	{
		base.Start ();
		this.color = new Color (255, 255, 255, 0);
	}
}
