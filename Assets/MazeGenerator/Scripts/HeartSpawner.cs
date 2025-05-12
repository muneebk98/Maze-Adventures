using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MazeSpawner))]
public class HeartSpawner : MonoBehaviour
{
    [Header("Heart Settings")]
    public GameObject heartPrefab;              // Your heart prefab
    public int heartsPerLevel = 2;              // Number of hearts to spawn in each level
    public float heightOffset = 0.5f;          // Height above the floor
    public string spawnPointTag = "Floor";      // Tag of objects to spawn hearts on
    
    [Header("Placement")]
    public bool avoidPlayerStart = true;        // Avoid placing hearts near player start
    public float minDistanceBetweenHearts = 5.0f; // Minimum distance between hearts
    public float playerSafeRadius = 5.0f;       // Safe radius around player start
    public float minDistanceFromExit = 5.0f;    // Minimum distance from exit
    
    private MazeSpawner mazeSpawner;
    private List<Vector3> usedPositions = new List<Vector3>();  // Keep track of all used positions
    
    void Start()
    {
        // Get reference to the maze spawner
        mazeSpawner = GetComponent<MazeSpawner>();
    }
    
    public void SpawnHearts()
    {
        if (heartPrefab == null)
        {
            Debug.LogError("Heart prefab not assigned to HeartSpawner!");
            return;
        }
        
        // Clean up any existing hearts
        GameObject[] existingHearts = GameObject.FindGameObjectsWithTag("Heart");
        foreach (GameObject heart in existingHearts)
        {
            Destroy(heart);
        }
        
        // Get orb and trap spawners to avoid their positions
        OrbSpawner orbSpawner = GetComponent<OrbSpawner>();
        TrapSpawner trapSpawner = GetComponent<TrapSpawner>();
        
        // Reset used positions list but include orbs and traps if available
        usedPositions.Clear();
        
        // Add exit position to used positions to avoid spawning near it
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        if (exit != null)
        {
            usedPositions.Add(exit.transform.position);
        }
        
        // Find all potential spawn points by tag
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogError($"No objects with tag '{spawnPointTag}' found for spawning hearts!");
            return;
        }
        
        // Calculate player start position to avoid spawning there
        Vector3 playerStartPos = mazeSpawner.GetPlayerStartPosition();
        
        // Filter out spawn points at player start position or near exit
        List<GameObject> validSpawnPoints = new List<GameObject>();
        foreach (GameObject spawnPoint in spawnPoints)
        {
            Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, 
                                         spawnPoint.transform.position.y + heightOffset, 
                                         spawnPoint.transform.position.z);
                                         
            bool tooCloseToUsedPosition = false;
            foreach (Vector3 usedPos in usedPositions)
            {
                if (Vector3.Distance(spawnPos, usedPos) < minDistanceBetweenHearts)
                {
                    tooCloseToUsedPosition = true;
                    break;
                }
            }
            
            // Check if too close to exit
            bool tooCloseToExit = false;
            if (exit != null)
            {
                if (Vector3.Distance(spawnPos, exit.transform.position) < minDistanceFromExit)
                {
                    tooCloseToExit = true;
                }
            }
            
            // Check if too close to player start
            bool tooCloseToStart = avoidPlayerStart && 
                Vector3.Distance(new Vector3(spawnPos.x, 0, spawnPos.z), 
                                new Vector3(playerStartPos.x, 0, playerStartPos.z)) < playerSafeRadius;
            
            if (!tooCloseToUsedPosition && !tooCloseToStart && !tooCloseToExit)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }
        
        if (validSpawnPoints.Count < heartsPerLevel)
        {
            Debug.LogWarning($"Not enough valid spawn points for hearts! Found {validSpawnPoints.Count}, needed {heartsPerLevel}");
            heartsPerLevel = Mathf.Min(heartsPerLevel, validSpawnPoints.Count);
        }
        
        // Spawn the hearts
        for (int i = 0; i < heartsPerLevel; i++)
        {
            if (validSpawnPoints.Count == 0) break;
            
            // Pick a random spawn point
            int index = Random.Range(0, validSpawnPoints.Count);
            GameObject spawnPoint = validSpawnPoints[index];
            validSpawnPoints.RemoveAt(index);
            
            // Get spawn position
            Vector3 spawnPos = new Vector3(spawnPoint.transform.position.x, 
                                         spawnPoint.transform.position.y + heightOffset, 
                                         spawnPoint.transform.position.z);
            
            // Spawn the heart
            GameObject heart = Instantiate(heartPrefab, spawnPos, Quaternion.identity);
            heart.transform.parent = transform;
            
            // Make sure it has the "Heart" tag
            heart.tag = "Heart";
            
            // Add to used positions
            usedPositions.Add(spawnPos);
            
            Debug.Log($"Spawned heart {i+1}/{heartsPerLevel} at {spawnPos}");
        }
    }
} 