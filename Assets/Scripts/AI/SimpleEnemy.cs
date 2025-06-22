using UnityEngine;
using UnityEngine.AI;

public class SimpleChasingEnemy : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    private Player player;
    private NavMeshAgent agent;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.GetPosition());

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.GetPosition());

            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.ResetPath(); // stop moving if player is far away
        }
    }

    void Attack()
    {
        agent.ResetPath(); // stop while attacking
        Debug.Log("Enemy attacks!");
        // Add damage logic or animation trigger here
    }
}