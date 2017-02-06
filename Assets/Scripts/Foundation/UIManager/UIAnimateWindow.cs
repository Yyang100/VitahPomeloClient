using UnityEngine;
using System.Collections;

public class UIAnimateWindow : UIWindow
{
    private Animator animatorObject;

    public override void OnEnter()
    {
        this.animatorObject.SetTrigger("OnEnter");
    }

    public override void OnExit()
    {
        this.animatorObject.SetTrigger("OnExit");
    }

    public virtual void OnPause()
    {
        this.animatorObject.SetTrigger("OnPause");
    }

    public override void OnResume()
    {
        this.animatorObject.SetTrigger("OnResume");
    }

    public override bool IsCollide()
    {
        return false;
    }

    public virtual void OnExitCallback()
    {
        this.DestroySelf();
    }

    private void Awake()
    {
        this.animatorObject = this.GetComponent<Animator>();
    }
}