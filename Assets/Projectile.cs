using UnityEngine;

public class Projectile : MonoBehaviour 
{
    public float speed = 15f;
    public float lifetime = 5f;
    
    private float damage;
    private Vector2 direction;
    
    void Start() 
    {
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
    }
    
    void Update() 
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
    
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy")) 
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null) 
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}