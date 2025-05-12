using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MazeSpawner))]
public class OrbSpawner : MonoBehaviour
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;            // The orb prefab to spawn
    public int orbCount = 10;               // Maximum orbs to spawn
    public float heightOffset = 0.75f;      // Height above the floor
    public string spawnPointTag = "Floor";  // Tag of objects to spawn orbs on
    
    private MazeSpawner mazeSpawner;
    
    void Start()
    {
        // Get reference to the maze spawner
        mazeSpawner = GetComponent<MazeSpawner>();
    }
    
    // Called by LevelManager when a new level is generated
    public void SpawnOrbs()
    {
        if (orbPrefab == null)
        {
            Debug.LogError("No orb prefab assigned to OrbSpawner!");
            return;
        }
        
        // Clean up any existing orbs
        GameObject[] existingOrbs = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject orb in existingOrbs)
        {
            Destroy(orb);
        }
        
        // Find all potential spawn points by tag
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogError($"No objects with tag '{spawnPointTag}' found for spawning orbs!");
            return;
        }
        
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
        
        // Filter out the spawn point at player start position
        List<GameObject> validSpawnPoints = new List<GameObject>();
        foreach (GameObject spawnPoint in spawnPoints)
        {
            // Check if this spawn point is at player start
            if (Vector3.Distance(new Vector3(spawnPoint.transform.position.x, 0, spawnPoint.transform.position.z), 
                                 playerStartPos) > 1.0f)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }
        
        // Determine how many orbs to spawn (limited by available spawn points)
        int orbsToSpawn = Mathf.Min(orbCount, validSpawnPoints.Count);
        
        // Randomly place orbs on the spawn points
        for (int i = 0; i < orbsToSpawn; i++)
        {
            // Pick a random spawn point
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            GameObject spawnPoint = validSpawnPoints[randomIndex];
            
            // Remove this spawn point so we don't spawn multiple orbs in the same spot
            validSpawnPoints.RemoveAt(randomIndex);
            
            // Get the position and add height offset
            Vector3 spawnPos = spawnPoint.transform.position;
            spawnPos.y += heightOffset;
            
            // Spawn the orb
            GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
            orb.tag = "Collectible";
            orb.transform.parent = transform;
            
            // Ensure the orb has a collider
            if (orb.GetComponent<Collider>() == null)
            {
                SphereCollider collider = orb.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = 0.5f;
            }
            
            // Ensure the orb has the CollectibleOrb script
            if (orb.GetComponent<CollectibleOrb>() == null)
            {
                orb.AddComponent<CollectibleOrb>();
            }
        }
    }
} 