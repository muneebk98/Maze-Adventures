using UnityEngine;

[RequireComponent(typeof(SkyboxManager))]
public class MazeSkyboxController : MonoBehaviour
{
    [Header("Level-based Skybox")]
    public bool changeSkyboxPerLevel = true;  // Whether to change skybox per level
    public Color[] skyboxTints;               // Different sky tints for each level
    public Color[] groundColors;              // Different ground colors for each level
    
    [Header("Procedural Skybox Settings")]
    public float defaultAtmosphereThickness = 1.0f;
    public float defaultExposure = 1.3f;
    
    private SkyboxManager skyboxManager;
    private LevelManager levelManager;
    private int lastCheckedLevel = -1;
    
    private void Awake()
    {
        // Get the required components
        skyboxManager = GetComponent<SkyboxManager>();
        levelManager = FindObjectOfType<LevelManager>();
        
        if (skyboxManager == null)
        {
            Debug.LogError("SkyboxManager component is required but not found!");
            return;
        }
    }
    
    private void Start()
    {
        // Set initial skybox
        UpdateSkyboxForCurrentLevel();
    }
    
    private void Update()
    {
        // Check if the level has changed
        if (levelManager != null)
        {
            int currentLevel = levelManager.GetCurrentLevel();
            if (currentLevel != lastCheckedLevel)
            {
                lastCheckedLevel = currentLevel;
                UpdateSkyboxForCurrentLevel();
            }
        }
    }
    
    private void UpdateSkyboxForCurrentLevel()
    {
        if (!changeSkyboxPerLevel || levelManager == null)
        {
            return;
        }
        
        // Get current level (zero-based index)
        int currentLevel = levelManager.GetCurrentLevel();
        
        // Create a procedural skybox with colors based on current level
        Color skyTint = GetLevelColor(skyboxTints, currentLevel);
        Color groundColor = GetLevelColor(groundColors, currentLevel);
        
        // Create a new procedural skybox
        Material newSkybox = SkyboxCreator.CreateProceduralSkybox(
            skyTint, 
            defaultAtmosphereThickness, 
            groundColor, 
            defaultExposure
        );
        
        // Add the new skybox to the skybox manager if it doesn't already exist
        if (skyboxManager.skyboxMaterials == null || skyboxManager.skyboxMaterials.Length == 0)
        {
            // Initialize the array if it's empty
            skyboxManager.skyboxMaterials = new Material[1] { newSkybox };
            skyboxManager.ApplySkybox(0);
        }
        else
        {
            // Check if we already have this skybox
            bool skyboxExists = false;
            for (int i = 0; i < skyboxManager.skyboxMaterials.Length; i++)
            {
                // Since we can't directly compare materials, check if this is for the same level
                if (i == currentLevel % skyboxManager.skyboxMaterials.Length)
                {
                    // Replace the existing material
                    skyboxManager.skyboxMaterials[i] = newSkybox;
                    skyboxManager.ApplySkybox(i);
                    skyboxExists = true;
                    break;
                }
            }
            
            if (!skyboxExists)
            {
                // Resize the array to add the new skybox
                System.Array.Resize(ref skyboxManager.skyboxMaterials, skyboxManager.skyboxMaterials.Length + 1);
                skyboxManager.skyboxMaterials[skyboxManager.skyboxMaterials.Length - 1] = newSkybox;
                skyboxManager.ApplySkybox(skyboxManager.skyboxMaterials.Length - 1);
            }
        }
    }
    
    // Get color for the current level from the array
    private Color GetLevelColor(Color[] colors, int level)
    {
        if (colors == null || colors.Length == 0)
        {
            // Default color if none provided
            return new Color(0.5f, 0.7f, 1.0f);
        }
        
        // Cycle through available colors
        return colors[level % colors.Length];
    }
} 