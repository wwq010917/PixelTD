using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour 
{
    [Header("Button References")]
    public Button continueButton;
    public Button upgradeAllButton;
    
    [Header("Tower Spawning")]
    // Array of tower prefabs to spawn from
    public GameObject[] towerPrefabs;
    // Reference to the sprite renderer (tower area) that marks the spawn area
    public SpriteRenderer towerArea;

    private GameController gameController;

    void Start()
    {

        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("No GameController found in scene!");
        }


        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        else
        {
            Debug.LogError("Continue button not assigned to UpgradePanel!");
        }
        

        if (upgradeAllButton != null)
        {
            upgradeAllButton.onClick.AddListener(OnUpgradeAllClicked);
        }
        else
        {
            Debug.LogError("Upgrade All button not assigned to UpgradePanel!");
        }


        gameObject.SetActive(false);
    }
    
    void OnUpgradeAllClicked()
    {
        if (gameController != null)
        {
            gameController.UpgradeAllTowers();
        }
    }

    void OnContinueClicked()
    {
        // Instantiate a random tower within the tower area bounds
        if (towerPrefabs != null && towerPrefabs.Length > 0 && towerArea != null)
        {
            int randomIndex = Random.Range(0, towerPrefabs.Length);
            GameObject towerPrefab = towerPrefabs[randomIndex];
            
            // Use the tower area's sprite bounds to determine the spawn area
            Bounds areaBounds = towerArea.bounds;
            float randomX = Random.Range(areaBounds.min.x, areaBounds.max.x);
            float randomY = Random.Range(areaBounds.min.y, areaBounds.max.y);
            Vector3 spawnPosition = new Vector3(randomX, randomY, towerArea.transform.position.z);

            Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Tower Prefabs array is empty or Tower Area is not assigned!");
        }
        
        // Continue game logic
        if (gameController != null)
        {
            gameController.ContinueGame();
        }
    }

    void OnDestroy()
    {

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }
        
        if (upgradeAllButton != null)
        {
            upgradeAllButton.onClick.RemoveListener(OnUpgradeAllClicked);
        }
    }
}
