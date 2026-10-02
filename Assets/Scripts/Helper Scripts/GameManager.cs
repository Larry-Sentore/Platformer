using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Header("Player Life")]
    public int maxLives = 3;
    public int currentLives;
    public Text lifeText;

    [Header("Respawn")]
    public Transform player;
    public Transform respawnPoint;
    public Vector3 respawnPosition = new Vector3(-1.18f, -3.652f, 0f);

    [Header("UI Panels")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;

    private CameraFollow cameraFollow;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        currentLives = maxLives;
        RefreshRefs();
        UpdateLifeText();
    }

    private void Start() {
        RefreshRefs();
        ShowStartMenu();
    }

    private void RefreshRefs() {
        if (lifeText == null) {
            lifeText = GameObject.Find("LifeText")?.GetComponent<Text>();
        }

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (startMenuPanel == null) {
            startMenuPanel = GameObject.Find("StartPanel") ?? GameObject.Find("Start Menu") ?? GameObject.Find("MainMenuPanel");
        }

        if (gameOverPanel == null) {
            gameOverPanel = GameObject.Find("GameOverPanel") ?? GameObject.Find("GameOver") ?? GameObject.Find("Game Over") ?? GameObject.Find("EndPanel");
        }

        if (respawnPoint == null) {
            respawnPoint = GameObject.FindGameObjectWithTag("Respawn")?.transform;
        }

        if (respawnPoint != null) {
            respawnPosition = respawnPoint.position;
        } else {
            respawnPosition = new Vector3(-1.18f, -3.652f, 0f);
        }

        if (cameraFollow == null) {
            Camera mainCamera = Camera.main;
            if (mainCamera != null) {
                cameraFollow = mainCamera.GetComponent<CameraFollow>();
            }
        }
    }

    public void ShowStartMenu() {
        if (startMenuPanel != null) {
            startMenuPanel.SetActive(true);
        }

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    public void StartGame() {
        currentLives = maxLives;
        UpdateLifeText();

        if (startMenuPanel != null) {
            startMenuPanel.SetActive(false);
        }

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (player != null) {
            player.gameObject.SetActive(true);
            player.position = respawnPosition;
            ResetCameraToRespawn();
        }
    }

    public void RegisterPlayer(Transform playerTransform) {
        player = playerTransform;
        if (player != null) {
            respawnPosition = new Vector3(-1.18f, -3.652f, 0f);
        }
    }

    public void UpdateLifeText() {
        if (lifeText == null) {
            lifeText = GameObject.Find("LifeText")?.GetComponent<Text>();
        }

        if (lifeText != null) {
            lifeText.text = "x" + currentLives;
        }
    }

    public void HandlePlayerHit() {
        if (currentLives <= 0) {
            return;
        }

        currentLives--;
        UpdateLifeText();

        if (currentLives <= 0) {
            ShowGameOverPanel();
            return;
        }
    }

    public void HandleWaterDeath() {
        if (currentLives <= 0) {
            return;
        }

        currentLives--;
        UpdateLifeText();

        if (currentLives > 0) {
            RespawnPlayer();
            return;
        }

        ShowGameOverPanel();
    }

    private void ShowGameOverPanel() {
        if (gameOverPanel == null) {
            gameOverPanel = GameObject.Find("GameOverPanel") ?? GameObject.Find("GameOver") ?? GameObject.Find("Game Over") ?? GameObject.Find("EndPanel");
        }

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RespawnPlayer() {
        if (player == null) {
            return;
        }

        player.gameObject.SetActive(true);
        Time.timeScale = 1f;
        player.position = respawnPosition;
        ResetCameraToRespawn();
    }

    public void ResetCameraToRespawn() {
        if (cameraFollow == null) {
            Camera mainCamera = Camera.main;
            if (mainCamera != null) {
                cameraFollow = mainCamera.GetComponent<CameraFollow>();
            }
        }

        if (cameraFollow != null) {
            cameraFollow.ResetToPosition(respawnPosition);
        }
    }

    public void ReplayGame() {
        StartGame();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
