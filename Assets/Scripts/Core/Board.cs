﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public Transform m_emptySprite;
	public ParticlePlayer[] m_rowGlowFx = new ParticlePlayer[4];
	public GameObject[] m_gridCells;
	public string m_gridCellTag = "GridCell";

	public int m_height = 30;
	public int m_width = 10;
	public int m_header = 8;
	public int m_completedRows = 0;

	Transform[,] m_grid;

	void Awake() {
		m_grid = new Transform[m_width, m_height];
	}

	void Start() {
		DrawEmptyCells();
		if (m_gridCellTag != null && m_gridCellTag != "") {
			m_gridCells = GameObject.FindGameObjectsWithTag(m_gridCellTag);
		}
		ToggleGrid(true);
		m_grid = new Transform[m_width, m_height];
	}

	void Update() {
		
	}

	bool IsWithinBoard(int x, int y) {
		return (x >= 0 && x < m_width && y >= 0);
	}

	bool IsOccupied(int x, int y, Shape shape) {
		return (m_grid[x, y] != null && m_grid[x, y].parent != shape.transform);
	}

	public bool IsValidPosition(Shape shape) {
		bool isValidPosition = true;

		foreach (Transform child in shape.transform) {
			Vector2 pos = Vectorf.Round(child.position);

			if (!IsWithinBoard((int)pos.x, (int)pos.y) || IsOccupied((int)pos.x, (int)pos.y, shape)) {
				isValidPosition = false;
			}
		}
		return isValidPosition;
	}

	void DrawEmptyCells() {
		if (m_emptySprite) {
			for (int y = 0; y < m_height - m_header; y++) {
				for (int x = 0; x < m_width; x++) {
					Transform clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;
					clone.name = "Board Space (x = " + x.ToString() + ", y = " + y.ToString() + ")";
					clone.transform.parent = transform;
				}
			}
		} else {
			Debug.Log("WARNING! Please assign the emptySprite object!");
		}
	}

	public void StoreShapeInGrid(Shape shape) {
		if (shape == null) {
			return;
		}

		foreach (Transform child in shape.transform) {
			Vector2 pos = Vectorf.Round(child.position);
			m_grid[(int)pos.x, (int)pos.y] = child;
		}
	}

	bool IsComplete(int y) {
		bool isComplete = true;
		for (int x = 0; x < m_width; x++) {
			if (m_grid[x, y] == null) {
				isComplete = false;
			}
		}
		return isComplete;
	}

	void ClearRow(int y) {
		for (int x = 0; x < m_width; x++) {
			if (m_grid[x, y] != null) {
				Destroy(m_grid[x, y].gameObject);
			}
			m_grid[x, y] = null;
		}
	}

	void ShiftOneRowDown(int y) {
		for (int x = 0; x < m_width; x++) {
			if (m_grid[x, y] != null) {
				m_grid[x, y - 1] = m_grid[x, y];
				m_grid[x, y] = null;
				m_grid[x, y - 1].position += new Vector3(0, -1, 0);
			}
		}
	}

	void ShiftRowsDown(int startY) {
		for (int y = startY; y < m_height; y++) {
			ShiftOneRowDown(y);
		}
	}

	public IEnumerator ClearAllRows() {
		m_completedRows = 0;

		for (int y = 0; y < m_height; y++) {
			if (IsComplete(y)) {
				ClearRowFX(m_completedRows++, y);
			}
		}

		yield return new WaitForSeconds(0.3f);

		for (int y = 0; y < m_height; y++) {
			if (IsComplete(y)) {
				ClearRow(y);
				ShiftRowsDown(y + 1);
				yield return new WaitForSeconds(0.2f);
				y--;
			}
		}
	}

	public bool IsOverLimit(Shape shape) {
		bool isOverLimit = false;
		foreach (Transform child in shape.transform) {
			if (child.transform.position.y >= (m_height - m_header - 1)) {
				isOverLimit = true;
			}
		}
		return isOverLimit;
	}

	void ClearRowFX(int idx, int y) {
		if (m_rowGlowFx[idx]) {
			m_rowGlowFx[idx].transform.position = new Vector3(0, y, -1.1f);
			m_rowGlowFx[idx].Play();
		}
	}

	public void ToggleGrid(bool showGrid) {
		float targetAlpha = showGrid && m_emptySprite ? m_emptySprite.gameObject.GetComponent<SpriteRenderer>().color.a : 0f;
		for (int i = 0; i < m_gridCells.Length; i++) {
			Color currentColor = m_gridCells[i].GetComponent<SpriteRenderer>().color;
			Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
			m_gridCells[i].GetComponent<SpriteRenderer>().color = newColor;
		}
	}
}
