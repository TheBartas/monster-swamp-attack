using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMenu : MonoBehaviour
{
    [SerializeField] private GameObject deadPanel;
    public void ShowMenu() {
        Time.timeScale = 0.0f;
        deadPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void Home() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

}
