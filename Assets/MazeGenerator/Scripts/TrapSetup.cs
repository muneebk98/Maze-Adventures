using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
public class TrapSetup : MonoBehaviour
{
    [Header("Death Traps Asset")]
    public Object[] sourceTraps; // Drag Death Traps prefabs here
    
    [Header("Output Settings")]
    public string outputPath = "Assets/MazeGenerator/Prefabs/Traps/";
    
    [ContextMenu("Setup Traps")]
    public void SetupTraps()
    {
        if (sourceTraps == null || sourceTraps.Length == 0)
        {
            Debug.LogError("No trap prefabs assigned! Please assign prefabs from the Death Traps asset.");
            return;
        }
        
        List<GameObject> createdPrefabs = new List<GameObject>();
        
        // Create folder if it doesn't exist
        if (!System.IO.Directory.Exists(outputPath))
        {
            System.IO.Directory.CreateDirectory(outputPath);
        }
        
        foreach (Object sourceTrap in sourceTraps)
        {
            if (sourceTrap == null) continue;
            
            // Instantiate original prefab
            GameObject sourceObj = sourceTrap as GameObject;
            if (sourceObj == null)
            {
                Debug.LogError($"Object {sourceTrap.name} is not a GameObject!");
                continue;
            }
            
            GameObject trapInstance = PrefabUtility.InstantiatePrefab(sourceObj) as GameObject;
            trapInstance.name = sourceObj.name + "_trap";
            
            // Add required components
            BoxCollider collider = trapInstance.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = trapInstance.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;
            collider.size = new Vector3(1, 0.5f, 1);
            collider.center = new Vector3(0, 0.25f, 0);
            
            // Add the trap behavior script
            TrapBehavior trapBehavior = trapInstance.GetComponent<TrapBehavior>();
            if (trapBehavior == null)
            {
                trapBehavior = trapInstance.AddComponent<TrapBehavior>();
            }
            
            // Set trap-specific settings based on names
            SetTrapProperties(trapBehavior, sourceObj.name);
            
            // Add tag
            trapInstance.tag = "Trap";
            
            // Create prefab at the specified location
            string prefabPath = outputPath + trapInstance.name + ".prefab";
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(trapInstance, prefabPath);
            
            if (newPrefab != null)
            {
                createdPrefabs.Add(newPrefab);
                Debug.Log($"Created trap prefab: {newPrefab.name}");
            }
            
            // Clean up the instance
            DestroyImmediate(trapInstance);
        }
        
        Debug.Log($"Created {createdPrefabs.Count} trap prefabs");
    }
    
    private void SetTrapProperties(TrapBehavior trapBehavior, string trapName)
    {
        // Set properties based on trap type
        if (trapName.Contains("DT_01"))
        {
            // Spike trap
            trapBehavior.damageAmount = 15f;
            trapBehavior.resetTime = 2.0f;
        }
        else if (trapName.Contains("DT_02"))
        {
            // Axe/blade trap
            trapBehavior.damageAmount = 25f;
            trapBehavior.resetTime = 3.0f;
        }
        else if (trapName.Contains("DT_03"))
        {
            // Spinning blade trap
            trapBehavior.damageAmount = 20f;
            trapBehavior.resetTime = 1.5f;
        }
        else if (trapName.Contains("DT_04"))
        {
            // Fire trap
            trapBehavior.damageAmount = 10f;
            trapBehavior.resetTime = 1.0f;
        }
        else if (trapName.Contains("DT_05"))
        {
            // Spike wall
            trapBehavior.damageAmount = 30f;
            trapBehavior.resetTime = 4.0f;
        }
        else if (trapName.Contains("DT_06"))
        {
            // Ceiling crush trap
            trapBehavior.damageAmount = 40f;
            trapBehavior.resetTime = 5.0f;
        }
        else
        {
            // Default values
            trapBehavior.damageAmount = 20f;
            trapBehavior.resetTime = 3.0f;
        }
    }
}
#endif 