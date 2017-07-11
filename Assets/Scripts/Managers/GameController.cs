using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//using UnityEngine.UI;

public class GameController : MonoBehaviour {

	Board m_gameBoard;
	Spawner m_spawner;
	Shape m_activeShape;
	SoundManager m_soundManager;
	ScoreManager m_scoreManager;
	Ghost m_ghost;
	Holder m_holder;

	public float m_dropInterval = 1f;
	float m_dropIntervalModded;

	float m_timeToDrop = 0.5f;

	float m_timeToNextKeyLeftRight;
	[Range(0.02f, 1f)]
	public float m_keyRepeatRateLeftRight = 0.25f;

	float m_timeToNextKeyDown;
	[Range(0.01f, 1f)]
	public float m_keyRepeatRateDown = 0.04f;

	float m_timeToNextKeyRotate;
	[Range(0.02f, 1f)]
	public float m_keyRepeatRateRotate = 0.2f;

	bool m_gameOver = false;
	public GameObject m_gameOverPanel;

	public IconToggle m_rotateIconToggle;
	bool m_clockwise = true;

	public bool m_isPaused = false;
	public GameObject m_pausePanel;
    
	public ParticlePlayer m_levelUpFx;
	public ParticlePlayer m_gameOverFx;

	//	public Text diagnosticText;

	enum Direction {
		none,
		left,
		right,
		up,
		down
	}

	Direction m_dragDirection = Direction.none;
	Direction m_swipeDirection = Direction.none;

	float m_timeToNextDrag;
	[Range(0.05f, 1f)]
	public float m_minTimeToDrag = 0.15f;

	float m_timeToNextSwipe;
	[Range(0.05f, 1f)]
	public float m_minTimeToSwipe = 0.3f;

	bool m_didTap = false;

	GameObject m_shapeQueue;
	public string m_shapeQueueTag = "ShapeQueue";

	void OnEnable() {
		TouchController.DragEvent += DragHandler;
		TouchController.SwipeEvent += SwipeHandler;
		TouchController.TapEvent += TapHandler;
	}

	void OnDisable() {
		TouchController.DragEvent -= DragHandler;
		TouchController.SwipeEvent -= SwipeHandler;
		TouchController.TapEvent -= TapHandler;
	}

	void Start() {
		m_gameBoard = FindObjectOfType<Board>();
		m_spawner = FindObjectOfType<Spawner>();
		m_soundManager = FindObjectOfType<SoundManager>();
		m_scoreManager = FindObjectOfType<ScoreManager>();
		m_ghost = FindObjectOfType<Ghost>();
		m_holder = FindObjectOfType<Holder>();
		m_shapeQueue = GameObject.FindGameObjectWithTag(m_shapeQueueTag);

		m_timeToNextKeyLeftRight = Time.time;
		m_timeToNextKeyDown = Time.time;
		m_timeToNextKeyRotate = Time.time;
		m_minTimeToDrag = Time.time;
		m_minTimeToSwipe = Time.time;

		if (!m_gameBoard) {
			Debug.Log("WARNING! There is no game board defined!");
		}
		if (!m_soundManager) {
			Debug.Log("WARNING! There is no sound manager defined!");
		}
		if (!m_spawner) {
			Debug.Log("WARNING! There is no spawner defined!");
		} else {
			m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);

			if (m_activeShape == null) {
				m_activeShape = m_spawner.SpawShape();
			}
		}
		if (!m_scoreManager) {
			Debug.Log("WARNING! There is no score manager defined!");
		}

		if (m_gameOverPanel) {
			m_gameOverPanel.SetActive(false);
		}
		if (m_pausePanel) {
			m_pausePanel.SetActive(false);
		}

//		if(diagnosticText){
//			diagnosticText.text = "";
//		}

		m_dropIntervalModded = m_dropInterval;
	}

	void Update() {
		if (!m_gameBoard || !m_spawner || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager) {
			return;
		}

		PlayerInput();
	}

	void LateUpdate() {
		if (m_ghost) {
			m_ghost.DrawGhost(m_activeShape, m_gameBoard);
		}
	}

	void MoveRight() {
		m_activeShape.MoveRight();
		m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
		if (!m_gameBoard.IsValidPosition(m_activeShape)) {
			m_activeShape.MoveLeft();
			PlaySound(m_soundManager.m_errorSound);
		} else {
			PlaySound(m_soundManager.m_moveSound, .5f);
		}
	}

	void MoveLeft() {
		m_activeShape.MoveLeft();
		m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
		if (!m_gameBoard.IsValidPosition(m_activeShape)) {
			m_activeShape.MoveRight();
			PlaySound(m_soundManager.m_errorSound);
		} else {
			PlaySound(m_soundManager.m_moveSound, .5f);
		}
	}

	void Rotate() {
		m_activeShape.RotateClockwise(m_clockwise);
		m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
		if (!m_gameBoard.IsValidPosition(m_activeShape)) {
			m_activeShape.RotateClockwise(!m_clockwise);
			PlaySound(m_soundManager.m_errorSound);
		} else {
			PlaySound(m_soundManager.m_moveSound, .5f);
		}
	}

	void MoveDown() {
		m_timeToDrop = Time.time + m_dropIntervalModded;
		m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
		m_activeShape.MoveDown();
		if (!m_gameBoard.IsValidPosition(m_activeShape)) {
			if (m_gameBoard.IsOverLimit(m_activeShape)) {
				GameOver();
			} else {
				LandShape();
			}
		}
	}

	void PlayerInput() {
		if ((Input.GetButton("MoveRight") && Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveRight")) {
			MoveRight();
		} else if ((Input.GetButton("MoveLeft") && Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft")) {
			MoveLeft();
		} else if (Input.GetButtonDown("Rotate") && Time.time > m_timeToNextKeyRotate) {
			Rotate();
		} else if ((Input.GetButton("MoveDown") && Time.time > m_timeToNextKeyDown) || Time.time > m_timeToDrop) {
			MoveDown();
		} else if ((m_swipeDirection == Direction.right && Time.time > m_timeToNextSwipe) || (m_dragDirection == Direction.right && Time.time > m_timeToNextDrag)) {
			MoveRight();
			m_timeToNextDrag = Time.time + m_minTimeToDrag;
			m_timeToNextSwipe = Time.time + m_timeToNextSwipe;
		} else if ((m_swipeDirection == Direction.left && Time.time > m_timeToNextSwipe) || (m_dragDirection == Direction.left && Time.time > m_timeToNextDrag)) {
			MoveLeft();
			m_timeToNextDrag = Time.time + m_minTimeToDrag;
			m_timeToNextSwipe = Time.time + m_timeToNextSwipe;
		} else if ((m_swipeDirection == Direction.up && Time.time > m_timeToNextSwipe) || m_didTap) {
			Rotate();
			m_timeToNextDrag = Time.time + m_minTimeToDrag;
			m_didTap = false;
		} else if (m_dragDirection == Direction.down && Time.time > m_timeToNextDrag) {
			MoveDown();
		} else if (Input.GetButtonDown("ToggleRotate")) {
			ToggleRotateDirection();
		} else if (Input.GetButtonDown("Pause")) {
			TogglePause();
		} else if (Input.GetButtonDown("Hold")) {
			Hold();
		}

		m_dragDirection = Direction.none;
		m_swipeDirection = Direction.none;
		m_didTap = false;
	}

	private void GameOver() {
		m_activeShape.MoveUp();
		Debug.LogWarning(m_activeShape.name + " is over the limit!");
        
		StartCoroutine(GameOverRoutine());

		PlaySound(m_soundManager.m_gameOverSound, 5f);
		PlaySound(m_soundManager.m_gameOverVocalClip, 5f);

		m_gameOver = true;
	}

	IEnumerator GameOverRoutine() {
		if (m_gameOverFx) {
			m_gameOverFx.Play();
		}

		yield return new WaitForSeconds(0.3f);

		if (m_gameOverPanel) {
			m_gameOverPanel.SetActive(true);
		}
	}

	private void LandShape() {
		m_timeToNextKeyLeftRight = Time.time;
		m_timeToNextKeyDown = Time.time;
		m_timeToNextKeyRotate = Time.time;

		m_activeShape.MoveUp();
		m_gameBoard.StoreShapeInGrid(m_activeShape);

		m_activeShape.LandShapeFX();

		if (m_ghost) {
			m_ghost.Reset();
		}

		if (m_holder) {
			m_holder.m_canRelease = true;
		}
			
		m_activeShape = m_spawner.SpawShape();

		//m_gameBoard.ClearAllRows();
		m_gameBoard.StartCoroutine("ClearAllRows");

		PlaySound(m_soundManager.m_dropSound, 0.75f);

		if (m_gameBoard.m_completedRows > 0) {
			m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

			if (m_scoreManager.m_didLevelUp) {
				PlaySound(m_soundManager.m_levelUpVocalClip);
				m_dropIntervalModded = Mathf.Clamp(m_dropInterval - (((float)m_scoreManager.m_level - 1) * 0.05f), 0.05f, 1f);

				if (m_levelUpFx) {
					m_levelUpFx.Play();
				}
			} else {
				if (m_gameBoard.m_completedRows > 1) {
					PlaySound(m_soundManager.GetRandomClip(m_soundManager.m_vocalClips));
				}
			}
			PlaySound(m_soundManager.m_clearRowSound, 0.75f);
		}
	}

	public void Restart() {
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void PlaySound(AudioClip clip) {
		PlaySound(clip, 1f);
	}

	private void PlaySound(AudioClip clip, float volMultiplier) {
		if (m_soundManager.m_fxEnabled && clip) {
			AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume * volMultiplier, 0.05f, 1f));
		}
	}

	public void ToggleRotateDirection() {
		m_clockwise = !m_clockwise;
		if (m_rotateIconToggle) {
			m_rotateIconToggle.ToggleIcon(m_clockwise);
		}
	}

	public void TogglePause() {
		if (m_gameOver) {
			return;
		}

		m_isPaused = !m_isPaused;
		if (m_pausePanel) {
			m_pausePanel.SetActive(m_isPaused);

			if (m_soundManager) {
				m_soundManager.m_musicSource.volume = m_soundManager.m_musicVolume * ((m_isPaused) ? 0.25f : 1f);
			}

			Time.timeScale = (m_isPaused) ? 0 : 1;
		}
	}

	public void Hold() {
		if (!m_holder) {
			return;
		}

		if (!m_holder.m_heldShape) {
			m_holder.Catch(m_activeShape);
			m_activeShape = m_spawner.SpawShape();
			PlaySound(m_soundManager.m_holdSound);
		} else if (m_holder.m_canRelease) {
			Shape shape = m_activeShape;
			m_activeShape = m_holder.Release();
			m_activeShape.transform.position = m_spawner.transform.position;
			m_holder.Catch(shape);
			PlaySound(m_soundManager.m_holdSound);
		} else {
			Debug.LogWarning("HOLDER WARNING! Wait for cool down!");
			PlaySound(m_soundManager.m_errorSound);
		}

		if (m_ghost) {
			m_ghost.Reset();
		}
	}

	void DragHandler(Vector2 dragMovement) {
		m_dragDirection = GetDirection(dragMovement);
	}

	void SwipeHandler(Vector2 swipeMovement) {
		m_swipeDirection = GetDirection(swipeMovement);
	}

	void TapHandler(Vector2 tapMovement) {
		m_didTap = true;
	}

	Direction GetDirection(Vector2 swipeMovement) {
		Direction direction = Direction.none;

		//Horizontal
		if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y)) {
			direction = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
		}
		//Vertical
		else {
			direction = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
		}

		return direction;
	}

	public void ToggleGridCells(bool showGridCells) {
		Debug.Log("ToggleGridCells: " + showGridCells.ToString());
		if (m_gameBoard) {
			m_gameBoard.ToggleGrid(showGridCells);
		}
	}

	public void ToggleGhost(bool showGhost) {
		Debug.Log("ToggleGhost: " + showGhost.ToString());
		if (m_ghost) {
			m_ghost.ToggleGhost(showGhost);
		}
	}

	public void ToggleShapeQueue(bool showShapeQueue) {
		Debug.Log("ToggleShapeQueue: " + showShapeQueue.ToString());
		if (m_shapeQueue) {
			float targetZ = showShapeQueue ? -1f : -10f;
			Vector3 newPosition = new Vector3(m_shapeQueue.transform.position.x, m_shapeQueue.transform.position.y, targetZ);
			m_shapeQueue.transform.position = newPosition;
			m_spawner.ChangeQueuedShapesPosition(newPosition);
		}
	}

	public void Quit() {
		Application.Quit();
	}

}
