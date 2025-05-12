using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu UI")]
    public TextMeshProUGUI gameTitleText;
    public Button startGameButton;
    public Button quitButton;
    
    [Header("Level Selection")]
    public GameObject levelSelectionPanel;
    public Button levelSelectButton;
    public Button backButton;
    public Button[] levelButtons;
    
    [Header("Settings")]
    public string gameTitle = "Maze Adventures";
    
    private MainMenu mainMenu;
    
    void Start()
    {
        // Get reference to MainMenu
        mainMenu = GetComponent<MainMenu>();
        if (mainMenu == null)
        {
            mainMenu = gameObject.AddComponent<MainMenu>();
        }
        
        // Set game title
        if (gameTitleText != null)
        {
            gameTitleText.text = gameTitle;
        }
        
        // Initialize level selection panel as hidden
        if (levelSelectionPanel != null)
        {
            levelSelectionPanel.SetActive(false);
        }
        
        // Set up button listeners
        SetupButtonListeners();
    }
    
    void SetupButtonListeners()
    {
        // Main menu buttons
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(() => mainMenu.StartGame());
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => mainMenu.QuitGame());
        }
        
        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.AddListener(ShowLevelSelection);
        }
        
        // Level selection buttons
        if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] != null)
                {
                    int levelIndex = i; // Need to store in local variable for lambda
                    levelButtons[i].onClick.AddListener(() => mainMenu.StartLevel(levelIndex));
                }
            }
        }
        
        // Back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(HideLevelSelection);
        }
    }
    
    void ShowLevelSelection()
    {
        if (levelSelectionPanel != null)
        {
            levelSelectionPanel.SetActive(true);
        }
    }
    
    void HideLevelSelection()
    {
        if (levelSelectionPanel != null)
        {
            levelSelectionPanel.SetActive(false);
        }
    }
} 