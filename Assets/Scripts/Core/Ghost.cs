﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	Shape m_ghostShape;
	bool m_hitBottom = false;
	public Color m_color = new Color(1f, 1f, 1f, 0.2f);

	public void DrawGhost(Shape originalShape, Board gameBoard) {
		if (!m_ghostShape) {
			m_ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation) as Shape;
			m_ghostShape.gameObject.name = "GhostShape";

			SetGhostColor();
		} else {
			m_ghostShape.transform.position = originalShape.transform.position;
			m_ghostShape.transform.rotation = originalShape.transform.rotation;
			m_ghostShape.transform.localScale = Vector3.one;
		}

		m_hitBottom = false;

		while (!m_hitBottom) {
			m_ghostShape.MoveDown();
			if (!gameBoard.IsValidPosition(m_ghostShape)) {
				m_ghostShape.MoveUp();
				m_hitBottom = true;
			}
		}
	}

	void SetGhostColor() {
		SpriteRenderer[] allRenderes = m_ghostShape.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer r in allRenderes) {
			r.color = m_color;
		}
	}

	public void ToggleGhost(bool showGhost) {
		float targetAlpha = showGhost ? 0.2f : 0f;
		m_color = new Color(m_color.r, m_color.g, m_color.b, targetAlpha);

		SetGhostColor();
	}

	public void Reset() {
		Destroy(m_ghostShape.gameObject);
	}
}
