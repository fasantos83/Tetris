using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour {

    public bool m_canRotate = true;
    public Vector3 m_queueOffset;
	GameObject[] m_glowSquareFx;
	public string m_glowSquareTag = "LandShapeFx";

	void Start () {
		if (m_glowSquareTag != null && m_glowSquareTag != "") {
			m_glowSquareFx = GameObject.FindGameObjectsWithTag(m_glowSquareTag);
		}
	}

	public void LandShapeFX(){
		int i = 0;
		foreach (Transform child in gameObject.transform) {
			if (m_glowSquareFx[i]) {
				m_glowSquareFx[i].transform.position = child.position - new Vector3(0, 0, 0.1f);

				ParticlePlayer particlePlayer = m_glowSquareFx[i].GetComponent<ParticlePlayer>();
				if (particlePlayer) {
					particlePlayer.Play();
				}
				i++;
			}
		}
	}

    void Move(Vector3 moveDirection) {
        transform.position += moveDirection;
    }
        
    public void MoveLeft() {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight() {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveDown() {
        Move(new Vector3(0, -1, 0));
    }

    public void MoveUp() {
        Move(new Vector3(0, 1, 0));
    }

    public void RotateRight() {
        if (m_canRotate) {
            transform.Rotate(0, 0, -90);
        }
    }

    public void RotateLeft() {
        if (m_canRotate) {
            transform.Rotate(0, 0, 90);
        }
    }

    public void RotateClockwise(bool clockwise) {
        if (clockwise) {
            RotateRight();
        } else {
            RotateLeft();
        }
    }

   
	
	void Update () {
		
	}
}
