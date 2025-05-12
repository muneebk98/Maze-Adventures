using UnityEngine;
using System.Collections;

public class AurynTrapBehavior : MonoBehaviour
{
    [Header("Trap Settings")]
    public float damageAmount = 20f;              // Amount of damage to deal
    public float resetTime = 3.0f;                // Time until trap can damage player again
    public bool activateOnlyWhenPlayerNear = true; // Only activate when player is near
    public float activationDistance = 3.0f;       // Distance at which the trap activates
    
    [Header("Animation")]
    public Animator trapAnimator;                 // Animator component
    public string openTrigger = "open";           // Animator trigger for open/activate
    public string closeTrigger = "close";         // Animator trigger for close/deactivate
    
    [Header("Effects")]
    public GameObject hitEffect;                  // Optional effect when trap triggers
    public AudioClip hitSound;                    // Optional sound when trap triggers
    
    private bool canDamage = true;                // Can the trap deal damage
    private bool isAnimating = false;             // Is the trap currently animating
    private AudioSource audioSource;              // Audio source component
    private Transform playerTransform;            // Reference to the player
    
    void Start()
    {
        // Get the animator component if not assigned
        if (trapAnimator == null)
        {
            trapAnimator = GetComponent<Animator>();
        }
        
        // Add an audio source if we have a hit sound
        if (hitSound != null && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = hitSound;
            audioSource.playOnAwake = false;
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Find the player object
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        
        // If not activating only when player is near, start the trap cycle
        if (!activateOnlyWhenPlayerNear)
        {
            StartCoroutine(TrapCycle());
        }
    }
    
    void Update()
    {
        // If set to activate when player is near
        if (activateOnlyWhenPlayerNear && playerTransform != null && !isAnimating)
        {
            // Calculate distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            // If player is close enough and trap isn't animating, start the trap
            if (distanceToPlayer < activationDistance)
            {
                StartCoroutine(TrapCycle());
            }
        }
    }
    
    IEnumerator TrapCycle()
    {
        isAnimating = true;
        
        // Activate the trap
        if (trapAnimator != null)
        {
            trapAnimator.SetTrigger(openTrigger);
        }
        
        // Wait a moment before enabling damage (to match animation)
        yield return new WaitForSeconds(0.5f);
        
        // Enable damage
        canDamage = true;
        
        // Wait for animation to complete
        yield return new WaitForSeconds(1.0f);
        
        // Deactivate the trap
        if (trapAnimator != null)
        {
            trapAnimator.SetTrigger(closeTrigger);
        }
        
        // Wait for deactivation animation
        yield return new WaitForSeconds(1.0f);
        
        // Disable damage
        canDamage = false;
        
        // Wait before allowing next cycle
        yield return new WaitForSeconds(resetTime);
        
        isAnimating = false;
        
        // If not player-activated, loop the cycle
        if (!activateOnlyWhenPlayerNear)
        {
            StartCoroutine(TrapCycle());
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Only deal damage if:
        // 1. The collider is the player
        // 2. The trap is in its damaging state
        // 3. We can currently deal damage
        if (other.CompareTag("Player") && canDamage)
        {
            // Find the UI Manager to reduce health
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.TakeDamage(damageAmount);
            }
            
            // Play hit effect if assigned
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            }
            
            // Play hit sound if assigned
            if (audioSource != null && hitSound != null)
            {
                audioSource.Play();
            }
            
            // Don't allow damage again until next cycle
            canDamage = false;
        }
    }
} 