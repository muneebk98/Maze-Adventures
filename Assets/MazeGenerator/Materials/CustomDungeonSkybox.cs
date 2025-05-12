using UnityEngine;

public class CustomDungeonSkybox : MonoBehaviour
{
    [Header("Dungeon Skybox Settings")]
    public Color[] dungeonThemes = new Color[]
    {
        // Dark blue/purple theme for lower levels
        new Color(0.1f, 0.1f, 0.3f),
        // Dark red theme 
        new Color(0.3f, 0.05f, 0.05f),
        // Dark green theme
        new Color(0.05f, 0.2f, 0.05f),
        // Orange/fire theme
        new Color(0.4f, 0.2f, 0.0f),
        // Mystical purple
        new Color(0.2f, 0.0f, 0.3f),
        // Deep teal
        new Color(0.0f, 0.25f, 0.25f)
    };
    
    [Header("Ground Colors")]
    public Color[] groundThemes = new Color[]
    {
        // Dark ground for blue theme
        new Color(0.05f, 0.05f, 0.15f),
        // Dark ground for red theme
        new Color(0.15f, 0.02f, 0.02f),
        // Dark ground for green theme
        new Color(0.02f, 0.1f, 0.02f),
        // Dark ground for orange theme
        new Color(0.2f, 0.1f, 0.0f),
        // Dark ground for purple theme
        new Color(0.1f, 0.0f, 0.15f),
        // Dark ground for teal theme
        new Color(0.0f, 0.12f, 0.12f)
    };
    
    [Header("Skybox Configuration")]
    public float exposure = 0.8f;
    public float atmosphereThickness = 0.5f;
    
    private void Start()
    {
        // Find or add the skybox manager
        SkyboxManager skyboxManager = FindObjectOfType<SkyboxManager>();
        if (skyboxManager == null)
        {
            // Create a new game object for skybox management
            GameObject skyboxObj = new GameObject("Skybox Manager");
            skyboxManager = skyboxObj.AddComponent<SkyboxManager>();
            skyboxObj.AddComponent<MazeSkyboxController>();
        }
        
        // Set up the skybox controller if needed
        MazeSkyboxController controller = skyboxManager.GetComponent<MazeSkyboxController>();
        if (controller == null)
        {
            controller = skyboxManager.gameObject.AddComponent<MazeSkyboxController>();
        }
        
        // Apply our dungeon-themed settings
        controller.skyboxTints = dungeonThemes;
        controller.groundColors = groundThemes;
        controller.defaultExposure = exposure;
        controller.defaultAtmosphereThickness = atmosphereThickness;
        
        // Force an update
        controller.enabled = true;
    }
} 