using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
    public float health = 20f;
    
    private GameController gameController;
    
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogWarning("No GameController found in the scene!");
        }
    }

    public void TakeDamage(float damage) 
    {
        health -= damage;
        if (health <= 0f) 
        {

            if (gameController != null)
            {
                gameController.OnEnemyKilled();
            }
            
            Destroy(gameObject);
        }
    }
}