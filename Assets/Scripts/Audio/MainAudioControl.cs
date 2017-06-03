using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioControl : MonoBehaviour {

    public AudioClip Shield;
    public AudioClip BreakCrystal;
    public AudioClip ClumsyRush;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }
}
