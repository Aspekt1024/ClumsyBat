using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioController : MonoBehaviour
{
    private bool _enabled;
    private SampleType _musicSample;
    private string _pathFromAudioFolder;
    private AudioSource _primaryAudio;
    private AudioSource _secondaryAudio;
    private readonly Dictionary<int, SampleType> _audioDict = new Dictionary<int, SampleType>();

    private const int InvalidSound = -1;

    protected bool IsOnRepeat;
    protected AudioTypes AudioType;
    protected string PathFromAudioFolder
    {
        get { return _pathFromAudioFolder; }
        set
        {
            value = FormatForwardSlashes(value);
            _pathFromAudioFolder = value;
        }
    }

    private StatsHandler _dataHandler;

    protected enum AudioTypes
    {
        Music,
        SoundFx
    }

    protected struct SampleType
    {
        public float Volume;
        public AudioClip AudioClip;
    }

    private void OnEnable()
    {
        EventListener.OnMusicToggle += ToggleMusic;
        EventListener.OnSfxToggle += ToggleSfx;
    }
    private void OnDisable()
    {
        EventListener.OnMusicToggle -= ToggleMusic;
        EventListener.OnSfxToggle -= ToggleSfx;
    }
    
    private void ToggleMusic()
    {
        if (AudioType != AudioTypes.Music) return;  // TODO setup eventHandler better
        _enabled = _dataHandler.Settings.Music;
        if (!_enabled)
            StopAllSounds();
        else
            ResumeMusic();
    }

    private void ToggleSfx()
    {
        if (AudioType != AudioTypes.SoundFx) return;  // TODO setup eventHandler better
        _enabled = _dataHandler.Settings.Sfx;
        if (!_enabled)
            StopAllSounds();
    }

    private void Awake()
    {
        _enabled = true;
        _primaryAudio = gameObject.AddComponent<AudioSource>();
        _secondaryAudio = gameObject.AddComponent<AudioSource>();
        SetupAudioProperties();
        SetupAudioDict();
    }

    private void Start()
    {
        _dataHandler = GameData.Instance.Data.Stats;
        switch (AudioType)
        {
            case AudioTypes.Music:
                _enabled = _dataHandler.Settings.Music;
                break;
            case AudioTypes.SoundFx:
                _enabled = _dataHandler.Settings.Sfx;
                break;
        }
    }

    /// <summary>
    /// Set IsOnRepeat and PathFromAudioFolder. By default, the audio does not repeat,
    /// and the path is the root of /Audio
    /// </summary>
    protected abstract void SetupAudioProperties();
    protected abstract void SetupAudioDict();
    
    protected void AddToAudioDict(object soundName, string fileName, float volume)
    {
        int soundIndex = ConvertEnumToInt(soundName);
        var newSample = new SampleType
        {
            Volume = volume,
            AudioClip = Resources.Load<AudioClip>("Audio/" + _pathFromAudioFolder + fileName)
        };
        _audioDict.Add(soundIndex, newSample);
    }

    public void PlaySound(object soundName)
    {
        int soundIndex = ConvertEnumToInt(soundName);
        if (AudioType == AudioTypes.Music) _musicSample = _audioDict[soundIndex];
        if (!_enabled || soundIndex == InvalidSound) return;

        AudioSource aSource = SelectAudioSource();
        FadeOutOtherSourceIfPlaying(aSource);
        SetToPlay(aSource, _audioDict[soundIndex]);
    }

    private void SetToPlay(AudioSource aSource, SampleType sample)
    {
        aSource.volume = sample.Volume;
        aSource.clip = sample.AudioClip;
        aSource.loop = IsOnRepeat;
        aSource.Play();
    }

    private void StopAllSounds()
    {
        if (_secondaryAudio.isPlaying)
            StartCoroutine("FadeOutSound", _secondaryAudio);
        if (_primaryAudio.isPlaying)
            StartCoroutine("FadeOutSound", _primaryAudio);
    }

    private void ResumeMusic()
    {
        SetToPlay(_primaryAudio, _musicSample);
        _primaryAudio.Play();
    }

    private AudioSource SelectAudioSource()
    {
        if (!_primaryAudio.isPlaying) return _primaryAudio;
        if (!_secondaryAudio.isPlaying) return _secondaryAudio;
        return _primaryAudio;
    }

    private void FadeOutOtherSourceIfPlaying(AudioSource aSource)
    {
        if (aSource == _primaryAudio)
        {
            if (!_secondaryAudio.isPlaying) return;
            StartCoroutine("FadeOutSound", _secondaryAudio);
        }
        else
        {
            if (!_primaryAudio.isPlaying) return;
            StartCoroutine("FadeOutSound", _primaryAudio);
        }
    }

    private IEnumerator FadeOutSound(AudioSource aSource)
    {
        // This is to remove the popping sound when a sound is stopped
        const float fadeDuration = 0.04f;
        float fadeTimer = 0f;
        float startVolume = aSource.volume;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            aSource.volume = startVolume * (1 - fadeTimer / fadeDuration);
            yield return null;
        }
        aSource.Stop();
    }

    private static int ConvertEnumToInt(object soundName)
    {
        if (soundName.GetType().IsEnum) return (int)soundName;
        ReportInvalidSound(soundName.ToString());
        return -1;
    }
    private static void ReportInvalidSound(string soundName)
    {
        Debug.Log("Unable to play sound: " + soundName);
    }

    #region Path Parameter Forward Slash Formatting Methods
    private static string FormatForwardSlashes(string str)
    {
        if (str == string.Empty) { return str; }
        str = AssertEndingForwardSlash(str);
        str = AssertNoLeadingForwardSlash(str);
        return str;
    }

    private static string AssertEndingForwardSlash(string str)
    {
        return str.Substring(str.Length - 1, 1) == "/" ? str : str + "/";
    }

    private static string AssertNoLeadingForwardSlash(string str)
    {
        return str.Substring(0, 1) == "/" ? str.Substring(1, str.Length - 1) : str;
    }
    #endregion
}
