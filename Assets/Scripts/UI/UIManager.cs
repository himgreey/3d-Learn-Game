using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance = new UIManager();

    public static UIManager Instance => instance;

    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform canvasTransform;

    private int modalPanelCount;

    private UIManager()
    {
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTransform = canvas.transform;
        GameObject.DontDestroyOnLoad(canvas);
    }

    private string GetPanelName<T>() where T : BasePanel
    {
        return typeof(T).Name;
    }

    // 场景切换后面板可能被 Destroy，但字典仍保留引用，需要先清理
    private void RemoveIfDestroyed(string panelName)
    {
        if (panelDic.TryGetValue(panelName, out BasePanel panel) && panel == null)
        {
            panelDic.Remove(panelName);
        }
    }

    public T showPanel<T>() where T : BasePanel
    {
        string panelName = GetPanelName<T>();
        RemoveIfDestroyed(panelName);

        if (panelDic.TryGetValue(panelName, out BasePanel cachedPanel))
        {
            cachedPanel.Show();
            NotifyPanelShown();
            return cachedPanel as T;
        }

        GameObject panelPrefab = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        panelPrefab.transform.SetParent(canvasTransform, false);
        T panel = panelPrefab.GetComponent<T>();
        panelDic[panelName] = panel;
        panel.Show();
        NotifyPanelShown();
        return panel;
    }

    public void hidePanel<T>(bool destroy = true) where T : BasePanel
    {
        string panelName = GetPanelName<T>();
        RemoveIfDestroyed(panelName);

        if (!panelDic.TryGetValue(panelName, out BasePanel panel))
        {
            return;
        }

        if (destroy)
        {
            panel.Hide(() =>
            {
                if (panel != null)
                {
                    GameObject.Destroy(panel.gameObject);
                }
                panelDic.Remove(panelName);
                NotifyPanelHidden();
            });
        }
        else
        {
            GameObject.Destroy(panel.gameObject);
            panelDic.Remove(panelName);
            NotifyPanelHidden();
        }
    }

    public void ResetModalCursorState()
    {
        modalPanelCount = 0;
        GameCursor.TryLockForGameplay(true);
    }

    private void NotifyPanelShown()
    {
        modalPanelCount++;
        GameCursor.UnlockForUI();
    }

    private void NotifyPanelHidden()
    {
        modalPanelCount = Mathf.Max(0, modalPanelCount - 1);
        GameCursor.TryLockForGameplay(modalPanelCount == 0);
    }

    public void RegisterPanel<T>(T panel) where T : BasePanel
    {
        panelDic[GetPanelName<T>()] = panel;
    }

    public void UnregisterPanel<T>() where T : BasePanel
    {
        panelDic.Remove(GetPanelName<T>());
    }

    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = GetPanelName<T>();
        RemoveIfDestroyed(panelName);

        if (panelDic.TryGetValue(panelName, out BasePanel panel))
        {
            return panel as T;
        }
        return null;
    }

    public bool IsPanelShowing<T>() where T : BasePanel
    {
        T panel = GetPanel<T>();
        return panel != null && panel.IsShow;
    }

    public bool HasBlockingModalExceptPause()
    {
        foreach (KeyValuePair<string, BasePanel> entry in panelDic)
        {
            BasePanel panel = entry.Value;
            if (panel == null || !panel.IsShow)
            {
                continue;
            }

            if (entry.Key == "GamePanel" || entry.Key == "GamePausePanel")
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public bool TryDismissModalOnEscape()
    {
        TipPanel tipPanel = GetPanel<TipPanel>();
        if (tipPanel != null && tipPanel.IsShow)
        {
            tipPanel.Dismiss();
            return true;
        }

        return false;
    }
}
