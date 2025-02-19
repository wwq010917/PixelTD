using UnityEngine;
using TMPro;

public class BasementHealth : MonoBehaviour {
    public int health = 10;
    
    [Tooltip("Drag and drop the TextMeshPro text component here.")]
    public TMP_Text healthText;
    
    private GameController gameController;

    void Start() {

        gameController = FindObjectOfType<GameController>();
        if (gameController == null) {
            Debug.LogWarning("No GameController found in the scene!");
        }
        UpdateHealthText();
    }

    // Call this to deduct damage from the basement.
    public void TakeDamage(int damage = 1) {
        health -= damage;
        Debug.Log("Basement took damage. Remaining health: " + health);
        UpdateHealthText();

        if (health <= 0) {
            Debug.Log("Basement destroyed!");
            if (gameController != null) {
                // Trigger game over
                gameController.GameOver();
            }
            Destroy(gameObject);
        }
    }

    void UpdateHealthText() {
        if (healthText != null) {
            healthText.text = health.ToString();
        }
    }
}
