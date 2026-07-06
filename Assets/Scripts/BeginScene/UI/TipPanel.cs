using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public Text infoText;
    public Button closeButton;

    private UnityAction closeCallBack;

    public override void Init()
    {
        closeButton.onClick.AddListener(Dismiss);
    }

    public void Dismiss()
    {
        closeCallBack?.Invoke();
        closeCallBack = null;
        UIManager.Instance.hidePanel<TipPanel>();
    }

    public void SetInfo(string info, UnityAction onClose = null)
    {
        infoText.text = info;
        closeCallBack = onClose;
    }
}
