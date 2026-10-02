using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour {

	private Text lifeText;
	private int lifeScoreCount;

	private bool canDamage;

	void Awake () {
		lifeText = GameObject.Find ("LifeText").GetComponent<Text> ();
		lifeScoreCount = 3;
		lifeText.text = "x" + lifeScoreCount;

		if (GameManager.Instance != null) {
			GameManager.Instance.RegisterPlayer(transform);
			GameManager.Instance.lifeText = lifeText;
			GameManager.Instance.currentLives = lifeScoreCount;
			GameManager.Instance.UpdateLifeText();
		}

		canDamage = true;
	}

	void Start() {
		Time.timeScale = 1f;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Water")) {
			if (GameManager.Instance != null) {
				GameManager.Instance.HandleWaterDeath();
			}
		}
	}
	
	public void DealDamage() {
		if (canDamage) {
			TakeLife();
		}
	}

	private void TakeLife() {
		if (!canDamage) {
			return;
		}

		canDamage = false;

		if (GameManager.Instance != null) {
			GameManager.Instance.HandlePlayerHit();
			lifeScoreCount = GameManager.Instance.currentLives;
			if (lifeText != null) {
				lifeText.text = "x" + lifeScoreCount;
			}
		} else {
			lifeScoreCount--;
			if (lifeScoreCount >= 0) {
				lifeText.text = "x" + lifeScoreCount;
			}

			if (lifeScoreCount == 0) {
				// RESTART THE GAME
				Time.timeScale = 0f;
				StartCoroutine(RestartGame());
			}
		}

		StartCoroutine (WaitForDamage ());
	}

	IEnumerator WaitForDamage() {
		yield return new WaitForSeconds (2f);
		canDamage = true;
	}

	IEnumerator RestartGame() {
		yield return new WaitForSecondsRealtime(2f);
		SceneManager.LoadScene ("GameScene-ALU");
	}

} // class









































