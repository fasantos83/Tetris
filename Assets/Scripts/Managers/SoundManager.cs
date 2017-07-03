﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public bool m_musicEnabled = true;
    public bool m_fxEnabled = true;

    [Range(0, 1)]
    public float m_musicVolume = 0.5f;

    [Range(0, 1)]
    public float m_fxVolume = 0.5f;

    public AudioClip m_clearRowSound;
    public AudioClip m_moveSound;
    public AudioClip m_errorSound;
    public AudioClip m_dropSound;
    public AudioClip m_holdSound;
    public AudioClip m_gameOverSound;
    
    public AudioSource m_musicSource;

    public AudioClip[] m_musicClips;
    AudioClip m_randomMusicClip;

    public AudioClip[] m_vocalClips;
    public AudioClip m_gameOverVocalClip;
    public AudioClip m_levelUpVocalClip;

    public IconToggle m_musicIconToggle;
    public IconToggle m_fxIconToggle;


    void Start() {
        m_randomMusicClip = GetRandomClip(m_musicClips);
        PlayBackgroundMusic(m_randomMusicClip);
    }

    public AudioClip GetRandomClip(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

    public void PlayBackgroundMusic(AudioClip musicClip) {
        if (!m_musicEnabled || !musicClip || !m_musicSource) {
            return;
        }

        m_musicSource.Stop();
        m_musicSource.clip = musicClip;

        m_musicSource.volume = m_musicVolume;
        m_musicSource.loop = true;
        m_musicSource.Play();
    }

    void UpdateMusic() {
        if (m_musicSource.isPlaying != m_musicEnabled) {
            if (m_musicEnabled) {
                m_randomMusicClip = GetRandomClip(m_musicClips);
                PlayBackgroundMusic(m_randomMusicClip);
            } else {
                m_musicSource.Stop();
            }
        }
    }

    public void ToggleMusic() {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();

        if (m_musicIconToggle) {
            m_musicIconToggle.ToggleIcon(m_musicEnabled);
        }
    }


    public void ToggleFX() {
        m_fxEnabled = !m_fxEnabled;

        if (m_fxIconToggle) {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }
    }
}
