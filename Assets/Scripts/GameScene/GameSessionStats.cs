using UnityEngine;

public class GameSettlementResult
{
    public int wave;
    public int kills;
    public int startMoney;
    public int earnedMoney;
    public int endMoney;
    public float survivedSeconds;
    public string roleName;
}

public class GameSessionStats
{
    private static GameSessionStats instance = new GameSessionStats();

    public static GameSessionStats Instance => instance;

    public int StartMoney { get; private set; }
    public int KillCount { get; private set; }
    public int CurrentWave { get; private set; }

    private float startTime;

    public void Reset(int startMoney)
    {
        StartMoney = startMoney;
        KillCount = 0;
        CurrentWave = 0;
        startTime = Time.time;
    }

    public void RecordKill()
    {
        KillCount++;
    }

    public void SetWave(int wave)
    {
        CurrentWave = wave;
    }

    public GameSettlementResult BuildSettlement(int endMoney)
    {
        RoleInfo roleInfo = GameDataManager.Instance.NowRoleInfo;

        return new GameSettlementResult
        {
            wave = CurrentWave,
            kills = KillCount,
            startMoney = StartMoney,
            earnedMoney = endMoney - StartMoney,
            endMoney = endMoney,
            survivedSeconds = Mathf.Max(0f, Time.time - startTime),
            roleName = roleInfo != null ? roleInfo.tips : "未知角色"
        };
    }
}
