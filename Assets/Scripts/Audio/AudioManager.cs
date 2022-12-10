using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    [SerializeField]
    private AudioMixer mainAudioMixer;

    public void SetMasterVolume(float volume) {
        mainAudioMixer.SetFloat("MasterVolume", volume);
    }
    public void SetMusicVolume(float volume) {
        mainAudioMixer.SetFloat("MusicVolume", volume);
    }
    public void SetSoundEffectsVolume(float volume) {
        mainAudioMixer.SetFloat("SFXVolume", volume);
    }

    public static float ConvertToDecibelVolume(float normalizedVolumeRange) {
        float volume = Mathf.Log10(normalizedVolumeRange) * 20; //We need to convert the 0-1 value to the logarithmic volume scale
        if (normalizedVolumeRange == 0) {
            volume = -80f; //This is the lowest it goes on the audio mixer
        }
        return volume;
    }
}
