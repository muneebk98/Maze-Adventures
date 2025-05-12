using UnityEngine;
using Invector.vCharacterController;

public class InvectorMazeAdapter : MonoBehaviour
{
    private MazeSpawner mazeSpawner;
    private LevelManager levelManager;
    private vThirdPersonController controller;
    private Rigidbody rb;
    
    void Start()
    {
        // Get references
        mazeSpawner = FindObjectOfType<MazeSpawner>();
        levelManager = FindObjectOfType<LevelManager>();
        controller = GetComponent<vThirdPersonController>();
        rb = GetComponent<Rigidbody>();
        
        // Set initial position
        ResetPosition();
    }
    
    public void ResetPosition()
    {
        if (mazeSpawner != null)
        {
            // Position at the maze start position (same as original PlayerController)
            transform.position = mazeSpawner.GetPlayerStartPosition();
            
            // Reset velocity when position is reset
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
            
            Debug.Log("Invector character positioned at: " + transform.position);
        }
        else
        {
            Debug.LogError("MazeSpawner not found! Cannot position character correctly.");
        }
    }
    
    // Handle exit collision (similar to the original PlayerController)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit") && levelManager != null)
        {
            levelManager.OnPlayerExit();
        }
    }
    
    // Add this for trap damage
    public void TakeDamage(float amount)
    {
        // Find UI Manager and apply damage
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.TakeDamage(amount);
        }
    }
} 