using UnityEngine;

public class EnemySteering2D : MonoBehaviour 
{
    [Tooltip("If left empty, the enemy will automatically target the GameObject tagged 'Basement'.")]
    public Transform target;

    [Header("Basement Health Reference (Optional)")]
    [Tooltip("Drag the Basement GameObject (with a BasementHealth component) here if desired.")]
    public BasementHealth basement;

    [Header("Movement Settings")]
    public float baseSpeed = 3f;
    public float rotationSpeed = 200f;

    [Header("Obstacle Avoidance")]
    public float raycastLength = 1f;
    public float avoidStrength = 0.5f;
    [Tooltip("Angle in degrees to check left/right of the forward direction.")]
    public float angleOffset = 30f;
    public LayerMask obstacleLayer;

    private Rigidbody2D rb;
    private float currentSlowStrength = 0f;
    private float slowResetTimer = 0f;
    private const float SLOW_RESET_TIME = 0.1f; // Time before resetting slow if not reapplied

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing on " + gameObject.name);
        }
        
        // Ignore collisions between enemies and towers
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int towerUILayer = LayerMask.NameToLayer("UI");
        Physics2D.IgnoreLayerCollision(enemyLayer, towerUILayer, true);
    }

    void Start()
    {
        if (target == null)
        {
            GameObject baseObj = GameObject.FindGameObjectWithTag("Basement");
            if (baseObj != null)
            {
                target = baseObj.transform;
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Basement' found in the scene.");
            }
        }
    }

    void Update()
    {
        // Reset slow effect if not reapplied recently
        slowResetTimer -= Time.deltaTime;
        if (slowResetTimer <= 0)
        {
            currentSlowStrength = 0f;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Calculate actual speed considering slow effect
        float effectiveSpeed = baseSpeed * (1f - currentSlowStrength);
        
        // Determine direction to target
        Vector2 targetDirection = ((Vector2)target.position - rb.position).normalized;
        Vector2 avoidanceDirection = Vector2.zero;

        // Obstacle avoidance
        RaycastHit2D hitCenter = Physics2D.Raycast(rb.position, targetDirection, raycastLength, obstacleLayer);
        if (hitCenter.collider != null)
        {
            avoidanceDirection += hitCenter.normal;
        }

        Vector2 leftDirection = Quaternion.Euler(0, 0, angleOffset) * targetDirection;
        RaycastHit2D hitLeft = Physics2D.Raycast(rb.position, leftDirection, raycastLength, obstacleLayer);
        if (hitLeft.collider != null)
        {
            avoidanceDirection += hitLeft.normal;
        }

        Vector2 rightDirection = Quaternion.Euler(0, 0, -angleOffset) * targetDirection;
        RaycastHit2D hitRight = Physics2D.Raycast(rb.position, rightDirection, raycastLength, obstacleLayer);
        if (hitRight.collider != null)
        {
            avoidanceDirection += hitRight.normal;
        }

        // Calculate final movement direction
        Vector2 desiredDirection = targetDirection;
        if (avoidanceDirection != Vector2.zero)
        {
            desiredDirection = (targetDirection + avoidanceDirection * avoidStrength).normalized;
        }

        // Rotate towards movement direction
        float desiredAngle = Mathf.Atan2(desiredDirection.y, desiredDirection.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.LerpAngle(rb.rotation, desiredAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.rotation = newAngle;

        // Move with current effective speed
        rb.MovePosition(rb.position + (Vector2)transform.right * effectiveSpeed * Time.fixedDeltaTime);
    }

    public void ApplySlowEffect(float slowStrength)
    {
        currentSlowStrength = Mathf.Max(currentSlowStrength, slowStrength);
        slowResetTimer = SLOW_RESET_TIME; // Reset the timer when slow is applied
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basement"))
        {
            if (basement == null)
            {
                basement = other.GetComponent<BasementHealth>();
            }
            if (basement != null)
            {
                basement.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}