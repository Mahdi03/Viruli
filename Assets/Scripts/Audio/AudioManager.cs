using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    [SerializeField]
    private AudioMixer mainAudioMixer;

    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }
    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Now we can instantiate stuff if needed
            //audioManager = GetComponent<AudioManager>();
            //Time.timeScale = 2;
            //ClearAllGameSettings();

        }
    }

    private void Start() {
        /*
        foreach (AudioMixerGroup a in mainAudioMixer.FindMatchingGroups(string.Empty)) {
            Debug.Log(a.name);
        }
        */
    }

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

    public AudioMixerGroup enemyInjuredAudioMixerGroup;
    [SerializeField]
    private Transform oneShotAudiosParent;

    /// <summary>
    /// 
    /// </summary>
    /// https://answers.unity.com/questions/975754/how-to-route-sound-from-playclipatpoint-to-a-speci.html
    /// <param name="clip">AudioClip to play</param>
    /// <param name="position">Position of where to emanate the sound</param>
    /// <param name="volume">optional volume parameter (default 1)</param>
    /// <param name="audioMixerGroup"></param>
    public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f, AudioMixerGroup audioMixerGroup = null) {
        if (clip == null) {
            return;
        }

        GameObject audioGameObject = new GameObject("Custom one shot audio");
        audioGameObject.transform.SetParent(oneShotAudiosParent, false);
        audioGameObject.transform.position = position;
        AudioSource audioSource = audioGameObject.AddComponent<AudioSource>();
        if (audioMixerGroup!= null) {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Object.Destroy(audioGameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
    public void PauseAllOneShotAudios() {
        //Loop thru all one shot audios in parent and pause their audio source components
        foreach (Transform oneShotAudioObject in oneShotAudiosParent) {
            oneShotAudioObject.GetComponent<AudioSource>().Pause();
        }
    }
    public void UnPauseAllOneShotAudios() {
        //Loop thru all one shot audios in parent and unpause their audio source components
        foreach (Transform oneShotAudioObject in oneShotAudiosParent) {
            oneShotAudioObject.GetComponent<AudioSource>().UnPause();
        }
    }
}
