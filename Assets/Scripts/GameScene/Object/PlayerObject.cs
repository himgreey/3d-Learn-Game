using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerObject : MonoBehaviour
{
    public static PlayerObject Instance { get; private set; }

    //玩家对象的属性
    private int hp = 100; //玩家的生命值
    private int maxHp = 100;
    private int atk;
    private int money;
    private float ratate = 100;
    private bool isDead;
    public bool IsDead => isDead;
    public int Money => money;

    private Animator animator;
    public Transform shootPoint;

    [Header("射击检测")]
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private float shootAssistHalfAngle = 30f;

    private void Awake()
    {
        Instance = this;
    }

    internal static void ForceSetInstance(PlayerObject player)
    {
        Instance = player;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        RoleInfo roleInfo = GameDataManager.Instance.NowRoleInfo;
        if (roleInfo != null)
        {
            InitPlayerInfo(roleInfo.hp, roleInfo.atk, GameDataManager.Instance.playerData.havaMoney);
        }
        else
        {
            InitPlayerInfo(hp, atk, money);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        //移动，动作变化
        animator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
        animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));

        this.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * ratate * Time.deltaTime);

        //蹲
        if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetLayerWeight(1, 1);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            animator.SetLayerWeight(1, 0);
        }

        //滚
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Roll");
        }

        //攻击
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void InitPlayerInfo(int hp, int atk, int money)
    {
        this.hp = hp;
        this.maxHp = hp;
        this.atk = atk <= 0 ? 10 : atk;
        this.money = money;
        UpdateMoneyUI();
        UpdateBlood();
    }

    public void KnifeEvent()
    {
        DealMeleeDamage();
    }

    // 动画事件名是 ShootEvent
    public void ShootEvent()
    {
        DealRangedDamage();
    }

    private void DealMeleeDamage()
    {
        if (atk <= 0) return;

        GameDataManager.Instance.PlaySound("Music/Knife");

        int enemyLayer = LayerMask.GetMask("Enemy");
        Vector3 center = transform.position + transform.forward * 1.2f + Vector3.up * 0.8f;
        Collider[] colliders = Physics.OverlapSphere(center, 1.5f, enemyLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            TryDamageMonster(colliders[i]);
        }
    }

    private void DealRangedDamage()
    {
        if (atk <= 0) return;

        GameDataManager.Instance.PlaySound("Music/Gun");

        Transform firePoint = shootPoint != null ? shootPoint : transform;
        int enemyLayer = LayerMask.GetMask("Enemy");
        Vector3 bodyForward = transform.forward;

        if (!TryFindShootTarget(firePoint.position, firePoint.forward, bodyForward, enemyLayer,
            out MonsterObject target, out Vector3 hitPoint, out Vector3 hitNormal))
        {
            return;
        }

        GameObject effPrefab = Resources.Load<GameObject>("eff/1");
        if (effPrefab != null)
        {
            GameObject refobj = Instantiate(effPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(refobj, 1f);
        }

        target.TakeDamage(atk);
    }

    // 先尝试精确射线，未命中再在玩家正面扇形范围内找最近一只怪
    private bool TryFindShootTarget(Vector3 origin, Vector3 aimDirection, Vector3 bodyForward, int enemyLayer,
        out MonsterObject target, out Vector3 hitPoint, out Vector3 hitNormal)
    {
        target = null;
        hitPoint = origin + aimDirection * shootRange;
        hitNormal = -aimDirection;

        if (Physics.Raycast(origin, aimDirection, out RaycastHit rayHit, shootRange, enemyLayer))
        {
            MonsterObject rayMonster = MonsterUtil.GetMonster(rayHit.collider);
            if (rayMonster != null && IsMonsterInFrontCone(origin, bodyForward, rayMonster))
            {
                target = rayMonster;
                hitPoint = rayHit.point;
                hitNormal = rayHit.normal;
                return true;
            }
        }

        Vector3 flatForward = FlattenDirection(bodyForward);
        if (flatForward.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        float minDot = Mathf.Cos(shootAssistHalfAngle * Mathf.Deg2Rad);
        Collider[] colliders = Physics.OverlapSphere(origin, shootRange, enemyLayer);

        float closestDistance = float.MaxValue;
        MonsterObject closestMonster = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            MonsterObject monster = MonsterUtil.GetMonster(colliders[i]);
            if (monster == null) continue;
            if (!IsMonsterInFrontCone(origin, flatForward, monster, minDot)) continue;

            float distance = Vector3.Distance(origin, monster.transform.position);
            if (distance > shootRange || distance >= closestDistance) continue;

            closestDistance = distance;
            closestMonster = monster;
        }

        if (closestMonster == null)
        {
            return false;
        }

        target = closestMonster;
        hitPoint = closestMonster.transform.position + Vector3.up;
        hitNormal = -flatForward;
        return true;
    }

    private static Vector3 FlattenDirection(Vector3 direction)
    {
        direction.y = 0f;
        return direction.normalized;
    }

    private bool IsMonsterInFrontCone(Vector3 origin, Vector3 flatForward, MonsterObject monster, float minDot = -1f)
    {
        if (minDot < 0f)
        {
            minDot = Mathf.Cos(shootAssistHalfAngle * Mathf.Deg2Rad);
        }

        Vector3 toMonster = monster.transform.position - origin;
        Vector3 flatToMonster = FlattenDirection(toMonster);
        if (flatToMonster.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        return Vector3.Dot(flatForward, flatToMonster) >= minDot;
    }

    private void TryDamageMonster(Collider col)
    {
        MonsterObject monster = MonsterUtil.GetMonster(col);
        if (monster != null)
        {
            monster.TakeDamage(atk);
        }
    }

    public void Wound(int value)
    {
        if (isDead || value <= 0)
        {
            return;
        }

        hp -= value;
        if (hp < 0)
        {
            hp = 0;
        }

        UpdateBlood();
        GameDataManager.Instance.PlaySound("Music/Eat");

        if (hp <= 0)
        {
            ShowSettlementScreen(true);
        }
    }

    public void UpdateBlood()
    {
        GamePanel panel = UIManager.Instance.GetPanel<GamePanel>();
        if (panel != null)
        {
            panel.UpdateBlood(hp, maxHp);
        }
    }

    private void UpdateMoneyUI()
    {
        GamePanel panel = UIManager.Instance.GetPanel<GamePanel>();
        if (panel != null)
        {
            panel.UpdateMoney(money);
        }
    }

    public void AddMoney(int value)
    {
        money += value;
        UpdateMoneyUI();
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0 || money >= amount)
        {
            if (amount > 0)
            {
                money -= amount;
                UpdateMoneyUI();
            }
            return true;
        }

        return false;
    }

    public void SettleGame()
    {
        ShowSettlementScreen(false);
    }

    private void ShowSettlementScreen(bool playDeadAnim)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        GamePauseManager.ForceClosePause();
        Time.timeScale = 1f;

        if (playDeadAnim)
        {
            animator.SetTrigger("Dead");
        }

        GameSettlementResult result = GameSessionStats.Instance.BuildSettlement(money);
        GameDataManager.Instance.playerData.havaMoney = money;
        GameDataManager.Instance.SavePlayerData();

        SettlementPanel panel = UIManager.Instance.showPanel<SettlementPanel>();
        panel.ShowSettlement(result, () =>
        {
            UIManager.Instance.UnregisterPanel<GamePanel>();
            SceneManager.LoadScene("BeginScene");
        });
    }
}
