using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private UIManager uiManager;
    private LevelManager levelManager;
    
    [Header("Game Over")]
    public bool restartGameOnDeath = true;
    public float delayBeforeRestart = 2.0f;
    
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        
        // Subscribe to health change events
        if (uiManager != null)
        {
            uiManager.OnHealthChanged += CheckHealth;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (uiManager != null)
        {
            uiManager.OnHealthChanged -= CheckHealth;
        }
    }
    
    void CheckHealth(float newHealth)
    {
        // When health reaches zero, respawn
        if (newHealth <= 0)
        {
            // Disable player controls temporarily
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            
            // Wait a moment, then handle player death
            if (restartGameOnDeath)
            {
                // Restart the entire game
                Invoke("RestartGame", delayBeforeRestart);
            }
            else
            {
                // Just restart the current level
                Invoke("RestartLevel", delayBeforeRestart);
            }
        }
    }
    
    void RestartLevel()
    {
        // Re-enable player controls
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Restart the level
        if (levelManager != null)
        {
            levelManager.RestartCurrentLevel();
        }
    }
    
    void RestartGame()
    {
        Debug.Log("Game Over! Restarting game...");
        // Reload the current scene to restart the game completely
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 