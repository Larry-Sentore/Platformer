using UnityEngine;

public class MainMenuController : MonoBehaviour {

    public GameObject startPanel;
    public GameObject gameOverPanel;

    public void PlayGame() {
        if (GameManager.Instance != null) {
            if (startPanel != null) {
                startPanel.SetActive(false);
            }

            if (gameOverPanel != null) {
                gameOverPanel.SetActive(false);
            }

            GameManager.Instance.StartGame();
        }
    }

    public void ReplayGame() {
        if (GameManager.Instance != null) {
            if (startPanel != null) {
                startPanel.SetActive(false);
            }

            if (gameOverPanel != null) {
                gameOverPanel.SetActive(false);
            }

            GameManager.Instance.ReplayGame();
        }
    }

    public void QuitGame() {
        if (GameManager.Instance != null) {
            GameManager.Instance.QuitGame();
        }
    }
}