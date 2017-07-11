using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public Shape[] m_allShapes;
	public Transform[] m_queuedXforms = new Transform[3];
	public ParticlePlayer m_spawnFx;
	public string m_activeShapeTag = "ActiveShape";
	public string m_queuedShapeTag = "QueuedShape";

	Shape[] m_queuedShapes = new Shape[3];

	float m_queueScale = 0.5f;

	void Start() {
		InitQueue();
	}

	Shape GetRandomShape() {
		Shape shape = null;
		int i = Random.Range(0, m_allShapes.Length);
		if (m_allShapes[i]) {
			shape = m_allShapes[i];
		} else {
			Debug.Log("SPAWNER WARNING! Invalid shape in spawner!");
		}
		return shape;
	}

	public Shape SpawShape() {
		//return Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
		Shape shape = GetQueuedShape();
		shape.transform.position = transform.position;
		if (m_activeShapeTag != null && m_activeShapeTag != "") {
			shape.gameObject.tag = m_activeShapeTag;
		}
		//shape.transform.localScale = Vector3.one;
		StartCoroutine(GrowShape(shape, transform.position, 0.25f));

		if (m_spawnFx) {
			m_spawnFx.Play();
		}

		return shape;
	}

	void InitQueue() {
		for (int i = 0; i < m_queuedShapes.Length; i++) {
			m_queuedShapes[i] = null;
		}

		FillQueue();
	}

	void FillQueue() {
		for (int i = 0; i < m_queuedShapes.Length; i++) {
			if (!m_queuedShapes[i]) {
				m_queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
				m_queuedShapes[i].transform.position = m_queuedXforms[i].position + m_queuedShapes[i].m_queueOffset;
				m_queuedShapes[i].transform.localScale = new Vector3(m_queueScale, m_queueScale, m_queueScale);
				if (m_queuedShapeTag != null && m_queuedShapeTag != "") {
					m_queuedShapes[i].gameObject.tag = m_queuedShapeTag;
				}
			}
		}
	}

	Shape GetQueuedShape() {
		Shape firstShape = null;

		if (m_queuedShapes[0]) {
			firstShape = m_queuedShapes[0];
		}

		for (int i = 1; i < m_queuedShapes.Length; i++) {
			m_queuedShapes[i - 1] = m_queuedShapes[i];
			m_queuedShapes[i - 1].transform.position = m_queuedXforms[i - 1].position + m_queuedShapes[i].m_queueOffset;
		}

		m_queuedShapes[m_queuedShapes.Length - 1] = null;

		FillQueue();
        
		return firstShape;
	}

	IEnumerator GrowShape(Shape shape, Vector3 position, float growTime = 0.5f) {
		float size = 0f;
		growTime = Mathf.Clamp(growTime, 0.1f, 2f);
		float sizeDelta = Time.deltaTime / growTime;

		while (size < 1f) {
			shape.transform.localScale = new Vector3(size, size, size);
			size += sizeDelta;
			shape.transform.position = position;
			yield return null;
		}
		shape.transform.localScale = Vector3.one;
	}

	public void ChangeQueuedShapesPosition(Vector3 newPosition) {
		GameObject[] queuedShapes = new GameObject[m_queuedXforms.Length];
		if (m_queuedShapeTag != null && m_queuedShapeTag != "") {
			queuedShapes = GameObject.FindGameObjectsWithTag(m_queuedShapeTag);

			for (int i = 0; i < queuedShapes.Length; i++) {
				queuedShapes[i].transform.position = new Vector3(queuedShapes[i].transform.position.x, queuedShapes[i].transform.position.y, newPosition.z);
			}
		}
	}
}
