using UnityEngine;

public class SlowingTower : MonoBehaviour 
{
    [Header("Tower Settings")]
    public float damagePerSecond = 3f;
    public float slowStrength = 0.2f;    // 20% initial slow
    
    [Header("Slow Limits")]
    public float maxSlowStrength = 0.6f;  // 60% maximum slow
    
    private float damageTimer = 0f;
    private TowerRangeDisplay towerRangeDisplay;
    
    void Start()
    {
        towerRangeDisplay = GetComponent<TowerRangeDisplay>();
        if (towerRangeDisplay == null)
        {
            Debug.LogError("TowerRangeDisplay component is missing on " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (towerRangeDisplay != null && towerRangeDisplay.IsPlaced)
        {
            damageTimer += Time.deltaTime;
            
            if (damageTimer >= 1f)
            {
                // Check for enemies in range
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    transform.position, 
                    towerRangeDisplay.range * 2f,
                    LayerMask.GetMask("Enemy")
                );
                
                // Apply slow effect and damage to all enemies in range
                foreach (Collider2D hit in hits)
                {
                    EnemySteering2D enemy = hit.GetComponent<EnemySteering2D>();
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    
                    if (enemy != null)
                    {
                        enemy.ApplySlowEffect(slowStrength);
                        
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(damagePerSecond);
                        }
                    }
                }
                
                damageTimer = 0f;
            }
        }
    }

    public void UpgradeDamage(float multiplier)
    {
        // Increase damage
        damagePerSecond *= (1f + multiplier);
        
        // Increase slow effect (half as much as damage)
        slowStrength *= (1f + (multiplier * 0.5f));
        
        // Clamp slow to maximum
        slowStrength = Mathf.Min(slowStrength, maxSlowStrength);
        
        Debug.Log($"Tower upgraded - Damage: {damagePerSecond:F1}, Slow: {slowStrength * 100:F1}%");
    }
    
    void OnDrawGizmosSelected()
    {
        if (towerRangeDisplay != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, towerRangeDisplay.range * 2f);
        }
    }
}