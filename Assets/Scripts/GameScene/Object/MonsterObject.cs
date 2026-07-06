using UnityEngine;
using UnityEngine.AI;

public class MonsterObject : MonoBehaviour
{
    [SerializeField] private int monsterId = 1;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float playerStillSpeedThreshold = 0.05f;
    [SerializeField] private float attackTurnSpeed = 720f;

    private Animator animator;
    private NavMeshAgent agent;
    private MonsterInfo monsterInfo;

    private int hp;
    private float attackTimer;
    private float attackCooldown;
    private bool bornOver;
    private Vector3 lastPlayerPos;
    private bool hasLastPlayerPos;

    public bool isDead { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent 负责位移；Root Motion 会导致“跑步动画在播但位置不动”
        if (animator != null) animator.applyRootMotion = false;
        agent.updatePosition = true;
        agent.updateRotation = false;
        agent.isStopped = true;
    }

    void Start()
    {
        if (monsterInfo == null && GameDataManager.Instance.MonsterInfos != null)
        {
            MonsterInfo info = GameDataManager.Instance.MonsterInfos.Find(m => m.id == monsterId);
            if (info != null) Init(info);
        }
    }

    public void Init(MonsterInfo info)
    {
        monsterInfo = info;
        hp = info.hp;
        attackCooldown = info.atkOffst;

        if (animator != null)
        {
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(info.animator);
            if (controller != null) animator.runtimeAnimatorController = controller;
        }

        if (agent != null)
        {
            agent.speed = info.moveSpeed;
            agent.acceleration = info.moveSpeed;
            agent.angularSpeed = info.roundSpeed;
            agent.stoppingDistance = attackRange * 0.8f;
            agent.isStopped = true;
            agent.ResetPath();
        }

        isDead = false;
        bornOver = false;
        attackTimer = 0f;
        hasLastPlayerPos = false;

        if (animator != null)
        {
            animator.SetBool("dead", false);
            animator.SetBool("run", false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        hp -= damage;

        GameDataManager.Instance.PlaySound("Music/Wound");

        animator.SetTrigger("hurted");
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        GameDataManager.Instance.PlaySound("Music/dead");
        isDead = true;
        animator.SetBool("dead", true);
        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;

        if (PlayerObject.Instance != null)
        {
            PlayerObject.Instance.AddMoney(20);
        }

        GameSessionStats.Instance.RecordKill();

        Destroy(gameObject, 3f);
    }

    public void BornOver()
    {
        bornOver = true;
        hasLastPlayerPos = false;
        agent.isStopped = false;
        animator.SetBool("run", true);
    }

    public void AtkEvent()
    {
        if (isDead || monsterInfo == null || PlayerObject.Instance == null) return;

        Transform player = PlayerObject.Instance.transform;
        FacePlayer(player, true);

        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward + transform.up,
            1f,
            1 << LayerMask.NameToLayer("Player"));

        for (int i = 0; i < hits.Length; i++)
        {
            PlayerObject players = hits[i].GetComponentInParent<PlayerObject>();
            if (players != null)
            {
                players.Wound(monsterInfo.atk);
                break;
            }
        }
    }

    void Update()
    {
        if (isDead || !bornOver || monsterInfo == null || PlayerObject.Instance == null) return;

        Transform player = PlayerObject.Instance.transform;
        float distance = Vector3.Distance(transform.position, player.position);
        bool playerMoving = IsPlayerMoving(player);

        // 始终手动朝向玩家
        FacePlayer(player);

        if (distance <= attackRange)
        {
            // 玩家不动：停下打；玩家在动：边追边打
            agent.isStopped = !playerMoving;
            animator.SetBool("run", playerMoving);

            if (playerMoving)
                agent.SetDestination(player.position);
            else
                agent.ResetPath();

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                FacePlayer(player, true);
                animator.SetTrigger("atk");
                attackTimer = 0f;
            }
        }
        else
        {
            agent.isStopped = false;
            animator.SetBool("run", true);
            agent.SetDestination(player.position);
        }
    }

    private void FacePlayer(Transform player, bool instant = false)
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = instant
            ? targetRot
            : Quaternion.RotateTowards(transform.rotation, targetRot, attackTurnSpeed * Time.deltaTime);
    }

    private bool IsPlayerMoving(Transform player)
    {
        if (!hasLastPlayerPos)
        {
            lastPlayerPos = player.position;
            hasLastPlayerPos = true;
            return false;
        }

        float speed = Vector3.Distance(player.position, lastPlayerPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPlayerPos = player.position;
        return speed >= playerStillSpeedThreshold;
    }
}
