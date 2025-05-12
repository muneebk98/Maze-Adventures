using UnityEngine;
using System.Collections.Generic;
using Invector.vCharacterController;

[RequireComponent(typeof(MazeSpawner))]
public class TrapSpawner : MonoBehaviour
{
    [Header("Trap Settings")]
    public GameObject[] trapPrefabs;    // Array of different trap prefabs from AurynSky
    public int baseTrapCount = 5;       // Base number of traps for level 1
    public int trapsPerLevelIncrease = 2; // How many additional traps to add per level
    public float heightOffset = 0.1f;   // Height above the floor
    public string spawnPointTag = "Floor"; // Tag of objects to spawn traps on
    
    // Debug mode flag
    public bool debugMode = true;       // Enable/disable debug logging
    
    // Trap counting
    private int totalTrapsSpawned = 0;  // Total traps spawned in current session
    private int currentLevelTraps = 0;  // Traps spawned in current level
    
    // Public property to access trap count
    public int TotalTrapsSpawned => totalTrapsSpawned;
    public int CurrentLevelTraps => currentLevelTraps;
    
    // This property calculates trap count based on current level
    private int trapCount {
        get {
            int level = 1;
            if (levelManager != null) {
                level = levelManager.GetCurrentLevel() + 1; // +1 because level index is zero-based
            }
            if (debugMode) Debug.Log($"Calculating traps for level {level}: {baseTrapCount + (level - 1) * trapsPerLevelIncrease}");
            return baseTrapCount + (level - 1) * trapsPerLevelIncrease;
        }
    }
    
    [Header("Trap Type Weights")]
    [Range(0, 100)]
    public int spikeWeight = 30;        // Weight for spike traps
    [Range(0, 100)]
    public int guillotineWeight = 45;   // Weight for guillotine traps (replaced blades)
    [Range(0, 100)]
    public int swingTrapWeight = 25;    // Weight for swing traps
    
    [Header("Obstacle Pack References")]
    public GameObject guillotinePrefab;  // Reference to the guillotine from Obstacle Pack
    
    [Header("Placement")]
    public bool avoidPlayerStart = true;  // Avoid placing traps near player start
    public float minDistanceBetweenTraps = 2.0f; // Minimum distance between traps
    public float playerSafeRadius = 5.0f; // Safe radius around player start
    
    [Header("Materials")]
    public bool fixMaterials = true;      // Whether to fix missing materials
    
    private MazeSpawner mazeSpawner;
    private LevelManager levelManager;
    private TrapMaterialFixer materialFixer;
    private List<Vector3> usedPositions = new List<Vector3>(); // Keep track of used positions
    
    // References to the different types of traps
    private List<GameObject> spikeTraps = new List<GameObject>();
    private List<GameObject> guillotineTraps = new List<GameObject>();
    private List<GameObject> swingTraps = new List<GameObject>();
    
    void Awake()
    {
        // Immediate initialization for quicker response
        // Get reference to the maze spawner
        mazeSpawner = GetComponent<MazeSpawner>();
        
        // Get reference to the level manager
        levelManager = FindObjectOfType<LevelManager>();
        
        // Categorize trap prefabs
        CategorizeTrapPrefabs();
    }
    
    void Start()
    {
        // Get reference to the maze spawner
        mazeSpawner = GetComponent<MazeSpawner>();
        
        // Get reference to the level manager
        levelManager = FindObjectOfType<LevelManager>();
        
        // Get or add the material fixer
        materialFixer = GetComponent<TrapMaterialFixer>();
        if (materialFixer == null && fixMaterials)
        {
            materialFixer = gameObject.AddComponent<TrapMaterialFixer>();
        }
        
        // Add guillotine to the trap list if not null
        if (guillotinePrefab != null)
        {
            guillotineTraps.Add(guillotinePrefab);
        }
        else
        {
            Debug.LogWarning("Guillotine prefab not assigned! Please assign it in the inspector.");
        }
        
        // Categorize trap prefabs
        CategorizeTrapPrefabs();
    }
    
    private void CategorizeTrapPrefabs()
    {
        if (trapPrefabs == null || trapPrefabs.Length == 0)
        {
            if (debugMode) Debug.LogWarning("No trap prefabs assigned to TrapSpawner!");
            return;
        }
            
        spikeTraps.Clear();
        guillotineTraps.Clear();
        swingTraps.Clear();
        
        if (debugMode) Debug.Log("Categorizing trap prefabs...");
        
        foreach (GameObject trap in trapPrefabs)
        {
            if (trap == null)
            {
                if (debugMode) Debug.LogWarning("Null trap prefab found in trapPrefabs array!");
                continue;
            }
            
            string trapName = trap.name.ToLower();
            
            if (trapName.Contains("spike"))
            {
                spikeTraps.Add(trap);
                if (debugMode) Debug.Log($"Added {trap.name} to spike traps");
            }
            else if (trapName.Contains("gyotine") || trapName.Contains("guillotine"))
            {
                guillotineTraps.Add(trap);
                if (debugMode) Debug.Log($"Added {trap.name} to guillotine traps");
            }
            else if (trapName.Contains("swing"))
            {
                swingTraps.Add(trap);
                if (debugMode) Debug.Log($"Added {trap.name} to swing traps");
            }
            else
            {
                guillotineTraps.Add(trap);
                if (debugMode) Debug.Log($"Added {trap.name} to guillotine traps (default category)");
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Categorization complete: {spikeTraps.Count} spike traps, " +
                     $"{guillotineTraps.Count} guillotine traps, {swingTraps.Count} swing traps");
        }
    }
    
    // Get a random trap based on weights
    private GameObject GetRandomTrap()
    {
        // Calculate total weight
        int totalWeight = spikeWeight + guillotineWeight + swingTrapWeight;
        
        // Get a random value between 0 and total weight
        int randomValue = Random.Range(0, totalWeight);
        
        // Determine which trap type to use based on weights
        if (randomValue < spikeWeight)
        {
            return GetRandomTrapFromCategory(spikeTraps);
        }
        else if (randomValue < spikeWeight + guillotineWeight)
        {
            return GetRandomTrapFromCategory(guillotineTraps);
        }
        else
        {
            return GetRandomTrapFromCategory(swingTraps);
        }
    }
    
    // Get a random trap from a specific category
    private GameObject GetRandomTrapFromCategory(List<GameObject> category)
    {
        if (category.Count == 0)
        {
            // If this category is empty, use any available trap
            if (trapPrefabs.Length > 0)
                return trapPrefabs[Random.Range(0, trapPrefabs.Length)];
            return null;
        }
        
        return category[Random.Range(0, category.Count)];
    }
    
    // Called by LevelManager when a new level is generated
    public void SpawnTraps()
    {
        if (debugMode) Debug.Log("Starting trap spawning process...");
        
        // Reset current level trap count
        currentLevelTraps = 0;
        
        if (trapPrefabs == null || trapPrefabs.Length == 0)
        {
            Debug.LogError("No trap prefabs assigned to TrapSpawner!");
            return;
        }
        
        // Clean up any existing traps
        GameObject[] existingTraps = GameObject.FindGameObjectsWithTag("Trap");
        if (debugMode) Debug.Log($"Cleaning up {existingTraps.Length} existing traps");
        foreach (GameObject trap in existingTraps)
        {
            Destroy(trap);
        }
        
        // Reset used positions
        usedPositions.Clear();
        
        // Find all potential spawn points by tag
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogError($"No objects with tag '{spawnPointTag}' found for spawning traps!");
            return;
        }
        
        if (debugMode) Debug.Log($"Found {spawnPoints.Length} potential spawn points");
        
        // Get maze data
        int rows = mazeSpawner.Rows;
        int cols = mazeSpawner.Columns;
        float cellWidth = mazeSpawner.CellWidth;
        float cellHeight = mazeSpawner.CellHeight;
        
        // Calculate player start position to avoid spawning there
        Vector3 playerStartPos = new Vector3(
            (cols - 1) * (cellWidth + (mazeSpawner.AddGaps ? 0.2f : 0f)),
            0,
            (rows - 1) * (cellHeight + (mazeSpawner.AddGaps ? 0.2f : 0f))
        );
        
        // Filter out spawn points at player start position
        List<GameObject> validSpawnPoints = new List<GameObject>();
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (!avoidPlayerStart || Vector3.Distance(
                new Vector3(spawnPoint.transform.position.x, 0, spawnPoint.transform.position.z), 
                playerStartPos) > playerSafeRadius)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }
        
        if (validSpawnPoints.Count == 0)
        {
            Debug.LogError("No valid spawn points found for traps after filtering!");
            return;
        }
        
        // Get the dynamic trap count based on current level
        int currentTrapCount = trapCount;
        Debug.Log($"Spawning {currentTrapCount} traps for level {(levelManager != null ? levelManager.GetCurrentLevel() + 1 : 1)}");
        
        // Determine how many traps to spawn (limited by available spawn points)
        int trapsToSpawn = Mathf.Min(currentTrapCount, validSpawnPoints.Count);
        
        // Track the spawned traps count
        int spawnedTraps = 0;
        int attempts = 0;
        int maxAttempts = currentTrapCount * 10; // Limit attempts to prevent infinite loops
        
        while (spawnedTraps < trapsToSpawn && attempts < maxAttempts)
        {
            attempts++;
            
            // Pick a random spawn point
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            GameObject spawnPoint = validSpawnPoints[randomIndex];
            
            // Get the position and add height offset
            Vector3 spawnPos = spawnPoint.transform.position;
            spawnPos.y += heightOffset;
            
            // Check if this position is too close to any other used position
            bool tooClose = false;
            foreach (Vector3 usedPos in usedPositions)
            {
                if (Vector3.Distance(spawnPos, usedPos) < minDistanceBetweenTraps)
                {
                    tooClose = true;
                    break;
                }
            }
            
            // If too close, try again
            if (tooClose)
            {
                continue;
            }
            
            // Get a random trap prefab based on weights
            GameObject trapPrefab = GetRandomTrap();
            if (trapPrefab == null)
            {
                Debug.LogError("Failed to select a trap prefab!");
                continue;
            }
            
            // Spawn the trap with random rotation
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
            GameObject trap = Instantiate(trapPrefab, spawnPos, randomRotation);
            trap.tag = "Trap";
            trap.transform.parent = transform;
            
            // Fix materials if needed
            if (fixMaterials && materialFixer != null)
            {
                materialFixer.FixTrapMaterials(trap);
            }
            
            // Set up the AurynTrapBehavior
            AurynTrapBehavior trapBehavior = trap.GetComponent<AurynTrapBehavior>();
            if (trapBehavior == null)
            {
                trapBehavior = trap.AddComponent<AurynTrapBehavior>();
            }
            
            // Set trap settings based on trap type
            ConfigureTrapBehavior(trapBehavior, trap.name);
            
            // Ensure the trap has a trigger collider
            EnsureTrapHasCollider(trap);
            
            // Record this position
            usedPositions.Add(spawnPos);
            spawnedTraps++;
            
            // Increment counters
            totalTrapsSpawned++;
            currentLevelTraps++;
            
            if (debugMode)
            {
                Debug.Log($"Trap spawned! Total: {totalTrapsSpawned}, Current Level: {currentLevelTraps}");
            }
        }
        
        if (spawnedTraps < currentTrapCount)
        {
            Debug.LogWarning($"Could only spawn {spawnedTraps} out of {currentTrapCount} traps due to space constraints");
        }
        
        if (debugMode)
        {
            Debug.Log($"Level complete - Traps spawned this level: {currentLevelTraps}");
            Debug.Log($"Total traps spawned in session: {totalTrapsSpawned}");
        }
    }
    
    // Configure trap behavior based on trap type
    private void ConfigureTrapBehavior(AurynTrapBehavior behavior, string trapName)
    {
        trapName = trapName.ToLower();
        
        if (trapName.Contains("spike"))
        {
            behavior.damageAmount = 15f;
            behavior.resetTime = 2.0f;
            behavior.activateOnlyWhenPlayerNear = true;
            behavior.activationDistance = 2.0f;
        }
        else if (trapName.Contains("gyotine") || trapName.Contains("guillotine"))
        {
            behavior.damageAmount = 30f;
            behavior.resetTime = 2.0f;
            behavior.activateOnlyWhenPlayerNear = false; // Guillotine moves continuously
            
            // Ensure the guillotine has an animator
            Animator animator = behavior.gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                animator = behavior.gameObject.AddComponent<Animator>();
                
                // Find the animator controller in the project
                string controllerPath = "Assets/Obstacle Pack/Animations/Controllers/obstacle gyotine.controller";
                RuntimeAnimatorController controller = null;
                
                #if UNITY_EDITOR
                // Use AssetDatabase in editor
                controller = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
                #else
                // In builds, we should have assigned the controller in the editor already
                #endif
                
                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
                else
                {
                    Debug.LogWarning("Could not find guillotine animator controller at: " + controllerPath);
                }
            }
            
            behavior.trapAnimator = animator;
        }
        else if (trapName.Contains("swing"))
        {
            behavior.damageAmount = 30f;
            behavior.resetTime = 3.0f;
            behavior.activateOnlyWhenPlayerNear = false; // Swing traps move continuously
        }
        else
        {
            // Default values
            behavior.damageAmount = 20f;
            behavior.resetTime = 2.0f;
        }
        
        // Set the animator reference
        if (behavior.trapAnimator == null)
        {
            behavior.trapAnimator = behavior.GetComponent<Animator>();
        }
    }
    
    // Make sure the trap has a collider for triggering damage
    private void EnsureTrapHasCollider(GameObject trap)
    {
        // Check if the trap already has a collider
        Collider[] colliders = trap.GetComponentsInChildren<Collider>();
        bool hasTriggerCollider = false;
        
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger)
            {
                hasTriggerCollider = true;
                break;
            }
        }
        
        // If no trigger collider found, add one
        if (!hasTriggerCollider)
        {
            // Add a box collider to the main trap object
            BoxCollider triggerCollider = trap.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            
            // Size the collider based on the trap type
            string trapName = trap.name.ToLower();
            
            if (trapName.Contains("spike"))
            {
                triggerCollider.size = new Vector3(1.0f, 0.5f, 1.0f);
                triggerCollider.center = new Vector3(0, 0.25f, 0);
            }
            else if (trapName.Contains("swing"))
            {
                triggerCollider.size = new Vector3(2.0f, 1.0f, 0.5f);
                triggerCollider.center = new Vector3(0, 0.5f, 0);
            }
            else if (trapName.Contains("gyotine") || trapName.Contains("guillotine"))
            {
                triggerCollider.size = new Vector3(1.5f, 2.0f, 1.0f);
                triggerCollider.center = new Vector3(0, 1.0f, 0);
            }
            else
            {
                // Default size
                triggerCollider.size = new Vector3(1.0f, 0.5f, 1.0f);
                triggerCollider.center = new Vector3(0, 0.25f, 0);
            }
        }
    }

    // Add a method to reset the total counter if needed
    public void ResetTotalTraps()
    {
        totalTrapsSpawned = 0;
        if (debugMode) Debug.Log("Total traps counter reset to 0");
    }
} 