using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    private static GameDataManager instance = new GameDataManager();
    public static GameDataManager Instance => instance;

    public RoleInfo NowRoleInfo;
    public TowerInfo NowTowerInfo;

    public MusicData musicData;
    public PlayerData playerData;

    public List<RoleInfo> RoleInfos;
    public List<MonsterInfo> MonsterInfos;
    public List<TowerInfo> TowerInfos;

    private GameDataManager()
    {
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
        playerData = JsonMgr.Instance.LoadData<PlayerData>("PlayerData");
        RoleInfos = JsonMgr.Instance.LoadData<List<RoleInfo>>("RoleInfo");
        MonsterInfos = JsonMgr.Instance.LoadData<List<MonsterInfo>>("MonsterInfo");
        TowerInfos = JsonMgr.Instance.LoadData<List<TowerInfo>>("TowerInfo");
    }

    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData, "MusicData");
    }

    public void SavePlayerData()
    {
        JsonMgr.Instance.SaveData(playerData, "PlayerData");
    }

    public void PlaySound(string soundName)
    {
        GameObject soundobj = new GameObject();
        AudioSource audioSource = soundobj.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>(soundName);
        audioSource.volume = musicData.SoundVolume;
        audioSource.mute = !musicData.SoundOn;
        audioSource.Play();
        GameObject.Destroy(soundobj, 1);
    }

    public TowerInfo GetTowerInfo(int id)
    {
        return TowerInfos == null ? null : TowerInfos.Find(t => t.id == id);
    }

    public void SpawnEffect(string resPath, Vector3 position, float lifeTime = 1f)
    {
        if (string.IsNullOrEmpty(resPath))
        {
            return;
        }

        GameObject prefab = Resources.Load<GameObject>(resPath);
        if (prefab == null)
        {
            return;
        }

        GameObject effect = GameObject.Instantiate(prefab, position, Quaternion.identity);
        GameObject.Destroy(effect, lifeTime);
    }
}
