using UnityEngine;
using UnityEngine.AI;

enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}

public class SimpleChasingEnemy : Entity
{
    [SerializeField] private Animator animator;

    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRange;

    private Player player;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private EnemyState currentState = EnemyState.Idle;
    private EnemyState previousState = EnemyState.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!player || !GameManager.Instance.IsGameInPlay()) return;

        float distance = Vector3.Distance(transform.position, player.GetPosition());

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.GetPosition());

            if (distance <= attackRange && Time.time > lastAttackTime)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.ResetPath(); // stop moving if player is far away
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Attack()
    {
        animator.SetBool("isShooting", true);
        agent.ResetPath();
    }

    protected override void onDeath()
    {
        GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>().OnEnemyKilled();
        Destroy(gameObject);
    }

    protected override void OnTakeDamage(int damage)
    {
        Debug.Log($"Enemy took {damage} damage.");
    }
}