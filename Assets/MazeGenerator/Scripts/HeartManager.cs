using UnityEngine;
using System.Collections.Generic;

public class HeartManager : MonoBehaviour
{
    [Header("Heart Settings")]
    public GameObject heartPrefab;
    public int heartsPerLevel = 2;
    public float heightOffset = 0.5f;
    
    [Header("Placement Rules")]
    public float minDistanceFromPlayer = 5f;
    public float minDistanceFromExit = 5f;
    public float minDistanceBetweenHearts = 3f;
    
    private List<GameObject> spawnedHearts = new List<GameObject>();
    private MazeSpawner mazeSpawner;
    
    private void Awake()
    {
        mazeSpawner = FindObjectOfType<MazeSpawner>();
        if (mazeSpawner == null)
        {
            Debug.LogError("Cannot find MazeSpawner in the scene!");
        }
    }
    
    public void ClearHearts()
    {
        // Destroy any existing hearts
        foreach (GameObject heart in spawnedHearts)
        {
            if (heart != null)
            {
                Destroy(heart);
            }
        }
        
        spawnedHearts.Clear();
        
        // Also clear any other hearts that might exist
        GameObject[] existingHearts = GameObject.FindGameObjectsWithTag("Heart");
        foreach (GameObject heart in existingHearts)
        {
            Destroy(heart);
        }
    }
    
    public void SpawnHeartsForLevel()
    {
        if (heartPrefab == null)
        {
            Debug.LogError("Heart prefab is not assigned to HeartManager!");
            return;
        }
        
        ClearHearts();
        
        // Find all floor tiles as potential spawn points
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("Floor");
        if (floorTiles.Length == 0)
        {
            Debug.LogError("No floor tiles found with tag 'Floor'!");
            return;
        }
        
        // Find player and exit positions
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        
        Vector3 playerPos = player != null ? player.transform.position : Vector3.zero;
        Vector3 exitPos = exit != null ? exit.transform.position : Vector3.zero;
        
        // Shuffle the floor tiles to get random positions
        List<GameObject> shuffledFloors = new List<GameObject>(floorTiles);
        for (int i = 0; i < shuffledFloors.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledFloors.Count);
            GameObject temp = shuffledFloors[i];
            shuffledFloors[i] = shuffledFloors[randomIndex];
            shuffledFloors[randomIndex] = temp;
        }
        
        int heartsSpawned = 0;
        List<Vector3> heartPositions = new List<Vector3>();
        
        // Try to find valid positions for hearts
        foreach (GameObject floor in shuffledFloors)
        {
            if (heartsSpawned >= heartsPerLevel)
                break;
                
            Vector3 potentialPos = floor.transform.position + new Vector3(0, heightOffset, 0);
            
            // Check distance from player
            if (player != null && Vector3.Distance(potentialPos, playerPos) < minDistanceFromPlayer)
                continue;
                
            // Check distance from exit
            if (exit != null && Vector3.Distance(potentialPos, exitPos) < minDistanceFromExit)
                continue;
                
            // Check distance from other hearts
            bool tooCloseToOtherHeart = false;
            foreach (Vector3 existingHeartPos in heartPositions)
            {
                if (Vector3.Distance(potentialPos, existingHeartPos) < minDistanceBetweenHearts)
                {
                    tooCloseToOtherHeart = true;
                    break;
                }
            }
            
            if (tooCloseToOtherHeart)
                continue;
                
            // This is a valid position, spawn a heart here
            GameObject heart = Instantiate(heartPrefab, potentialPos, Quaternion.identity, transform);
            heart.tag = "Heart";
            
            // Make sure the heart has a collider and the HealthHeart script
            if (!heart.GetComponent<Collider>())
            {
                heart.AddComponent<BoxCollider>().isTrigger = true;
            }
            
            if (!heart.GetComponent<HealthHeart>())
            {
                heart.AddComponent<HealthHeart>();
            }
            
            spawnedHearts.Add(heart);
            heartPositions.Add(potentialPos);
            heartsSpawned++;
            
            Debug.Log($"Spawned heart {heartsSpawned}/{heartsPerLevel} at {potentialPos}");
        }
        
        if (heartsSpawned < heartsPerLevel)
        {
            Debug.LogWarning($"Could only spawn {heartsSpawned}/{heartsPerLevel} hearts due to placement constraints");
        }
    }
} 