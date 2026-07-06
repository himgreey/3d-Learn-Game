using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPoint : MonoBehaviour
{
    [Header("准备时间")]
    [SerializeField] private float prepareTime = 15f;

    [Header("波次间隔")]
    [SerializeField] private float waveInterval = 8f;

    [Header("每波怪物数量")]
    [SerializeField] private int baseMonstersPerWave = 3;
    [SerializeField] private int monstersPerWaveGrowth = 1;
    [SerializeField] private int maxMonstersPerWave = 15;

    [Header("单只怪物生成间隔（秒，越小越快）")]
    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private float spawnIntervalDecreasePerWave = 0.15f;
    [SerializeField] private float minSpawnInterval = 0.4f;

    [Header("怪物强度（id 越高越强）")]
    [SerializeField] private int maxMonsterId = 6;

    public int CurrentWave { get; private set; }

    private Coroutine spawnCoroutine;
    private bool ownsWaveUi;

    private static bool waveUiOwnerTaken;

    void Start()
    {
        if (!waveUiOwnerTaken)
        {
            waveUiOwnerTaken = true;
            ownsWaveUi = true;
        }

        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    void OnDestroy()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        if (ownsWaveUi)
        {
            waveUiOwnerTaken = false;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return WaitForGamePanel();
        yield return WaitForPlayer();

        float remain = prepareTime;
        while (remain > 0f)
        {
            if (ShouldStopSpawning())
            {
                yield break;
            }

            UpdatePrepareUi(remain);
            remain -= Time.deltaTime;
            yield return null;
        }

        CurrentWave = 1;
        while (true)
        {
            if (ShouldStopSpawning())
            {
                yield break;
            }

            UpdateWaveUi(CurrentWave);
            GameSessionStats.Instance.SetWave(CurrentWave);

            int count = GetMonstersPerWave(CurrentWave);
            float interval = GetSpawnInterval(CurrentWave);

            for (int i = 0; i < count; i++)
            {
                if (ShouldStopSpawning())
                {
                    yield break;
                }

                SpawnMonster(GetMonsterIdForWave(CurrentWave));
                if (i < count - 1)
                {
                    yield return new WaitForSeconds(interval);
                }
            }

            float nextWaveRemain = waveInterval;
            while (nextWaveRemain > 0f)
            {
                if (ShouldStopSpawning())
                {
                    yield break;
                }

                UpdateNextWaveUi(nextWaveRemain);
                nextWaveRemain -= Time.deltaTime;
                yield return null;
            }

            CurrentWave++;
        }
    }

    private IEnumerator WaitForGamePanel()
    {
        while (GetGamePanel() == null)
        {
            yield return null;
        }
    }

    private IEnumerator WaitForPlayer()
    {
        while (PlayerObject.Instance == null)
        {
            yield return null;
        }
    }

    private GamePanel GetGamePanel()
    {
        return UIManager.Instance.GetPanel<GamePanel>();
    }

    private void UpdatePrepareUi(float remainSeconds)
    {
        if (!ownsWaveUi) return;

        GamePanel panel = GetGamePanel();
        if (panel != null)
        {
            panel.UpdatePrepare(remainSeconds);
        }
    }

    private void UpdateWaveUi(int wave)
    {
        if (!ownsWaveUi) return;

        GamePanel panel = GetGamePanel();
        if (panel != null)
        {
            panel.UpdateWave(wave);
        }
    }

    private void UpdateNextWaveUi(float remainSeconds)
    {
        if (!ownsWaveUi) return;

        GamePanel panel = GetGamePanel();
        if (panel != null)
        {
            panel.UpdateNextWave(remainSeconds);
        }
    }

    private int GetMonstersPerWave(int wave)
    {
        return Mathf.Min(maxMonstersPerWave, baseMonstersPerWave + (wave - 1) * monstersPerWaveGrowth);
    }

    private float GetSpawnInterval(int wave)
    {
        return Mathf.Max(minSpawnInterval, baseSpawnInterval - (wave - 1) * spawnIntervalDecreasePerWave);
    }

    private int GetMaxIdForWave(int wave)
    {
        return Mathf.Clamp(1 + wave / 2, 1, maxMonsterId);
    }

    private int GetMonsterIdForWave(int wave)
    {
        int maxId = GetMaxIdForWave(wave);
        return Random.Range(1, maxId + 1);
    }

    private void SpawnMonster(int monsterId)
    {
        List<MonsterInfo> infos = GameDataManager.Instance.MonsterInfos;
        if (infos == null)
        {
            Debug.LogError("MonsterPoint: MonsterInfos 为空，无法刷怪");
            return;
        }

        MonsterInfo info = infos.Find(m => m.id == monsterId);
        if (info == null)
        {
            Debug.LogError("MonsterPoint: 找不到怪物配置 id=" + monsterId);
            return;
        }

        GameObject prefab = Resources.Load<GameObject>(info.res);
        if (prefab == null)
        {
            Debug.LogError("MonsterPoint: 找不到怪物预制体 " + info.res);
            return;
        }

        GameObject obj = Instantiate(prefab, transform.position, transform.rotation);

        MonsterObject monster = obj.GetComponent<MonsterObject>();
        if (monster == null)
        {
            monster = obj.AddComponent<MonsterObject>();
        }

        monster.Init(info);
    }

    private bool ShouldStopSpawning()
    {
        return PlayerObject.Instance != null && PlayerObject.Instance.IsDead;
    }
}
