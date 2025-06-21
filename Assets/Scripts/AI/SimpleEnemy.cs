using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float detectionRange = 5f;
    public float attackCooldown = 1.5f;
    public float attackRange = 1.5f;

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            FacePlayer();

            if (dist <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.forward = direction;
    }

    void Attack()
    {
        
    }
}
