using UnityEngine;
using TMPro; // Подключаем библиотеку для работы с TextMeshPro

public class ScoreManager : MonoBehaviour
{
    // Делаем скрипт доступным отовсюду (Singleton)
    public static ScoreManager instance;

    [Header("UI Элементы")]
    public TMP_Text scoreText; // Сюда перетащим наш текст

    private int currentScore = 0;

    void Awake()
    {
        // Проверяем, чтобы на сцене был только один ScoreManager
        if (instance == null)
        {
            instance = this;
        }
    }

    // Эту функцию будут вызывать коты и доски, когда разрушаются
    public void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = "ОЧКИ: " + currentScore.ToString();
    }
}