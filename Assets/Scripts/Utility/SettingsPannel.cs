using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPannel : MonoBehaviour {

	GameController m_gameController;
	TouchController m_touchController;

	public Slider m_swipeDistanceSlider;
	public Slider m_dragDistanceSlider;
	public Slider m_dragSpeedSlider;
	public Toggle m_toggleDiagnostic;
	public Toggle m_toggleShapeQueue;
	public Toggle m_toggleGhost;
	public Toggle m_toggleGrid;

	void Start() {
		m_gameController = FindObjectOfType<GameController>();
		m_touchController = FindObjectOfType<TouchController>();

		if (m_swipeDistanceSlider != null) {
			m_swipeDistanceSlider.minValue = 20;
			m_swipeDistanceSlider.maxValue = 200;
			m_swipeDistanceSlider.value = 50;
		}

		if (m_dragDistanceSlider != null) {
			m_dragDistanceSlider.minValue = 50;
			m_dragDistanceSlider.maxValue = 150;
			m_dragDistanceSlider.value = 100;
		}

		if (m_dragSpeedSlider != null) {
			m_dragSpeedSlider.minValue = 0.05f;
			m_dragSpeedSlider.maxValue = 0.5f;
			m_dragSpeedSlider.value = 0.15f;
		}

		if (m_toggleDiagnostic != null && m_touchController != null) {
			m_touchController.m_useDiagnostic = m_toggleDiagnostic.isOn;
		}

		if (m_toggleShapeQueue != null) {
			m_gameController.ToggleShapeQueue(m_toggleShapeQueue.isOn);
		}

		if (m_toggleGhost != null) {
			m_gameController.ToggleGhost(m_toggleGhost.isOn);
		}

		if (m_toggleGrid != null) {
			m_gameController.ToggleGridCells(m_toggleGrid.isOn);
		}
		UpdatePanel();
	}

	public void UpdatePanel() {
		if (m_touchController != null) {
			if (m_swipeDistanceSlider != null) {
				m_touchController.m_minSwipeDistance = (int)m_swipeDistanceSlider.value;
			}

			if (m_dragDistanceSlider != null) {
				m_touchController.m_minDragDistance = (int)m_dragDistanceSlider.value;
			}

			if (m_toggleDiagnostic != null) {
				m_touchController.m_useDiagnostic = m_toggleDiagnostic.isOn;
			}
		}

		if (m_gameController != null) {
			if (m_dragSpeedSlider != null) {
				m_gameController.m_minTimeToDrag = m_dragSpeedSlider.value;
			}

			if (m_toggleShapeQueue != null) {
				m_gameController.ToggleShapeQueue(m_toggleShapeQueue.isOn);
			}

			if (m_toggleGhost != null) {
				m_gameController.ToggleGhost(m_toggleGhost.isOn);
			}

			if (m_toggleGrid != null) {
				m_gameController.ToggleGridCells(m_toggleGrid.isOn);
			}
		}
	}
}
