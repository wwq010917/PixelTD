using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour 
{
    [Header("Experience Settings")]
    public float expPerKill = 1f;
    public float currentExp = 0f;
    public int currentLevel = 1;
    
    [Header("Upgrade Settings")]
    public float damageUpgradeAmount = 0.25f;  
    public float spawnSpeedIncrease = 0.035f; 
    
    [Header("UI References")]
    public UpgradePanel upgradePanel;
    
    [Header("Game Over UI")]
    [Tooltip("Game Over Panel that shows the kill count and restart button.")]
    public GameObject gameOverPanel;
    [Tooltip("TMP Text component to display the number of minions killed.")]
    public TMP_Text killCountText;  

    [Header("Restart Button")]
    public Button restartButton;

    private List<float> expThresholds = new List<float>();
    private float nextLevelExp;
    private bool isUpgrading = false;
    private EnemySpawner[] enemySpawners;
    

    private int killCount = 0;

    void Start()
    {
        InitializeExpThresholds();
        nextLevelExp = expThresholds[0];
        
        if (upgradePanel == null)
        {
            Debug.LogError("UpgradePanel not assigned to GameController!");
        }
        
        enemySpawners = FindObjectsOfType<EnemySpawner>();
        if (enemySpawners == null)
        {
            Debug.LogWarning("No EnemySpawner found in scene!");
        }
        

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameOverPanel not assigned to GameController!");
        }


        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("Restart button not assigned to GameController!");
        }
        
        Debug.Log($"Starting game. Need {nextLevelExp} exp for first level");
    }
    
    void InitializeExpThresholds()
    {
        float a = 10f, b = 10f;
        expThresholds.Add(a);
        
        for (int i = 0; i < 20; i++)
        {
            expThresholds.Add(b);
            float temp = a/2 + b;
            a = b;
            b = temp;
        }
        
        string sequence = "EXP thresholds: ";
        foreach (float threshold in expThresholds)
        {
            sequence += threshold + ", ";
        }
        Debug.Log(sequence);
    }
    
    public void AddExperience(float exp)
    {
        if (isUpgrading) return;
        
        currentExp += exp;
        killCount++; 
        Debug.Log($"Got {exp} exp. Current: {currentExp}/{nextLevelExp} | Total Kills: {killCount}");
        
        if (currentExp >= nextLevelExp)
        {
            LevelUp();
        }
    }
    
    void LevelUp()
    {
        currentLevel++;
        currentExp = 0f;
        
        if (currentLevel <= expThresholds.Count)
        {
            nextLevelExp = expThresholds[currentLevel - 1];
        }
        else
        {
            nextLevelExp = nextLevelExp * 1.618f;
        }
        if (enemySpawners != null)
        {
            foreach (var spawner in enemySpawners)
            {
                spawner.UpdateSpeedScaleFactor(spawnSpeedIncrease);
            }
            Debug.Log($"Enemy spawn speed increased by {spawnSpeedIncrease * 100}%");
        }
        ShowUpgradePanel();
        
        Debug.Log($"Level Up! Now level {currentLevel}. Next level needs {nextLevelExp} exp");
    }

    public void UpgradeAllTowers()
    {
        // Find all tower types in the scene
        SlowingTower[] slowingTowers = FindObjectsOfType<SlowingTower>();
        MagneticSlowTower[] magneticTowers = FindObjectsOfType<MagneticSlowTower>();
        MultiTargetTower[] multiTowers = FindObjectsOfType<MultiTargetTower>();
        TowerShooting[] shootingTowers = FindObjectsOfType<TowerShooting>();

        // Upgrade each tower type
        foreach (var tower in slowingTowers)
        {
            tower.UpgradeDamage(damageUpgradeAmount);
        }

        foreach (var tower in magneticTowers)
        {
            tower.UpgradeDamage(damageUpgradeAmount);
        }

        foreach (var tower in multiTowers)
        {
            tower.UpgradeDamage(damageUpgradeAmount);
        }

        foreach (var tower in shootingTowers)
        {
            tower.UpgradeDamage(damageUpgradeAmount);
        }

        Debug.Log($"All towers damage increased by {damageUpgradeAmount * 100}%");
        
        // Continue game after upgrade
        ContinueGame();
    }

    void ShowUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.gameObject.SetActive(true);
            Time.timeScale = 0f;
            isUpgrading = true;
        }
    }
    
    public void ContinueGame()
    {
        if (upgradePanel != null)
        {
            upgradePanel.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        isUpgrading = false;
        
        Debug.Log($"Continuing game. Need {nextLevelExp - currentExp} more exp for next level");
    }
    
    public void OnEnemyKilled()
    {
        AddExperience(expPerKill);
    }
    

    public void GameOver()
    {
        Time.timeScale = 0f; 
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (killCountText != null)
            {
                killCountText.text = "Minions Killed: " + killCount;
            }
        }
        else
        {
            Debug.LogWarning("GameOverPanel not assigned!");
        }
        
        Debug.Log("Game Over!");
    }
    
  
    public void RestartGame()
    {
        Time.timeScale = 1f;  

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Helper methods
    public float GetCurrentExp() { return currentExp; }
    public float GetNextLevelExp() { return nextLevelExp; }
    public int GetCurrentLevel() { return currentLevel; }
    public bool IsUpgrading() { return isUpgrading; }
}
