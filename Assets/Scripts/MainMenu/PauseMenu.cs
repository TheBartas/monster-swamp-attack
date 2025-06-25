using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private PlayerShoot playerShoot; // Referencja do PlayerShoot

    private bool isResuming = false;

    private void Start() {
        playerShoot = GetComponent<PlayerShoot>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !pausePanel.activeSelf) {
            Pause();
        }
    }

    public void Pause() {
        pausePanel.SetActive(true);
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Wyłącz możliwość strzelania
        if (playerShoot != null) {
            playerShoot.SetCanShoot(false);
        }
    }

    public void Resume() {
        if (isResuming) return;

        StartCoroutine(ResumeWithDelay());
    }

    public void Home() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void Restart() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    private System.Collections.IEnumerator ResumeWithDelay() {
        isResuming = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pausePanel.SetActive(false);

        yield return null; 
        yield return new WaitForSecondsRealtime(0.1f); 

        Time.timeScale = 1.0f;

        if (playerShoot != null) {
            playerShoot.SetCanShoot(true);
        }

        isResuming = false;
    }
}
