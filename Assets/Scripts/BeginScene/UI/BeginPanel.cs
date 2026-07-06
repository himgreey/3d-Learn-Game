using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button StartButton;
    public Button SettingButton; 
    public Button ExitButton;

    public override void Init()
    {
        StartButton.onClick.AddListener(() => {
            Camera.main.GetComponent<CameraAnimator>().TurnLeft(() =>
            {
                //展示选角面板
                UIManager.Instance.showPanel<ChoosePanel>();
            });
            UIManager.Instance.hidePanel<BeginPanel>();
        });
        SettingButton.onClick.AddListener(() => {
            UIManager.Instance.showPanel<SettingPanel>();
        });
        ExitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
