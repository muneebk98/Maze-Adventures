using UnityEngine;

public class HealthHeart : MonoBehaviour
{
    [Header("Healing")]
    public float healAmount = 100f; // Full health restoration by default
    
    [Header("Visual Effects")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 1f;
    public float bobHeight = 0.3f;
    public GameObject collectEffectPrefab;
    public AudioClip collectSound;
    
    private Vector3 startPosition;
    private float bobTime;
    private AudioSource audioSource;
    
    void Start()
    {
        // Remember starting position for bob effect
        startPosition = transform.position;
        
        // Randomize bob cycle position
        bobTime = Random.Range(0f, 2f * Mathf.PI);
        
        // Add audio source if needed
        if (collectSound != null && !TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = collectSound;
        }
    }
    
    void Update()
    {
        // Rotate the heart
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        bobTime += bobSpeed * Time.deltaTime;
        float yOffset = Mathf.Sin(bobTime) * bobHeight;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Only react to player collisions
        if (other.CompareTag("Player"))
        {
            // Find UI Manager to heal the player
            UIManager uiManager = FindObjectOfType<UIManager>();
            
            if (uiManager != null)
            {
                // Only heal if player isn't already at full health
                if (uiManager.currentHealth < uiManager.maxHealth)
                {
                    uiManager.HealHealth(healAmount);
                    Debug.Log($"Player healed for {healAmount} health!");
                    
                    // Play collect sound
                    if (audioSource != null && collectSound != null)
                    {
                        // Detach audio source so it can finish playing after heart is destroyed
                        audioSource.transform.SetParent(null);
                        audioSource.Play();
                        Destroy(audioSource.gameObject, collectSound.length + 0.1f);
                    }
                    
                    // Spawn collect effect if assigned
                    if (collectEffectPrefab != null)
                    {
                        GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
                        Destroy(effect, 2f); // Destroy effect after 2 seconds
                    }
                    
                    // Destroy the heart
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Player already at full health, heart not collected.");
                }
            }
            else
            {
                Debug.LogError("UIManager not found in scene!");
            }
        }
    }
} 