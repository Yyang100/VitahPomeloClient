using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
        this.DestroySelf();
    }

    public virtual void OnResume()
    {
    }

    public virtual bool IsCollide()
    {
        return true;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void DestroySelf()
    {
        UnityEngine.Object.Destroy(this.gameObject);
    }
}