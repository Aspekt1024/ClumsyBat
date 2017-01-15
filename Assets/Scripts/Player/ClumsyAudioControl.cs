using UnityEngine;
using System.Collections.Generic;

public class ClumsyAudioControl : MonoBehaviour {

    private AudioSource _playerAudio1;
    private AudioSource _playerAudio2;  // Kept for reference and playtesting
    private readonly Dictionary<PlayerSounds, SampleType> _playerAudioDict = new Dictionary<PlayerSounds, SampleType>();

    private struct SampleType
    {
        public float volume;
        public AudioClip audioClip;
    }

    public enum PlayerSounds
    {
        Flap,
        Flap2,
        Collision 
    }
    
    void Start ()
    {
        _playerAudio1 = gameObject.AddComponent<AudioSource>();
        _playerAudio2 = gameObject.AddComponent<AudioSource>();
        SetupAudioDict();
    }

    private void SetupAudioDict()
    {
        AddToAudioDict(PlayerSounds.Flap, "ClumsyFlap", 1f);
        AddToAudioDict(PlayerSounds.Flap2, "Flap", 0.3f);   // Not used but kept for reference and playtesting
        AddToAudioDict(PlayerSounds.Collision, "ClumsyCollision", 1f);
    }

    private void AddToAudioDict(PlayerSounds soundName, string fileName, float volume)
    {
        var newSample = new SampleType
        {
            volume = volume,
            audioClip = Resources.Load<AudioClip>("Audio/" + fileName)
        };
        _playerAudioDict.Add(soundName, newSample);
    }
    
    public void PlaySound(PlayerSounds soundName)
    {
        _playerAudio1.volume = _playerAudioDict[soundName].volume;
        _playerAudio1.PlayOneShot(_playerAudioDict[soundName].audioClip);
    }
}
