using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioController : MonoBehaviour
{
    protected bool IsOnRepeat;
    private string _pathFromAudioFolder;
    private AudioSource _primaryAudio;
    private AudioSource _secondaryAudio;
    private readonly Dictionary<int, SampleType> _audioDict = new Dictionary<int, SampleType>();

    private const int InvalidSound = -1;

    protected string PathFromAudioFolder
    {
        get { return _pathFromAudioFolder; }
        set
        {
            value = FormatForwardSlashes(value);
            _pathFromAudioFolder = value;
        }
    }

    protected struct SampleType
    {
        public float Volume;
        public AudioClip AudioClip;
    }

    private void Awake()
    {
        _primaryAudio = gameObject.AddComponent<AudioSource>();
        _secondaryAudio = gameObject.AddComponent<AudioSource>();
        SetupAudioProperties();
        SetupAudioDict();
    }

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
        if (soundIndex == InvalidSound) return;
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
