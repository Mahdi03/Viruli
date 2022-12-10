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
}
