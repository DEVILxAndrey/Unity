using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject pauseMenuPanel; // Ссылка на нашу панель

    void Start()
    {
        // При старте уровня убеждаемся, что меню паузы скрыто, а время идет
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Показываем меню
        Time.timeScale = 0f;            // ОСТАНАВЛИВАЕМ ВРЕМЯ (физика замирает)
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Прячем меню
        Time.timeScale = 1f;             // ВОЗОБНОВЛЯЕМ ВРЕМЯ
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Обязательно возвращаем время перед загрузкой!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Убедись, что сцена называется именно так
    }

    // Заглушки для будущих функций
    public void OpenLevelSelection()
    {
        // Обязательно возвращаем время в норму! 
        // Иначе, если ты нажал кнопку из меню паузы, главное меню загрузится замороженным.
        Time.timeScale = 1f;

        // Загружаем сцену Главного меню (у нас она под индексом 0)
        SceneManager.LoadScene(0);
    }

    public void OpenSettings()
    {
        Debug.Log("Здесь будет открытие настроек");
    }
}