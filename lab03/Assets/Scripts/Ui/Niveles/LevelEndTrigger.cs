using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    [Header("Next Scene Settings (con Plataforma)")]
    [SerializeField] public string nextSceneName = "Scenes/Inicio";
    [SerializeField] public float delayBeforeLoad = 1.0f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(LoadNextScene());
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        if (nextSceneName != null)
        {
            SceneManager.LoadSceneAsync(nextSceneName);
        }
        else
        {
            SceneManager.LoadSceneAsync(
                SceneManager.GetActiveScene().buildIndex + 1
            );
        }
    }
}
