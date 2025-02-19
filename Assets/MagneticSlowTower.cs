using UnityEngine;

public class MagneticSlowTower : MonoBehaviour 
{
    [Header("Tower Settings")]
    [Tooltip("Fixed slow strength for magnetic link")]
    public float slowStrength = 0.75f;
    
    [Header("Damage Settings")]
    public float damagePerSecond = 2f;
    private float damageTimer = 0f;
    
    [Header("Visual Settings")]
    public Color linkColor = Color.yellow;
    public float lineWidth = 0.3f;
    
    private TowerRangeDisplay towerRangeDisplay;
    private LineRenderer lineRenderer;
    private EnemySteering2D linkedEnemy;
    private EnemyHealth linkedEnemyHealth;
    
    void Start()
    {
        towerRangeDisplay = GetComponent<TowerRangeDisplay>();
        if (towerRangeDisplay == null)
        {
            Debug.LogError("TowerRangeDisplay component missing on " + gameObject.name);
        }
        
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = linkColor;
        lineRenderer.endColor = linkColor;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
    
    void Update()
    {
        if (towerRangeDisplay != null && towerRangeDisplay.IsPlaced)
        {
            if (linkedEnemy == null)
            {
                FindAndLinkNewEnemy();
                if (linkedEnemy == null)
                {
                    lineRenderer.enabled = false;
                }
            }
            else
            {
                float distanceToEnemy = Vector2.Distance(transform.position, linkedEnemy.transform.position);
                if (distanceToEnemy > towerRangeDisplay.range * 2f)
                {
                    UnlinkCurrentEnemy();
                    FindAndLinkNewEnemy();
                }
                else
                {
                    UpdateLinkVisual();
                    linkedEnemy.ApplySlowEffect(slowStrength);
                    
                    damageTimer += Time.deltaTime;
                    if (damageTimer >= 1f)
                    {
                        if (linkedEnemyHealth != null)
                        {
                            linkedEnemyHealth.TakeDamage(damagePerSecond);
                        }
                        damageTimer = 0f;
                    }
                }
            }
        }
    }
    
    void FindAndLinkNewEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            towerRangeDisplay.range * 2f,
            LayerMask.GetMask("Enemy")
        );
        
        foreach (Collider2D hit in hits)
        {
            EnemySteering2D enemy = hit.GetComponent<EnemySteering2D>();
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            
            if (enemy != null && enemyHealth != null)
            {
                linkedEnemy = enemy;
                linkedEnemyHealth = enemyHealth;
                lineRenderer.enabled = true;
                UpdateLinkVisual();
                break;
            }
        }
    }
    
    void UnlinkCurrentEnemy()
    {
        linkedEnemy = null;
        linkedEnemyHealth = null;
        lineRenderer.enabled = false;
        damageTimer = 0f;
    }
    
    void UpdateLinkVisual()
    {
        if (linkedEnemy != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, linkedEnemy.transform.position);
        }
    }

    public void UpgradeDamage(float multiplier)
    {
        damagePerSecond *= (1f + multiplier);
        Debug.Log($"Magnetic tower damage increased to: {damagePerSecond}");
    }
    
    void OnDestroy()
    {
        UnlinkCurrentEnemy();
    }
    
    void OnDrawGizmosSelected()
    {
        if (towerRangeDisplay != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, towerRangeDisplay.range * 2f);
        }
    }
}