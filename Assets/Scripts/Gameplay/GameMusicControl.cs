using UnityEngine;
using System.Collections.Generic;

public class GameMusicControl : MonoBehaviour
{
    // TODO make an Audio Control base class
    private AudioSource _musicSource;
    private readonly Dictionary<GameTrack, SampleType> _musicDict = new Dictionary<GameTrack, SampleType>();
    
    public enum GameTrack
    {
        Boss,
        Cave,
        Village
    }

    private struct SampleType
    {
        public float Volume;
        public AudioClip AudioClip;
    }

    private void Start ()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        SetupAudioDict();
    }

    private void SetupAudioDict()
    {
        AddToAudioDict(GameTrack.Boss, "doom", 0.5f);
        AddToAudioDict(GameTrack.Cave, "fields", 0.5f);
        //AddToAudioDict(GameTrack.Cave, "mysterious scene", 0.5f);
        AddToAudioDict(GameTrack.Village, "village theme", 0.5f);
    }

    private void AddToAudioDict(GameTrack soundName, string fileName, float volume)
    {
        var newSample = new SampleType
        {
            Volume = volume,
            AudioClip = Resources.Load<AudioClip>("Audio/Test/" + fileName)
        };
        _musicDict.Add(soundName, newSample);
    }

    public void PlaySound(GameTrack soundName)
    {
        _musicSource.volume = 0;
        _musicSource.Stop();   // TODO remove the popping sound.
        _musicSource.clip = _musicDict[soundName].AudioClip;
        _musicSource.volume = _musicDict[soundName].Volume;
        _musicSource.loop = true;
        _musicSource.Play();
    }
}
