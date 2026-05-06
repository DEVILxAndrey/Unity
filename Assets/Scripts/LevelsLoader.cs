using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для переключения уровней

public class LevelLoader : MonoBehaviour
{
    [Header("UI Панели")]
    public GameObject mainButtonsPanel; // Главные кнопки (Играть, Настройки, Выйти)
    public GameObject levelsPanel;      // Наша новая панель уровней

    // Открываем панель уровней
    public void OpenLevelsPanel()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    // Закрываем панель уровней (возврат в главное меню)
    public void CloseLevelsPanel()
    {
        levelsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    // Загрузка самого уровня по его номеру
    public void LoadLevel(int levelIndex)
    {
        // levelIndex - это номер сцены в настройках билда (Build Settings)
        SceneManager.LoadScene(levelIndex);
    }
}