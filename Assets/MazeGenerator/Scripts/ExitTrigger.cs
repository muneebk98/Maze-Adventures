using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            FindObjectOfType<LevelManager>().OnPlayerExit();
        }
    }
}
