using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public Button startGameButton;
    public Button[] levelSelectButtons;
    public Button quitButton;

    [Header("Settings")]
    public string gameSceneName = "GameScene";
    public int maxLevels = 6;

    void Start()
    {
        // Set up button listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }

        // Set up level selection buttons
        for (int i = 0; i < levelSelectButtons.Length; i++)
        {
            if (levelSelectButtons[i] != null)
            {
                int levelIndex = i; // Need to store in local variable for lambda
                levelSelectButtons[i].onClick.AddListener(() => StartLevel(levelIndex));
            }
        }

        // Set up quit button
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void StartGame()
    {
        // Load the game scene and start from level 0
        PlayerPrefs.SetInt("SelectedLevel", 0);
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < maxLevels)
        {
            // Store the selected level in PlayerPrefs
            PlayerPrefs.SetInt("SelectedLevel", levelIndex);
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
} 