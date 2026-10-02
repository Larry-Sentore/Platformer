using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    [Header("Player Life")]
    public int maxLives = 3;
    public int currentLives;
    public Text lifeText;

    [Header("Respawn")]
    public Transform respawnPoint;
    private static readonly Vector3 DefaultRespawnPosition = new Vector3(-1.18f, -3.652f, 0f);
    private Transform player;

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

        SceneManager.sceneLoaded += HandleSceneLoaded;
        currentLives = maxLives;
        RefreshRefs();
        UpdateLifeText();
    }

    private void OnDestroy() {
        if (Instance == this) {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            Instance = null;
        }
    }

    private void Start() {
        RefreshRefs();
        ShowStartMenu();
    }

    private void RefreshRefs() {
        if (respawnPoint == null) {
            respawnPoint = GameObject.Find("Respawn point")?.transform;
        }

        if (lifeText == null) {
            lifeText = GameObject.Find("LifeText")?.GetComponent<Text>();
        }

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (startMenuPanel == null) {
            startMenuPanel = FindSceneObject("StartPanel", "Start Menu", "MainMenuPanel");
        }

        if (gameOverPanel == null) {
            gameOverPanel = FindSceneObject("GameOverPanel", "GameOver", "Game Over", "EndPanel", "End");
        }

        if (cameraFollow == null) {
            Camera mainCamera = Camera.main;
            if (mainCamera != null) {
                cameraFollow = mainCamera.GetComponent<CameraFollow>();
            }
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) {
        player = null;
        respawnPoint = null;
        lifeText = null;
        startMenuPanel = null;
        gameOverPanel = null;
        cameraFollow = null;
        currentLives = maxLives;

        RefreshRefs();
        UpdateLifeText();

        if (startMenuPanel != null) {
            startMenuPanel.SetActive(false);
        }

        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private GameObject FindSceneObject(params string[] possibleNames) {
        foreach (string objectName in possibleNames) {
            GameObject found = GameObject.Find(objectName);
            if (found != null) {
                return found;
            }
        }

        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>()) {
            if (obj == null || !obj.scene.IsValid() || obj.scene != SceneManager.GetActiveScene()) {
                continue;
            }

            foreach (string objectName in possibleNames) {
                if (obj.name == objectName) {
                    return obj;
                }
            }
        }

        return null;
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
            Vector3 spawnPosition = GetRespawnPosition();
            player.gameObject.SetActive(true);
            player.position = spawnPosition;
            ResetCameraToPosition(spawnPosition);
        }
    }

    public void RegisterPlayer(Transform playerTransform) {
        player = playerTransform;
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
            gameOverPanel = FindSceneObject("GameOverPanel", "GameOver", "Game Over", "EndPanel", "End");
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

        Vector3 spawnPosition = GetRespawnPosition();
        player.gameObject.SetActive(true);
        Time.timeScale = 1f;
        player.position = spawnPosition;
        ResetCameraToPosition(spawnPosition);
    }

    public void ResetCameraToRespawn() {
        ResetCameraToPosition(GetRespawnPosition());
    }

    private Vector3 GetRespawnPosition() {
        RefreshRefs();
        Vector3 position = respawnPoint != null ? respawnPoint.position : DefaultRespawnPosition;
        position.y = 5f;
        return position;
    }

    private void ResetCameraToPosition(Vector3 position) {
        if (cameraFollow == null) {
            Camera mainCamera = Camera.main;
            if (mainCamera != null) {
                cameraFollow = mainCamera.GetComponent<CameraFollow>();
            }
        }

        if (cameraFollow != null) {
            cameraFollow.ResetToPosition(position);
        }
    }

    public void ReplayGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
