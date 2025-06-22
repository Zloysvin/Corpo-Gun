using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int health = 100;
    [SerializeField] protected int maxHealth = 100;

    public void TakeDamage(int damage)
    {
        Debug.Log("Took damange: " + damage);

        health -= damage;
        if (health <= 0)
        {
            onDeath();
        }
        else
        {
            OnTakeDamage(damage);
        }
    }

    protected abstract void onDeath();

    protected abstract void OnTakeDamage(int damage);
}
