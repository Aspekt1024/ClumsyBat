using UnityEngine;
using System.Collections.Generic;

public class ClumsyAudioControl : MonoBehaviour {

    private AudioSource _playerAudio;
    private readonly Dictionary<PlayerSounds, AudioClip> _playerAudioDict = new Dictionary<PlayerSounds, AudioClip>();

    public enum PlayerSounds
    {
        Flap,
        Collision 
    }
    
    void Start ()
    {
        _playerAudio = GetComponent<AudioSource>();
        SetupAudioDict();
    }

    private void SetupAudioDict()
    {
        _playerAudioDict.Add(PlayerSounds.Flap, Resources.Load<AudioClip>("Audio/Flap"));
        _playerAudioDict.Add(PlayerSounds.Collision, Resources.Load<AudioClip>("Audio/ClumsyCollision"));
    }
    
    public void PlaySound(PlayerSounds soundName, float vol)
    {
        _playerAudio.volume = vol;
        _playerAudio.PlayOneShot(_playerAudioDict[soundName]);   // TODO move this to a Player module class thing
    }
}
