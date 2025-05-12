using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;

#if UNITY_EDITOR
public class FixAurynMaterials : MonoBehaviour
{
    [MenuItem("Tools/Fix AurynSky Materials")]
    public static void FixMaterials()
    {
        // Path to AurynSky materials
        string materialPath = "Assets/AurynSky/Dungeon Pack/Materials";
        
        // Get all material assets at the path
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material", new[] { materialPath });
        
        // Count for the log
        int fixedCount = 0;
        
        foreach (string guid in materialGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            
            if (material != null)
            {
                // Check if the material has an error shader
                if (material.shader == null || material.shader.name == "Hidden/InternalErrorShader")
                {
                    // Try to find the original color
                    Color originalColor = Color.white;
                    if (material.HasProperty("_Color"))
                    {
                        originalColor = material.color;
                    }
                    
                    // Check which render pipeline we're using
                    if (GraphicsSettings.defaultRenderPipeline != null)
                    {
                        // Using URP or HDRP
                        string pipelineName = GraphicsSettings.defaultRenderPipeline.name;
                        
                        if (pipelineName.Contains("Universal") || pipelineName.Contains("URP"))
                        {
                            // Using URP
                            material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        }
                        else if (pipelineName.Contains("HD") || pipelineName.Contains("High Definition"))
                        {
                            // Using HDRP
                            material.shader = Shader.Find("HDRP/Lit");
                        }
                    }
                    else
                    {
                        // Using Built-in Render Pipeline
                        material.shader = Shader.Find("Standard");
                    }
                    
                    // Set the material color to the original color
                    if (material.HasProperty("_BaseColor")) // URP and HDRP
                    {
                        material.SetColor("_BaseColor", originalColor);
                    }
                    else if (material.HasProperty("_Color")) // Built-in
                    {
                        material.SetColor("_Color", originalColor);
                    }
                    
                    // Save the changes
                    EditorUtility.SetDirty(material);
                    fixedCount++;
                }
            }
        }
        
        // Save all assets
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Fixed {fixedCount} materials in {materialPath}");
    }
    
    // Also create a runtime method to fix materials in the scene
    public static void FixMaterialsAtRuntime()
    {
        // Try to find all trap objects
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        
        foreach (GameObject trap in traps)
        {
            FixTrapMaterials(trap);
        }
        
        Debug.Log($"Attempted to fix materials for {traps.Length} traps at runtime");
    }
    
    public static void FixTrapMaterials(GameObject trap)
    {
        if (trap == null) return;
        
        // Get all renderers in the trap and its children
        Renderer[] renderers = trap.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            
            bool materialsChanged = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null || materials[i].shader == null || 
                    materials[i].shader.name == "Hidden/InternalErrorShader")
                {
                    // Replace with a standard material
                    Material newMat = new Material(Shader.Find("Standard"));
                    
                    // Try to get a good color based on trap type
                    string trapName = trap.name.ToLower();
                    if (trapName.Contains("spike"))
                    {
                        newMat.color = new Color(0.7f, 0.2f, 0.2f); // Dark red
                    }
                    else if (trapName.Contains("blade"))
                    {
                        newMat.color = new Color(0.2f, 0.2f, 0.7f); // Dark blue
                    }
                    else if (trapName.Contains("door"))
                    {
                        newMat.color = new Color(0.7f, 0.7f, 0.2f); // Yellow
                    }
                    else
                    {
                        newMat.color = new Color(0.5f, 0.5f, 0.5f); // Gray
                    }
                    
                    materials[i] = newMat;
                    materialsChanged = true;
                }
            }
            
            if (materialsChanged)
            {
                renderer.materials = materials;
            }
        }
    }
}
#endif 