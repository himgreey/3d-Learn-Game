using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private float alphaSpeed = 1f;

    public bool IsShow = false;

    private UnityAction hideCallBack = null;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null )
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected virtual void Start()
    {
        Init();
    }

    public abstract void Init();

    public virtual void Show()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0;
        IsShow = true;
    }

    public virtual void Hide(UnityAction callBack)
    {
        if (canvasGroup == null)
        {
            callBack?.Invoke();
            return;
        }
        canvasGroup.alpha = 1;
        IsShow = false;
        hideCallBack = callBack;
    }

    void Update()
    {
        if (canvasGroup == null) return;

        if (IsShow && canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        else if ( !IsShow && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //让面板完全隐藏后，调用回调函数
                hideCallBack?.Invoke();
            }
        }
    }
}
