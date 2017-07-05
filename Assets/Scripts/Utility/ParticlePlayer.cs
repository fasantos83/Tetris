using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour {

	public ParticleSystem[] m_allParticles;


	void Start () {
		m_allParticles = GetComponentsInChildren<ParticleSystem>();	
	}

	public void Play(){
		foreach (ParticleSystem ps in m_allParticles) {
			ps.Stop();
			ps.Play();
		}
	}
}
