using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Drag your Maze GameObject (with the MazeSpawner) into this slot
    public MazeSpawner mazeSpawner;

    // Sizes for each of the 6 levels (rows × columns)
    public Vector2Int[] levelSizes = {
        new Vector2Int(5, 5),
        new Vector2Int(6, 6),
        new Vector2Int(7, 7),
        new Vector2Int(8, 8),
        new Vector2Int(9, 9),
        new Vector2Int(10,10)
    };

    [Header("Scene Management")]
    public string mainMenuSceneName = "MainMenu";
    
    // Reference to the heart manager
    private HeartManager heartManager;
    
    private int currentLevel = 0;

    void Start()
    {
        // Find or create a heart manager
        heartManager = FindObjectOfType<HeartManager>();
        if (heartManager == null)
        {
            // Create a heart manager if one doesn't exist
            GameObject heartManagerObject = new GameObject("Heart Manager");
            heartManager = heartManagerObject.AddComponent<HeartManager>();
            Debug.Log("Created new HeartManager for heart spawning");
        }
        
        // Check if we have a selected level from the main menu
        if (PlayerPrefs.HasKey("SelectedLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("SelectedLevel");
            // Ensure the level is within valid range
            currentLevel = Mathf.Clamp(currentLevel, 0, levelSizes.Length - 1);
        }
        
        GenerateCurrentLevel();
    }

    private void ResetPlayerPosition()
    {
        // Try to find the Invector character first
        InvectorMazeAdapter invectorPlayer = FindObjectOfType<InvectorMazeAdapter>();
        if (invectorPlayer != null)
        {
            invectorPlayer.ResetPosition();
            Debug.Log("Reset Invector character position");
            return;
        }
        
        // Fall back to original PlayerController if Invector character isn't found
        PlayerController originalPlayer = FindObjectOfType<PlayerController>();
        if (originalPlayer != null)
        {
            originalPlayer.ResetPosition();
            Debug.Log("Reset original player position");
            return;
        }
        
        Debug.LogWarning("No player controller found!");
    }

    void GenerateCurrentLevel()
    {
        // 1) Update dimensions and generate maze
        mazeSpawner.GenerateMaze(levelSizes[currentLevel].x, levelSizes[currentLevel].y);
        Debug.Log($"Next Level: {currentLevel + 1} / {levelSizes.Length}");
        
        // 2) Reset player position using the new method
        ResetPlayerPosition();
        
        // 3) Reset timer, score, and health
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ResetTimer();
            uiManager.ResetScore(); // Reset score when level starts
            uiManager.ResetHealth(); // Reset health when level starts
        }
        
        // 4) Spawn collectible orbs
        OrbSpawner orbSpawner = mazeSpawner.GetComponent<OrbSpawner>();
        if (orbSpawner != null)
        {
            orbSpawner.SpawnOrbs();
        }
        else
        {
            Debug.LogError("OrbSpawner component not found on the Maze object!");
        }
        
        // 5) Spawn health hearts using new HeartManager
        if (heartManager != null)
        {
            heartManager.SpawnHeartsForLevel();
            Debug.Log("Spawned hearts using HeartManager");
        }
        else
        {
            Debug.LogError("HeartManager not found! No hearts will be spawned.");
        }
        
        // 6) Spawn traps
        TrapSpawner trapSpawner = mazeSpawner.GetComponent<TrapSpawner>();
        if (trapSpawner != null)
        {
            trapSpawner.SpawnTraps();
        }
        else
        {
            Debug.LogWarning("TrapSpawner component not found on the Maze object. Traps will not be spawned.");
        }
    }

    public void OnPlayerExit()
    {
        currentLevel++;
        if (currentLevel < levelSizes.Length)
        {
            GenerateCurrentLevel();
        }
        else
        {
            // Player has completed all levels - show win screen instead of returning to main menu
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null && uiManager.winPanel != null)
            {
                uiManager.ShowWinScreen();
                Debug.Log("Player has completed all levels! Victory!");
            }
            else
            {
                // Fallback if win screen not set up
                Debug.Log("Player has completed all levels! Returning to main menu...");
                ReturnToMainMenu();
            }
        }
    }

    // Getter for current level (used by UIManager)
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // Method to restart the current level without advancing
    public void RestartCurrentLevel()
    {
        // Just regenerate the maze with current level's dimensions
        GenerateCurrentLevel();
        Debug.Log($"Restarting Level: {currentLevel + 1} / {levelSizes.Length}");
    }
    
    // Method to return to the main menu
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
