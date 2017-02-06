using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMono<UIManager>
{
    private Stack<UIWindow> stackWindow = new Stack<UIWindow>();

    private Canvas canvas;

    public GameObject PushWindow(string prefabPath, bool hidePre = true)
    {
        if (hidePre && this.stackWindow.Count > 0)
        {
            UIWindow preWindow = this.stackWindow.Peek();
            preWindow.Hide();
        }

        GameObject prefab = (GameObject)Instantiate(Resources.Load(prefabPath, typeof(GameObject)));
        UIWindow window = prefab.GetComponent<UIWindow>();
        if (!window)
        {
            window = prefab.AddComponent<UIWindow>();
        }

        if (window.IsCollide())
        {
            prefab.AddComponent<UIImageCollide>();
        }

        prefab.transform.SetParent(this.canvas.transform, false);
        this.stackWindow.Push(window);
        window.Show();
        window.OnEnter();
        return prefab;
    }

    public void PopWindow()
    {
        if (this.stackWindow.Count <= 0)
        {
            Debug.Log("window stack is empty");
            return;
        }

        UIWindow popWindow = this.stackWindow.Pop();
        popWindow.OnExit();

        if (this.stackWindow.Count <= 0)
        {
            return;
        }

        UIWindow peekWindow = this.stackWindow.Peek();
        peekWindow.Show();
        peekWindow.OnResume();
    }

    public void PopAllWindow()
    {
        if (this.stackWindow.Count <= 0)
        {
            return;
        }

        foreach (UIWindow window in this.stackWindow)
        {
            window.OnExit();
        }

        this.stackWindow.Clear();
    }

    public void PopToTopWindow()
    {
        if (this.stackWindow.Count <= 1)
        {
            return;
        }

        while (this.stackWindow.Count > 1)
        {
            UIWindow window = this.stackWindow.Pop();
            window.OnExit();
        }

        UIWindow topWindow = this.stackWindow.Peek();
        topWindow.Show();
        topWindow.OnResume();
    }

    protected override void Awake()
    {
        base.Awake();
        //this.canvas = GameObject.FindObjectOfType<Canvas>();
        this.canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
}