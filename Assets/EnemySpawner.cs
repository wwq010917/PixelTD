using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Drag and drop your enemy prefab here")]
    public GameObject enemyPrefab;
    
    [Tooltip("Base time between enemy spawns in seconds")]
    public float baseSpawnInterval = 2f;
    
    [Tooltip("Optional delay before starting to spawn")]
    public float initialDelay = 0f;
    
    [Tooltip("Small random offset for spawn position")]
    public float positionRandomness = 0.5f;
    
    [Header("Level Scaling")]
    [Tooltip("How much faster spawning gets per level (0.2 = 20% faster)")]
    public float speedScaleFactor = 0.1f;
    
    [Tooltip("Minimum spawn interval (seconds) to prevent too fast spawning")]
    public float minSpawnInterval = 0.1f;
    
    private float currentSpawnInterval;
    private float spawnTimer = 0f;
    private bool hasStarted = false;
    private GameController gameController;
    
    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned on " + gameObject.name);
            enabled = false;
            return;
        }
        
        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogWarning("No GameController found! Spawn scaling will not work.");
        }
        
        UpdateSpawnInterval();
        
        if (initialDelay > 0f)
        {
            Invoke(nameof(StartSpawning), initialDelay);
        }
        else
        {
            StartSpawning();
        }
    }
    
    private void Update()
    {
        if (!hasStarted) return;
        
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    public void UpdateSpeedScaleFactor(float increase)
    {
        speedScaleFactor += increase;
        UpdateSpawnInterval();
    }
    
    private void UpdateSpawnInterval()
    {
        if (gameController != null)
        {
            int currentLevel = gameController.GetCurrentLevel();
            float speedMultiplier = 1f + (speedScaleFactor * (currentLevel - 1));
            currentSpawnInterval = Mathf.Max(baseSpawnInterval / speedMultiplier, minSpawnInterval);
            
            Debug.Log($"Spawn interval updated to {currentSpawnInterval:F2}s (Speed multiplier: {speedMultiplier:F2}x)");
        }
        else
        {
            currentSpawnInterval = baseSpawnInterval;
        }
    }
    
    private void StartSpawning()
    {
        hasStarted = true;
        spawnTimer = 0f;
    }
    
    private void SpawnEnemy()
    {
        Vector2 randomOffset = Random.insideUnitCircle * positionRandomness;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        
        Instantiate(enemyPrefab, spawnPosition, transform.rotation);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, positionRandomness);
    }
}