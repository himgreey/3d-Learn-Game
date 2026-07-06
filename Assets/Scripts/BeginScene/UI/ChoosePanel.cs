using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoosePanel : BasePanel
{
    public Text NameText;
    public Text UserMoneyText;
    public Text BuyText;
    public Button LeftButton;
    public Button RightButton;
    public Button ReturnButton;
    public Button StartButton;
    public Button BuyButton;

    private GameObject heroObj;
    private Transform HeroPos;
    private RoleInfo NowRoleInfo;
    private int NowIndex;
    public override void Init()
    {
        HeroPos = GameObject.Find("HeroPos").transform;
        UserMoneyText.text = GameDataManager.Instance.playerData.havaMoney.ToString();

        ReturnButton.onClick.AddListener(() =>
        {
            Camera.main.GetComponent<CameraAnimator>().TurnRight(() =>
            {
                UIManager.Instance.showPanel<BeginPanel>();
                Debug.Log("show BeginPanel");
            });
            UIManager.Instance.hidePanel<ChoosePanel>();
        });

        LeftButton.onClick.AddListener(() =>
        {
            --NowIndex;
            if (NowIndex < 0)
            {
                NowIndex = GameDataManager.Instance.RoleInfos.Count - 1;
            }
            HeroChanged();
        });

        RightButton.onClick.AddListener(() =>
        {
            ++NowIndex;
            if (NowIndex >= GameDataManager.Instance.RoleInfos.Count)
            {
                NowIndex = 0;
            }
            HeroChanged();
        });

        StartButton.onClick.AddListener(() =>
        {
            GameDataManager.Instance.NowRoleInfo = NowRoleInfo;
            UIManager.Instance.UnregisterPanel<ChoosePanel>();
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync("GameScene");
            Destroy(this.gameObject);
            asyncOp.completed += (op) =>
            {
                GameLevelManager.Instance.Init();
            }; 
        });

        BuyButton.onClick.AddListener(() =>
        {
            if (GameDataManager.Instance.playerData.havaMoney >= NowRoleInfo.lockMoney)
            {
                GameDataManager.Instance.playerData.havaMoney -= NowRoleInfo.lockMoney;
                GameDataManager.Instance.playerData.haveHeros.Add(NowRoleInfo.id);
                UserMoneyText.text = GameDataManager.Instance.playerData.havaMoney.ToString();
                UpdateLockButton();
                GameDataManager.Instance.SavePlayerData();
                UIManager.Instance.showPanel<TipPanel>().SetInfo("购买成功");
            }
            else
            {
                UIManager.Instance.showPanel<TipPanel>().SetInfo("金币不足");
            }
        });
        HeroChanged();
    }

    private void HeroChanged()
    {
        if (heroObj != null)
        {
            Destroy(heroObj);
            heroObj = null;
        }
        NowRoleInfo = GameDataManager.Instance.RoleInfos[NowIndex];
        heroObj = Instantiate(Resources.Load<GameObject>(NowRoleInfo.res), HeroPos.position, HeroPos.rotation);
        Destroy(heroObj.GetComponent<PlayerObject>());
        heroObj.GetComponent<Animator>().applyRootMotion = false;
        NameText.text = NowRoleInfo.tips;
        UpdateLockButton();
    }

    private void UpdateLockButton()
    {
        if (NowRoleInfo.lockMoney > 0 && !GameDataManager.Instance.playerData.haveHeros.Contains(NowRoleInfo.id))
        {
            BuyButton.interactable = true;
            BuyText.text = "$" + NowRoleInfo.lockMoney.ToString();
            StartButton.gameObject.SetActive(false);
        }
        else
        {
            BuyButton.interactable = false;
            BuyText.text = "已解锁";
            StartButton.gameObject.SetActive(true);
        }
    }

    public override void Hide(UnityAction callBack)
    {
        base.Hide(callBack);
        if (heroObj != null)
        {
            DestroyImmediate(heroObj);
            heroObj = null;
        }
    }
}
