using System.Collections.Generic;
using UnityEngine;

public abstract class AudioController : MonoBehaviour
{
    private string _pathFromAudioFolder;
    private AudioSource _audio;
    private readonly Dictionary<int, SampleType> _audioDict = new Dictionary<int, SampleType>();

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
        _audio = gameObject.AddComponent<AudioSource>();
        SetupAudioDict();
    }
    
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
        _audio.volume = 0;
        _audio.Stop();   // TODO remove the popping sound.
        _audio.volume = _audioDict[soundIndex].Volume;
        _audio.PlayOneShot(_audioDict[soundIndex].AudioClip);
    }

    private static int ConvertEnumToInt(object soundName)
    {
        if (soundName.GetType().IsEnum) return (int)soundName;
        return 0;
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
