using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuSliderBehavior : MonoBehaviour {

    [SerializeField]
    private GameObject musicVolumeSlider;
    private Slider musicVolumeSliderSlider;
    [SerializeField]
    private TextMeshProUGUI musicVolumeText;

    [SerializeField]
    private GameObject sfxVolumeSlider;
    private Slider sfxVolumeSliderSlider;
    [SerializeField]
    private TextMeshProUGUI sfxVolumeText;

    private void Start() {
        musicVolumeSliderSlider = musicVolumeSlider.GetComponent<Slider>();

        //musicVolumeSliderSlider.maxValue = 1;
        //musicVolumeSliderSlider.value = 1;

        sfxVolumeSliderSlider = sfxVolumeSlider.GetComponent<Slider>();

        //sfxVolumeSliderSlider.maxValue = 1;
        //sfxVolumeSliderSlider.value = 1;
    }

    public void SetSFXVolume(float volume) {
        if (sfxVolumeSliderSlider == null) {
            Start();
        }
        sfxVolumeSliderSlider.value = volume;
        UpdateSFXVolume();
    }
    public void SetMusicVolume(float volume) {
        if (musicVolumeSliderSlider == null) {
            Start();
        }
        musicVolumeSliderSlider.value = volume;
        UpdateMusicVolume();
    }

    public void UIChangeSFXVolume() {
        sfxVolumeText.text = (int)(sfxVolumeSliderSlider.value * 100) + "/" + (int)(sfxVolumeSliderSlider.maxValue * 100);
    }
    public void UIChangeMusicVolume() {
        musicVolumeText.text = (int)(musicVolumeSliderSlider.value * 100) + "/" + (int)(musicVolumeSliderSlider.maxValue * 100);
    }

    public void UpdateSFXVolume() {
        GameManager.Instance.SetSoundEffectsVolume(AudioManager.ConvertToDecibelVolume(sfxVolumeSliderSlider.value));
        GameManager.Instance.SaveGameSettings(musicVolumeSliderSlider.value, sfxVolumeSliderSlider.value);
    }
    public void UpdateMusicVolume() {
        GameManager.Instance.SetMusicVolume(AudioManager.ConvertToDecibelVolume(musicVolumeSliderSlider.value));
        GameManager.Instance.SaveGameSettings(musicVolumeSliderSlider.value, sfxVolumeSliderSlider.value);
    }
}
