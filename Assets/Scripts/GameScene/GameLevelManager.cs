using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager
{
    private static GameLevelManager instance = new GameLevelManager();

    public static GameLevelManager Instance => instance;

    public PlayerObject player;

    public void Init()
    {
        RoleInfo nowRoleInfo = GameDataManager.Instance.NowRoleInfo;
        if (nowRoleInfo == null)
        {
            Debug.LogError("GameLevelManager: 未选择角色，无法进入游戏");
            return;
        }

        GameObject bornObj = GameObject.Find("PlayerBorn");
        if (bornObj == null)
        {
            Debug.LogError("GameLevelManager: 场景中找不到 PlayerBorn");
            return;
        }

        GameObject rolePrefab = Resources.Load<GameObject>(nowRoleInfo.res);
        if (rolePrefab == null)
        {
            Debug.LogError("GameLevelManager: 找不到角色预制体 " + nowRoleInfo.res);
            return;
        }

        Transform roleTrans = bornObj.transform;
        GameObject roleObj = GameObject.Instantiate(rolePrefab, roleTrans.position, roleTrans.rotation);
        player = roleObj.GetComponent<PlayerObject>();
        if (player == null)
        {
            player = roleObj.AddComponent<PlayerObject>();
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraMove cameraMove = mainCamera.GetComponent<CameraMove>();
            if (cameraMove != null)
            {
                cameraMove.lookat = roleObj.transform;
            }
        }

        GameSessionStats.Instance.Reset(GameDataManager.Instance.playerData.havaMoney);
        GamePauseManager.ResetForNewGame();

        if (TowerBuildManager.Instance == null)
        {
            GameObject towerManager = new GameObject("TowerBuildManager");
            towerManager.AddComponent<TowerBuildManager>();
        }
    }
}
