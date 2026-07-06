using UnityEngine;

public static class MonsterUtil
{
    public static MonsterObject GetMonster(Collider col)
    {
        if (col == null)
        {
            return null;
        }

        MonsterObject monster = col.GetComponent<MonsterObject>();
        if (monster == null)
        {
            monster = col.GetComponentInParent<MonsterObject>();
        }

        if (monster == null || monster.isDead)
        {
            return null;
        }

        return monster;
    }
}
