using SimpleJson;
using System;
using Pomelo.DotNetClient;
using UnityEngine;
using System.Collections.Generic;

public class PushEventNotifyCenter : Singleton<PushEventNotifyCenter>
{
	private Dictionary<ProtocolFeature, List<Component>> pushEventListener = new Dictionary<ProtocolFeature, List<Component>> ();

	public bool AddNotification (ProtocolFeature feature, Component observer)
	{
		List<Component> listListener;
		if (this.pushEventListener.TryGetValue (feature, out listListener) == false) {
			listListener = new List<Component> ();
			this.pushEventListener [feature] = listListener;
		}

		if (listListener.Contains (observer)) {
			return false;
		}

		listListener.Add (observer);
		return true;
	}

	public bool RemoveObserver (ProtocolFeature feature, Component observer)
	{
		List<Component> listListener;
		if (this.pushEventListener.TryGetValue (feature, out listListener) == false) {
			return false;
		}

		return listListener.Remove (observer);
	}

	public void RemoveAllListener ()
	{
		this.pushEventListener.Clear ();
	}

	public void FireChangeEvent (ProtocolFeature feature)
	{
		List<Component> listListener;
		if (this.pushEventListener.TryGetValue (feature, out listListener) == false) {
			return;
		}

		List<Component> observerToRemove = new List<Component> ();
		foreach (Component observer in listListener) {
			if (observer == null) {
				observerToRemove.Add (observer);
			} else {
				observer.SendMessage (EnumUtil.GetString (feature), null, SendMessageOptions.DontRequireReceiver);
			}
		}

		// 删除无用的观察者
		foreach (Component observer in observerToRemove) {
			listListener.Remove (observer);
		}
	}
}