using UnityEngine;
using System.Collections.Generic;

public class MultiTargetTower : MonoBehaviour 
{
    [Header("Tower Settings")]
    public int maxTargets = 3;
    public float damagePerSecond = 15f;
    
    [Header("Visual Settings")]
    public Color linkColor = Color.red;
    public float lineWidth = 0.1f;
    
    private TowerRangeDisplay towerRangeDisplay;
    private Dictionary<EnemyHealth, LineRenderer> linkedEnemies = new Dictionary<EnemyHealth, LineRenderer>();
    private float damageTimer = 0f;
    
    void Start()
    {
        towerRangeDisplay = GetComponent<TowerRangeDisplay>();
        if (towerRangeDisplay == null)
        {
            Debug.LogError("TowerRangeDisplay component missing on " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (towerRangeDisplay != null && towerRangeDisplay.IsPlaced)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, 
                towerRangeDisplay.range * 2f,
                LayerMask.GetMask("Enemy")
            );

            if (hits.Length == 0)
            {
                ClearAllLinks();
                return;
            }

            List<EnemyHealth> destroyedEnemies = new List<EnemyHealth>();
            foreach (var enemy in linkedEnemies.Keys)
            {
                if (enemy == null)
                {
                    destroyedEnemies.Add(enemy);
                }
                else
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy > towerRangeDisplay.range * 2f)
                    {
                        destroyedEnemies.Add(enemy);
                    }
                }
            }
            
            foreach (var enemy in destroyedEnemies)
            {
                UnlinkEnemy(enemy);
            }
            
            if (linkedEnemies.Count < maxTargets)
            {
                FindAndLinkNewEnemies();
            }
            
            foreach (var kvp in linkedEnemies)
            {
                if (kvp.Key != null)
                {
                    UpdateLinkVisual(kvp.Value, kvp.Key.transform.position);
                }
            }
            
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                foreach (var enemy in linkedEnemies.Keys)
                {
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damagePerSecond);
                    }
                }
                damageTimer = 0f;
            }
        }
    }
    
    void FindAndLinkNewEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            towerRangeDisplay.range * 2f,
            LayerMask.GetMask("Enemy")
        );
        
        foreach (Collider2D hit in hits)
        {
            if (linkedEnemies.Count >= maxTargets) break;
            
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null && !linkedEnemies.ContainsKey(enemyHealth))
            {
                LineRenderer newLine = CreateLineRenderer();
                linkedEnemies.Add(enemyHealth, newLine);
                UpdateLinkVisual(newLine, enemyHealth.transform.position);
            }
        }
    }
    
    LineRenderer CreateLineRenderer()
    {
        GameObject lineObj = new GameObject("Link_" + linkedEnemies.Count);
        lineObj.transform.SetParent(transform);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = linkColor;
        line.endColor = linkColor;
        line.positionCount = 2;
        return line;
    }
    
    void UpdateLinkVisual(LineRenderer line, Vector3 targetPosition)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetPosition);
    }
    
    void UnlinkEnemy(EnemyHealth enemy)
    {
        if (linkedEnemies.TryGetValue(enemy, out LineRenderer line))
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
            linkedEnemies.Remove(enemy);
        }
    }

    void ClearAllLinks()
    {
        foreach (var line in linkedEnemies.Values)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        linkedEnemies.Clear();
    }
    
    void OnDestroy()
    {
        ClearAllLinks();
    }
    
    void OnDrawGizmosSelected()
    {
        if (towerRangeDisplay != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, towerRangeDisplay.range * 2f);
        }
    }

    public void UpgradeDamage(float multiplier)
    {
        damagePerSecond *= (1f + multiplier);
        Debug.Log($"Tower damage increased to: {damagePerSecond}");
    }
}