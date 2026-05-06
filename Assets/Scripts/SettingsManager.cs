using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Добавили для работы с ползунками из кода

public class SettingsManager : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject settingsPanel;
    public GameObject pausePanel;

    [Header("Настройки Звука")]
    public AudioMixer mixer;

    // Сюда мы перетащим ползунки, чтобы они знали, где стоять при старте игры
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public void OpenSettings()
    {
        // Включаем панель настроек
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        // Выключаем панель паузы (чтобы они не накладывались друг на друга)
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Загружаем сохраненные настройки (или ставим 0, если запускаем впервые)
        float masterV = PlayerPrefs.GetFloat("MasterVol", 0f);
        float musicV = PlayerPrefs.GetFloat("MusicVol", 0f);
        float sfxV = PlayerPrefs.GetFloat("SFXVol", 0f);

        // Двигаем ползунки в сохраненное положение
        if (masterSlider != null) masterSlider.value = masterV;
        if (musicSlider != null) musicSlider.value = musicV;
        if (sfxSlider != null) sfxSlider.value = sfxV;

        // Применяем звук в микшер
        mixer.SetFloat("MasterVol", masterV);
        mixer.SetFloat("MusicVol", musicV);
        mixer.SetFloat("SFXVol", sfxV);
    }

    public void SetMasterVolume(float sliderValue)
    {
        mixer.SetFloat("MasterVol", sliderValue);
        PlayerPrefs.SetFloat("MasterVol", sliderValue); // Сохраняем навсегда
    }

    public void SetMusicVolume(float sliderValue)
    {
        mixer.SetFloat("MusicVol", sliderValue);
        PlayerPrefs.SetFloat("MusicVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mixer.SetFloat("SFXVol", sliderValue);
        PlayerPrefs.SetFloat("SFXVol", sliderValue);
    }
}