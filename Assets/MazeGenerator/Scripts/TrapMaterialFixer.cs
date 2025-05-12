using UnityEngine;

public class TrapMaterialFixer : MonoBehaviour
{
    [Header("Materials")]
    public Material defaultTrapMaterial;       // A fallback material to use
    public Material spikesMaterial;            // Material for spikes
    public Material bladeMaterial;             // Material for blades
    public Material trapDoorMaterial;          // Material for trap doors
    
    void Awake()
    {
        // Create default materials if none are assigned
        if (defaultTrapMaterial == null)
        {
            defaultTrapMaterial = CreateDefaultMaterial(Color.red);
        }
        
        if (spikesMaterial == null)
        {
            spikesMaterial = CreateDefaultMaterial(new Color(0.7f, 0.2f, 0.2f)); // Dark red
        }
        
        if (bladeMaterial == null)
        {
            bladeMaterial = CreateDefaultMaterial(new Color(0.2f, 0.2f, 0.7f)); // Dark blue
        }
        
        if (trapDoorMaterial == null)
        {
            trapDoorMaterial = CreateDefaultMaterial(new Color(0.7f, 0.7f, 0.2f)); // Yellow
        }
    }
    
    // Call this method when traps are spawned
    public void FixTrapMaterials(GameObject trap)
    {
        if (trap == null) return;
        
        // Get all renderers in the trap and its children
        Renderer[] renderers = trap.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0) return;
        
        // Choose which material to use based on trap name
        Material materialToUse = defaultTrapMaterial;
        string trapName = trap.name.ToLower();
        
        if (trapName.Contains("spike"))
        {
            materialToUse = spikesMaterial;
        }
        else if (trapName.Contains("blade"))
        {
            materialToUse = bladeMaterial;
        }
        else if (trapName.Contains("trapdoor") || trapName.Contains("trap door"))
        {
            materialToUse = trapDoorMaterial;
        }
        
        // Apply the material to all renderers
        foreach (Renderer rend in renderers)
        {
            Material[] materials = new Material[rend.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = materialToUse;
            }
            rend.materials = materials;
        }
    }
    
    // Create a simple default material
    private Material CreateDefaultMaterial(Color color)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        // If URP shader not found, fallback to standard shader
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            mat = new Material(Shader.Find("Standard"));
        }
        
        mat.color = color;
        return mat;
    }
} 