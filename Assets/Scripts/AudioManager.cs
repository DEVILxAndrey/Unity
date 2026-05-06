using UnityEngine;
using System.Collections; // Обязательно для корутин!

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Плееры")]
    public AudioSource sfxSource;
    public AudioSource musicSource; // Сюда положим плеер с музыкой

    [Header("Звуковые эффекты")]
    public AudioClip shootSound;
    public AudioClip catDeathSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip woodBreakSound;
    public AudioClip glassBreakSound;
    public AudioClip stretchSound; 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // --- МАГИЯ ЗАТУХАНИЯ МУЗЫКИ ---

    // Вызываем при победе/поражении
    public void FadeMusicOut()
    {
        if (musicSource != null)
        {
            StartCoroutine(FadeOutCoroutine(1.5f)); // 1.5f - это время затухания в секундах
        }
    }

    // Вызываем при старте нового уровня
    public void ResetMusic()
    {
        if (musicSource != null)
        {
            StopAllCoroutines();     // Останавливаем затухание, если оно еще шло
            musicSource.volume = 1f; // Возвращаем громкость на 100%

            if (!musicSource.isPlaying)
            {
                musicSource.Play();  // Запускаем трек снова, если он остановился
            }
        }
    }

    // Сама логика плавного снижения громкости
    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null; // Ждем до следующего кадра
        }
        musicSource.volume = 0;
        musicSource.Stop();
    }
}