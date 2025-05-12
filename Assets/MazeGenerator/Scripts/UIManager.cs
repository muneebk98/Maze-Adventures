using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI levelText;
    public Slider healthBar;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public ToggleGroup settingsToggles;
    
    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Button restartLevelButton;
    public Button returnToMenuButton;
    
    [Header("Win Panel")]
    public GameObject winPanel;
    public Button winMenuButton;
    
    [Header("Game Values")]
    public int score = 0;
    public float timeRemaining = 60f;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    
    // Event for health changes
    public event Action<float> OnHealthChanged;
    
    private bool isTimerRunning = true;
    private bool isPaused = false;
    private LevelManager levelManager;
    
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        ResetTimer();
        UpdateUI();
        
        // Ensure panels are hidden initially
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Settings Panel not assigned to UIManager!");
        }
        
        // Ensure game over panel is hidden initially
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Ensure win panel is hidden initially
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        // Set up button listeners
        SetupButtonListeners();
    }
    
    private void SetupButtonListeners()
    {
        // Settings panel buttons
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        
        // Game over buttons
        if (restartLevelButton != null)
        {
            restartLevelButton.onClick.AddListener(RestartGame);
        }
        
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        
        // Win panel button
        if (winMenuButton != null)
        {
            winMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }
    
    void Update()
    {
        // ESC key to toggle settings/pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Only update game logic if not paused
        if (!isPaused)
        {
            // Timer logic
            if (isTimerRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    UpdateTimerUI();
                }
                else
                {
                    // Timer has reached zero
                    timeRemaining = 0;
                    isTimerRunning = false;
                    OnTimerEnd();
                }
            }
            
            // Regularly update level display in case it changes
            UpdateLevelUI();
        }
    }
    
    void UpdateUI()
    {
        UpdateScoreUI();
        UpdateTimerUI();
        UpdateHealthUI();
        UpdateLevelUI();
    }
    
    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
    
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }
    
    void UpdateLevelUI()
    {
        if (levelText != null && levelManager != null)
        {
            levelText.text = "Level: " + (levelManager.GetCurrentLevel() + 1);
        }
    }
    
    void OnTimerEnd()
    {
        Debug.Log("Time's up!");
        
        // Show Game Over screen instead of auto-restart
        ShowGameOver();
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthUI();
        
        // Trigger the health changed event
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player died!");
            
            // Add short delay before showing Game Over
            StartCoroutine(GameOverAfterDelay(1.5f));
        }
    }
    
    // Coroutine to delay Game Over screen after death
    private IEnumerator GameOverAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        // Show Game Over screen
        ShowGameOver();
    }
    
    // Show the Game Over screen
    public void ShowGameOver()
    {
        // Pause the game
        Time.timeScale = 0f;
        
        // Show the Game Over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Game Over panel not assigned!");
        }
    }
    
    public void HealHealth(float healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateHealthUI();
        
        // Trigger the health changed event
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Trigger the health changed event
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void ResetTimer()
    {
        timeRemaining = 60f;
        isTimerRunning = true;
        UpdateTimerUI();
    }
    
    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        // Freeze/unfreeze the game
        Time.timeScale = isPaused ? 0f : 1f;
        
        // Show/hide settings panel
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isPaused);
            Debug.Log("Game " + (isPaused ? "PAUSED" : "RESUMED") + ", Settings panel is " + (isPaused ? "VISIBLE" : "HIDDEN"));
        }
        else
        {
            Debug.LogError("Settings panel reference is missing!");
        }
    }
    
    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }
    
    public void RestartGame()
    {
        // Resume the game
        Time.timeScale = 1f;
        
        // Hide Game Over panel if it's active
        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Hide settings panel if it's active
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
            isPaused = false;
        }
        
        // Then restart the current level
        if (levelManager != null)
        {
            levelManager.RestartCurrentLevel();
            ResetHealth();
            ResetTimer();
        }
    }
    
    public void ReturnToMainMenu()
    {
        // Make sure time scale is normal before switching scenes
        Time.timeScale = 1f;
        
        // Return to main menu
        if (levelManager != null)
        {
            levelManager.ReturnToMainMenu();
        }
    }
    
    // Show the Win screen
    public void ShowWinScreen()
    {
        // Pause the game
        Time.timeScale = 0f;
        
        // Show the Win panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Win panel not assigned!");
        }
    }
} 