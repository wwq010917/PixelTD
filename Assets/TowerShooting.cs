using UnityEngine;

public class TowerShooting : MonoBehaviour {
    [Header("Tower Settings")]
    [Tooltip("Projectile prefab to shoot.")]
    public GameObject projectilePrefab;
    public float damage = 10f;
    public float fireRate = 1f;

    [Header("Detection Settings")]
    [Tooltip("Detection circle: drag your range circle here.")]
    public Transform detectionCircle;

    private float fireCooldown = 0f;
    private TowerRangeDisplay towerRangeDisplay;

    void Start() {
        towerRangeDisplay = GetComponent<TowerRangeDisplay>();
        if (towerRangeDisplay == null) {
            Debug.LogError("TowerRangeDisplay component is missing on " + gameObject.name);
        }

        if (detectionCircle == null) {
            detectionCircle = transform;
        }
    }

    void Update() {
        if (towerRangeDisplay != null && towerRangeDisplay.IsPlaced) {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f) {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    detectionCircle.position, 
                    towerRangeDisplay.range * 2f, 
                    LayerMask.GetMask("Enemy")
                );

                if (hits.Length > 0) {
                    Transform targetEnemy = hits[0].transform;
                    Vector2 shootDir = (targetEnemy.position - detectionCircle.position).normalized;
                    ShootProjectile(shootDir);
                    fireCooldown = 1f / fireRate;
                }
            }
        }
    }

    void ShootProjectile(Vector2 direction) {
        if (projectilePrefab != null && detectionCircle != null) {
            GameObject proj = Instantiate(projectilePrefab, detectionCircle.position, Quaternion.identity);
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            Projectile projScript = proj.GetComponent<Projectile>();
            if (projScript != null) {
                projScript.Initialize(direction, damage);
            }
        }
    }

    public void UpgradeDamage(float multiplier)
    {
        damage *= (1f + multiplier);
        Debug.Log($"Tower damage increased to: {damage}");
    }

    void OnDrawGizmosSelected() {
        if (detectionCircle != null && towerRangeDisplay != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(detectionCircle.position, towerRangeDisplay.range * 2f);
        }
    }
}