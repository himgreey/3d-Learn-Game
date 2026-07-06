using System.Collections.Generic;
using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    public static TowerBuildManager Instance { get; private set; }

    [SerializeField] private float fireDomeDetectRadius = 4f;

    private readonly List<FireDomeSite> sites = new List<FireDomeSite>();
    private FireDomeSite activeSite;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitFireDomes();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (PlayerObject.Instance == null || PlayerObject.Instance.IsDead || GamePauseManager.IsPaused)
        {
            return;
        }

        UpdateActiveSite();
        if (activeSite == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GetGamePanel()?.ToggleBot();
            return;
        }

        GamePanel panel = GetGamePanel();
        if (panel == null || !panel.IsBotVisible)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                activeSite.TryBuildOrUpgrade(i + 1);
                break;
            }
        }
    }

    private void UpdateActiveSite()
    {
        FireDomeSite nearest = null;
        float nearestDistance = float.MaxValue;
        Vector3 playerPos = PlayerObject.Instance.transform.position;

        for (int i = 0; i < sites.Count; i++)
        {
            float distance = HorizontalDistance(playerPos, sites[i].DomeTransform.position);
            if (distance <= fireDomeDetectRadius && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = sites[i];
            }
        }

        if (activeSite == nearest)
        {
            return;
        }

        if (activeSite != null)
        {
            GetGamePanel()?.HideBot();
        }

        activeSite = nearest;
    }

    private void InitFireDomes()
    {
        sites.Clear();
        Transform[] transforms = FindObjectsOfType<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name.StartsWith("FireDome"))
            {
                sites.Add(new FireDomeSite(transforms[i], fireDomeDetectRadius));
            }
        }
    }

    private static GamePanel GetGamePanel()
    {
        return UIManager.Instance.GetPanel<GamePanel>();
    }

    private static float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private class FireDomeSite
    {
        public Transform DomeTransform { get; }

        private readonly float detectRadius;
        private int currentTowerLevel;
        private BuiltTower currentTower;

        public FireDomeSite(Transform domeTransform, float detectRadius)
        {
            DomeTransform = domeTransform;
            this.detectRadius = detectRadius;
        }

        public bool TryBuildOrUpgrade(int targetLevel)
        {
            if (targetLevel < 1 || targetLevel > 3)
            {
                return false;
            }

            if (currentTowerLevel >= targetLevel)
            {
                UIManager.Instance.showPanel<TipPanel>().SetInfo("无法建造或升级到更低等级");
                return false;
            }

            PlayerObject player = PlayerObject.Instance;
            if (player == null || player.IsDead)
            {
                return false;
            }

            if (HorizontalDistance(player.transform.position, DomeTransform.position) > detectRadius)
            {
                return false;
            }

            TowerInfo info = GameDataManager.Instance.GetTowerInfo(targetLevel);
            if (info == null)
            {
                Debug.LogError("找不到炮塔配置 id=" + targetLevel);
                return false;
            }

            if (!player.TrySpendMoney(info.money))
            {
                UIManager.Instance.showPanel<TipPanel>().SetInfo("金币不足，需要 $" + info.money);
                return false;
            }

            GameObject prefab = Resources.Load<GameObject>(info.res);
            if (prefab == null)
            {
                Debug.LogError("找不到炮塔预制体: " + info.res);
                player.AddMoney(info.money);
                return false;
            }

            if (currentTower != null)
            {
                Destroy(currentTower.gameObject);
                currentTower = null;
            }

            Vector3 buildPos = DomeTransform.position;
            buildPos.y = prefab.transform.position.y;

            GameObject towerObj = Instantiate(prefab, buildPos, Quaternion.identity);
            BuiltTower builtTower = towerObj.GetComponent<BuiltTower>();
            if (builtTower == null)
            {
                builtTower = towerObj.AddComponent<BuiltTower>();
            }

            builtTower.Init(info);
            currentTower = builtTower;
            currentTowerLevel = targetLevel;

            GameDataManager.Instance.PlaySound("Music/Tower");
            GetGamePanel()?.HideBot();
            return true;
        }
    }
}
