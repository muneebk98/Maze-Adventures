using UnityEngine;

public class CollectibleOrb : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;               // Reference to the Animator component
    public string collectAnimationTrigger = "Collect"; // Name of the collection animation trigger
    
    [Header("Effects")]
    public GameObject collectEffect;        // Optional particle effect prefab
    
    [Header("Movement")]
    public float rotationSpeed = 100f;      // Speed of rotation
    public float hoverHeight = 0.5f;        // How high the orb hovers
    public float hoverSpeed = 1f;           // Speed of hover movement
    
    private Vector3 startPosition;
    private float hoverOffset;
    
    void Start()
    {
        // Get the animator if not assigned in inspector
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // Store the initial position for hover effect
        startPosition = transform.position;
        hoverOffset = Random.Range(0f, 2f * Mathf.PI); // Random start phase for hover
    }
    
    void Update()
    {
        // Rotate the orb
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Hover effect
        float newY = startPosition.y + Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the player collected the orb
        if (other.CompareTag("Player"))
        {
            // Find the UI Manager to increase the score
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.AddScore(1);
            }
            
            // Trigger collection animation if we have an animator
            if (animator != null)
            {
                animator.SetTrigger(collectAnimationTrigger);
                
                // Get the length of the animation
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                float destroyDelay = 0.5f; // Default delay
                
                foreach (AnimationClip clip in clips)
                {
                    if (clip.name.Contains("collect") || clip.name.Contains("Collect"))
                    {
                        destroyDelay = clip.length;
                        break;
                    }
                }
                
                // Destroy after the animation completes
                Destroy(gameObject, destroyDelay);
            }
            else
            {
                // No animator, destroy immediately
                Destroy(gameObject);
            }
            
            // Spawn collection effect if assigned
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
        }
    }
} 