using UnityEngine;

public class Wall : MonoBehaviour
{
    public int cost = 20; 
    public int health = 50; 

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
