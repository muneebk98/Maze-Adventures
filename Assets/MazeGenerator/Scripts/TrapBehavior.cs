using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 20f;       // Amount of damage to deal
    public bool destroyOnTrigger = false;  // Should the trap be removed after triggering?
    public float resetTime = 3.0f;         // Time until trap can damage player again

    [Header("Effects")]
    public GameObject hitEffect;           // Optional effect when trap triggers
    public AudioClip hitSound;             // Optional sound when trap triggers

    private bool canDamage = true;
    private AudioSource audioSource;

    void Start()
    {
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
    }

    void OnTriggerEnter(Collider other)
    {
        // Only trigger if it's the player and we can damage
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
            
            // Destroy if set to do so
            if (destroyOnTrigger)
            {
                Destroy(gameObject);
            }
            else
            {
                // Otherwise start cooldown
                canDamage = false;
                Invoke("ResetDamage", resetTime);
            }
        }
    }
    
    void ResetDamage()
    {
        canDamage = true;
    }
} 