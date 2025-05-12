using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour
{
    Animator anim;
    public float delayBeforeNext = 1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) Play the open animation
        anim.SetTrigger("Open");

        // 2) After a short delay, tell LevelManager to go next
        StartCoroutine(AdvanceAfterDelay());
    }

    IEnumerator AdvanceAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNext);
        FindObjectOfType<LevelManager>().OnPlayerExit();
    }
}
