using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettlementPanel : BasePanel
{
    public Text infoText;
    public Button confirmButton;

    private UnityAction confirmCallback;

    public override void Init()
    {
        EnsureUi();

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
    }

    public void ShowSettlement(GameSettlementResult result, UnityAction onConfirm = null)
    {
        Init();
        EnsureUi();
        confirmCallback = onConfirm;

        if (infoText != null)
        {
            infoText.text = BuildDetailText(result);
        }
    }

    private void OnConfirmClicked()
    {
        confirmCallback?.Invoke();
        confirmCallback = null;
        UIManager.Instance.hidePanel<SettlementPanel>();
    }

    private static string BuildDetailText(GameSettlementResult result)
    {
        int minutes = Mathf.FloorToInt(result.survivedSeconds / 60f);
        int seconds = Mathf.FloorToInt(result.survivedSeconds % 60f);

        return "角色：" + result.roleName + "\n"
             + "存活时间：" + minutes + "分" + seconds + "秒\n"
             + "坚持波数：" + result.wave + " 波\n"
             + "击杀怪物：" + result.kills + " 只\n"
             + "本局获得：$" + result.earnedMoney + "\n"
             + "当前金币：$" + result.endMoney;
    }

    private void EnsureUi()
    {
        if (infoText != null && confirmButton != null)
        {
            return;
        }

        Transform info = transform.Find("infoText");
        if (info != null)
        {
            infoText = info.GetComponent<Text>();
        }

        Transform button = transform.Find("ConfirmButton");
        if (button == null)
        {
            button = transform.Find("closeButton");
        }

        if (button != null)
        {
            confirmButton = button.GetComponent<Button>();
        }
    }
}
