using UnityEngine;

public class LevelIntroController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject introPanel;

    private bool introFinished = false;

    private void Start()
    {
        ShowIntro();
    }

    private void Update()
    {
        if (!introFinished && Input.GetMouseButtonDown(0))
        {
            StartLevel();
        }
    }

    private void ShowIntro()
    {
        introPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void StartLevel()
    {
        introFinished = true;
        introPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
