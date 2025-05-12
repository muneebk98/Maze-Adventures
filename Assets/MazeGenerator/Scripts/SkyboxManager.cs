using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [Header("Skybox Materials")]
    public Material[] skyboxMaterials; // Array of different skybox materials
    public int currentSkyboxIndex = 0; // Current skybox index
    
    [Header("Skybox Settings")]
    public float rotationSpeed = 0.5f; // Rotation speed for skybox
    public bool rotateSkybox = true;   // Whether to rotate the skybox
    
    private void Start()
    {
        // Apply the initial skybox
        if (skyboxMaterials != null && skyboxMaterials.Length > 0 && skyboxMaterials[currentSkyboxIndex] != null)
        {
            ApplySkybox(currentSkyboxIndex);
        }
    }
    
    private void Update()
    {
        // Rotate the skybox if enabled
        if (rotateSkybox)
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
        }
    }
    
    // Apply a skybox by index
    public void ApplySkybox(int index)
    {
        if (skyboxMaterials == null || skyboxMaterials.Length == 0)
        {
            Debug.LogWarning("No skybox materials assigned!");
            return;
        }
        
        // Ensure index is within bounds
        index = Mathf.Clamp(index, 0, skyboxMaterials.Length - 1);
        
        // Set the current index
        currentSkyboxIndex = index;
        
        // Apply the skybox material
        if (skyboxMaterials[index] != null)
        {
            RenderSettings.skybox = skyboxMaterials[index];
            
            // Force skybox to update
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning("Skybox material at index " + index + " is null!");
        }
    }
    
    // Switch to the next skybox
    public void NextSkybox()
    {
        int nextIndex = (currentSkyboxIndex + 1) % skyboxMaterials.Length;
        ApplySkybox(nextIndex);
    }
    
    // Switch to the previous skybox
    public void PreviousSkybox()
    {
        int prevIndex = (currentSkyboxIndex - 1 + skyboxMaterials.Length) % skyboxMaterials.Length;
        ApplySkybox(prevIndex);
    }
} 