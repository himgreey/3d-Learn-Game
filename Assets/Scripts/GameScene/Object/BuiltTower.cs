using UnityEngine;

public class BuiltTower : MonoBehaviour
{
    private TowerInfo towerInfo;
    private float attackTimer;
    private Transform turret;
    private int enemyLayer;

    public int TowerLevel => towerInfo != null ? towerInfo.id : 0;

    public void Init(TowerInfo info)
    {
        towerInfo = info;
        attackTimer = 0f;
        enemyLayer = LayerMask.GetMask("Enemy");
        turret = transform.Find("Turret");
    }

    private void Update()
    {
        if (towerInfo == null)
        {
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f)
        {
            return;
        }

        MonsterObject target = FindNearestMonster();
        if (target == null)
        {
            return;
        }

        attackTimer = towerInfo.offsetTime;
        FaceTarget(target.transform);
        GameDataManager.Instance.SpawnEffect(towerInfo.eff, target.transform.position + Vector3.up);
        target.TakeDamage(towerInfo.atk);
    }

    private void FaceTarget(Transform target)
    {
        Transform rotatePart = turret != null ? turret : transform;
        Vector3 dir = target.position - rotatePart.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        rotatePart.rotation = Quaternion.LookRotation(dir);
    }

    private MonsterObject FindNearestMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, towerInfo.atkRange, enemyLayer);
        MonsterObject nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            MonsterObject monster = MonsterUtil.GetMonster(hits[i]);
            if (monster == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = monster;
            }
        }

        return nearest;
    }
}
