using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SplashScreen : MonoBehaviour
{
    [Header("Splash Screen Settings")]
    public float displayTime = 3.0f;           // How long to display the splash screen
    public float fadeInTime = 1.0f;            // Fade in time
    public float fadeOutTime = 1.0f;           // Fade out time
    public string mainMenuSceneName = "MainMenu";
    
    [Header("UI References")]
    public CanvasGroup canvasGroup;            // Canvas group for fading
    public TextMeshProUGUI titleText;          // Title text
    public TextMeshProUGUI studioText;         // Studio text
    
    private void Start()
    {
        // Ensure we have the necessary components
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        
        if (titleText == null || studioText == null)
            Debug.LogWarning("Splash screen is missing title or studio text references!");
        
        // Set texts
        if (titleText != null)
            titleText.text = "Maze Adventures";
        
        if (studioText != null)
            studioText.text = "Kings Gaming Studio";
        
        // Start the splash screen sequence
        StartCoroutine(SplashSequence());
    }
    
    private IEnumerator SplashSequence()
    {
        // Initialize alpha to 0 (fully transparent)
        canvasGroup.alpha = 0;
        
        // Fade in
        float timer = 0;
        while (timer < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        
        // Wait for display time
        yield return new WaitForSeconds(displayTime);
        
        // Fade out
        timer = 0;
        while (timer < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        
        // Load main menu
        SceneManager.LoadScene(mainMenuSceneName);
    }
} 