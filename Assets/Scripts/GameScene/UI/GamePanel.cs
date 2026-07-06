using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public RawImage PlayerImage;
    public Image BloodPic;
    public Image BloodBKPic;
    public TextMeshProUGUI BloodText;
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI EarnText;
    public float hpW;
    public Transform Bot;
    public System.Collections.Generic.List<Bot> bots = new System.Collections.Generic.List<Bot>();

    private RoleInfo roleInfo;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.RegisterPanel(this);
    }

    private void OnDestroy()
    {
        UIManager.Instance.UnregisterPanel<GamePanel>();
    }

    public override void Init()
    {
        if (GameDataManager.Instance.playerData != null)
        {
            UpdateMoney(GameDataManager.Instance.playerData.havaMoney);
        }

        roleInfo = GameDataManager.Instance.NowRoleInfo;
        if (roleInfo != null)
        {
            hpW = roleInfo.hp * 3;
            UpdateBlood(roleInfo.hp, roleInfo.hp);
            StartCoroutine(LoadPlayerPortrait());
        }
        else
        {
            UpdateBlood(100, 100);
        }

        InitBotTowerPics();
    }

    public bool IsBotVisible => Bot != null && Bot.gameObject.activeSelf;

    public void ShowBot()
    {
        if (Bot != null)
        {
            Bot.gameObject.SetActive(true);
        }
    }

    public void HideBot()
    {
        if (Bot != null)
        {
            Bot.gameObject.SetActive(false);
        }
    }

    public void ToggleBot()
    {
        if (Bot == null)
        {
            return;
        }

        Bot.gameObject.SetActive(!Bot.gameObject.activeSelf);
    }

    private void InitBotTowerPics()
    {
        if (bots == null || GameDataManager.Instance.TowerInfos == null)
        {
            return;
        }

        for (int i = 0; i < bots.Count; i++)
        {
            if (bots[i] == null)
            {
                continue;
            }

            TowerInfo info = GameDataManager.Instance.TowerInfos.Find(t => t.id == i + 1);
            if (info != null)
            {
                bots[i].Init(info);
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePauseManager.HandleEscape();
        }
    }

    private System.Collections.IEnumerator LoadPlayerPortrait()
    {
        yield return null;

        if (roleInfo == null) yield break;
        RolePortraitUtil.Apply(PlayerImage, roleInfo);
    }

    public void UpdateBlood(int Hp, int MaxHp)
    {
        if (BloodText == null || BloodBKPic == null || BloodPic == null) return;
        if (MaxHp <= 0) MaxHp = 1;

        BloodText.text = Hp + " " + MaxHp;
        (BloodBKPic.transform as RectTransform).sizeDelta = new Vector2(hpW, 50);
        (BloodPic.transform as RectTransform).sizeDelta = new Vector2((float)Hp / MaxHp * hpW, 50);
    }

    public void UpdateWave(int Wave)
    {
        if (WaveText != null)
            WaveText.text = "第" + Wave + "波";
    }

    public void UpdatePrepare(float remainSeconds)
    {
        if (WaveText != null)
            WaveText.text = "准备 " + Mathf.CeilToInt(remainSeconds) + "s";
    }

    public void UpdateNextWave(float remainSeconds)
    {
        if (WaveText != null)
            WaveText.text = "下一波 " + Mathf.CeilToInt(remainSeconds) + "s";
    }

    public void UpdateMoney(int money)
    {
        if (EarnText != null)
            EarnText.text = "$" + money;
    }
}

